using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Npc
{
    public class NpcMovementController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _waypointsRoute = new List<GameObject>();
        [SerializeField] private float _defaultWaypointRounding = 0.3f;
        [SerializeField] private bool _patrolling = false;
        [SerializeField] private WaypointRoute _route;
        private NavMeshAgent _navMeshAgent;
        private float _waypointRoundingForNextNode;
        private bool _skipNextNodeActions = true;
        private GameObject _currentTravelDestinationNode;
        private const float MINIMUM_ROUNDING = 0.1f;
        private PathNodeController _pathNodeController;

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
            GoToNextWaypoint();
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

        private void GoToNextWaypoint()
        {
            if (_waypointsRoute.Count == 0)
            {
                Debug.Log("1");
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
            }
            
            Debug.Log("waipoynts: " + _waypointsRoute.Count);
            if (_waypointsRoute.Count == 0)
            {
                Debug.Log("2");
                return;
            }

            Debug.Log("3");
            _currentTravelDestinationNode = _waypointsRoute.First();
            _navMeshAgent.destination = _currentTravelDestinationNode.transform.position;
            _pathNodeController = _currentTravelDestinationNode.GetComponent<PathNodeController>();
            SetRoundingForNextNode(_currentTravelDestinationNode);
            _skipNextNodeActions = false;
        }

        private void SetRoundingForNextNode(GameObject node)
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