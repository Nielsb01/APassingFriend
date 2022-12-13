using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Npc
{
    public class NpcMovementController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _waypointsRoute = new();
        [SerializeField] private float _defaultWaypointRounding = 0.3f;
        [SerializeField] private bool _patrolling;
        [SerializeField] private WaypointRoute _route;
        [SerializeField] private GameObject _followingChild;
        private NavMeshAgent _navMeshAgent;
        private float _waypointRoundingForNextNode;
        private GameObject _currentTravelDestinationNode;
        private const float MINIMUM_ROUNDING = 0.1f;
        private PathNodeController _pathNodeController;
        private bool _teleportingToNextNode;
        private bool _teleportingBallAfterTeleport;

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            if (_defaultWaypointRounding < MINIMUM_ROUNDING)
            {
                _defaultWaypointRounding = MINIMUM_ROUNDING;
            }

            _waypointRoundingForNextNode = _defaultWaypointRounding;

            if (_route != null)
            {
                StartRoute(_route);
            }

            if (_waypointsRoute.Count != 0)
            {
                GoToNextWaypoint(false);
            }
        }

        private void Update()
        {
            if (!IsNpcAtDestination()) return;

            ExecuteNodeEffects();
            GoToNextWaypoint();
        }

        private bool IsNpcAtDestination()
        {
            var dest = _navMeshAgent.destination;
            var pos = transform.position;
            var destination = new Vector2(dest.x, dest.z);
            var currentPos = new Vector2(pos.x, pos.z);
            return (destination - currentPos).magnitude < _waypointRoundingForNextNode;
        }

        public void StartRoute(WaypointRoute route)
        {
            _waypointsRoute = route.waypoints;
            _patrolling = route.isPatrol;
        }

        private void GoToNextWaypoint(bool removeNextNodeFromQueue = true)
        {
            if (removeNextNodeFromQueue)
            {
                if (_waypointsRoute.Count == 0)
                {
                    return;
                }

                if (_patrolling)
                {
                    var point = _waypointsRoute.First();
                    _waypointsRoute.Remove(point);
                    _waypointsRoute.Add(point);
                }
                else
                {
                    _waypointsRoute.Remove(_waypointsRoute.First());
                    if (_waypointsRoute.Count == 0) return;
                }
            }

            _currentTravelDestinationNode = _waypointsRoute.First();
            _navMeshAgent.destination = _currentTravelDestinationNode.transform.position;
            _pathNodeController = _currentTravelDestinationNode.GetComponent<PathNodeController>();
            SetRoundingForNextNode();

            if (_teleportingToNextNode)
            {
                _teleportingToNextNode = false;
                transform.position = _currentTravelDestinationNode.transform.position;
                if (_teleportingBallAfterTeleport)
                {
                    _teleportingBallAfterTeleport = false;
                    _followingChild.transform.position = transform.position;
                }
            }
        }

        private void SetRoundingForNextNode()
        {
            var roundingForNextNode = _pathNodeController.RoundingForThisNode;
            if (SettingDisabled(roundingForNextNode))
            {
                _waypointRoundingForNextNode = _defaultWaypointRounding;
            }
            else
            {
                _waypointRoundingForNextNode =
                    roundingForNextNode >= MINIMUM_ROUNDING ? roundingForNextNode : MINIMUM_ROUNDING;
            }
        }

        private void ExecuteNodeEffects()
        {
            if (_pathNodeController.TeleportToNextNode)
            {
                _teleportingToNextNode = true;
            }

            var newMovementSpeed = _pathNodeController.NewMovementSpeed;
            if (!SettingDisabled(newMovementSpeed))
            {
                _navMeshAgent.speed = newMovementSpeed;
            }

            var waitTimeAtThisNode = _pathNodeController.WaitTimeAtThisNode;
            if (!SettingDisabled(waitTimeAtThisNode))
            {
                StartCoroutine(WaitForSeconds(waitTimeAtThisNode));
            }

            if (_pathNodeController.LockBallToController)
            {
                var controller = _followingChild.GetComponent<NpcBallController>();
                controller.LockedToController = true;
                _teleportingBallAfterTeleport = true;
            }

            if (_pathNodeController.UnlockBallFromController)
            {
                var controller = _followingChild.GetComponent<NpcBallController>();
                controller.LockedToController = false;
            }

            var newBallSpeed = _pathNodeController.BallFollowSpeedFromThisNode;
            if (!SettingDisabled(newBallSpeed))
            {
                _followingChild.GetComponent<NpcBallController>().FollowControllerSpeed = newBallSpeed;
            }

            var bounceStrengthFromThisNode = _pathNodeController.BounceStrengthFromThisNode;
            if (!SettingDisabled(bounceStrengthFromThisNode))
            {
                _followingChild.GetComponent<NpcBallController>().BounceStrength = bounceStrengthFromThisNode;
            }

            if (_pathNodeController.TriggerScripts.Count > 0)
            {
                _pathNodeController.Trigger();
            }
        }

        private static bool SettingDisabled(float value)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return value == -1;
        }

        private IEnumerator WaitForSeconds(float time)
        {
            var speed = _navMeshAgent.speed;
            _navMeshAgent.speed = 0;
            yield return new WaitForSeconds(time);
            _navMeshAgent.speed = speed;
        }
    }
}