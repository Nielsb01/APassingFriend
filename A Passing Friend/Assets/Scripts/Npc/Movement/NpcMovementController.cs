using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Interactions;

namespace Npc
{
    public class NpcMovementController : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private List<GameObject> _waypointsRoute = new();
        [SerializeField] private float _defaultWaypointRounding = 0.3f;
        [SerializeField] private bool _patrolling;
        [SerializeField] private WaypointRoute _route;
        [SerializeField] private GameObject _followingChild;
        [SerializeField] private GameObject _npcLoadLocation;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        private float _waypointRoundingForNextNode;
        private GameObject _currentTravelDestinationNode;
        private const float MINIMUM_ROUNDING = 0.1f;
        private PathNodeController _pathNodeController;
        private bool _teleportingToNextNode;
        private bool _teleportingBallAfterTeleport;
        private float _resumeMovementSpeed;

        private NpcAnimationController _npcAnimationController;

        private void Start()
        {
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

            _npcAnimationController = GetComponent<NpcAnimationController>();
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
                _navMeshAgent.enabled = false;
                transform.position = _currentTravelDestinationNode.transform.position;
                _navMeshAgent.enabled = true;
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

            SetAnimation(_pathNodeController.GetAnimationToPlay);

            var waitTimeAtThisNode = _pathNodeController.WaitTimeAtThisNode;
            if (!SettingDisabled(waitTimeAtThisNode))
            {
                StartCoroutine(WaitForSeconds(waitTimeAtThisNode));
            }

            if (_pathNodeController.LockBallToController)
            {
                var controller = _followingChild.GetComponent<NpcBallController>();

                controller.StartLockToController();
                _teleportingBallAfterTeleport = true;
            }

            if (_pathNodeController.UnlockBallFromController)
            {
                var controller = _followingChild.GetComponent<NpcBallController>();
                controller.UnlockFromController();
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
                _pathNodeController.Trigger(this);
            }
        }

        private static bool SettingDisabled(float value)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return value == -1;
        }

        private void SetAnimation(NpcAnimations npcAnimation)
        {
            if (npcAnimation != NpcAnimations.none && _npcAnimationController != null)
            {
                _npcAnimationController.SetAnimationState(npcAnimation);
            }
        }

        private IEnumerator WaitForSeconds(float time)
        {
            PauseNpc();
            yield return new WaitForSeconds(time);
            UnpauseNpc();
        }

        public void PauseNpc()
        {
            _resumeMovementSpeed = _navMeshAgent.speed;
            _navMeshAgent.speed = 0;
        }

        public void UnpauseNpc()
        {
            _navMeshAgent.speed = _resumeMovementSpeed;
            SetAnimation(_pathNodeController.GetAnimationToPlayOnExit);
        }


        public void LoadData(GameData gameData)
        {
            if (_npcLoadLocation == null) return;

            if (name.Equals("Asha") && gameData.ashaIsAtHouse)
            {
                transform.position = _npcLoadLocation.transform.position;
                _waypointsRoute = new List<GameObject> { _npcLoadLocation };
                _patrolling = false;
                _route = null;
                GoToNextWaypoint(false);
                transform.rotation = _npcLoadLocation.transform.rotation;
            }

            if (gameObject.name.Equals("Yarn"))
            {
                switch (gameData.questOneState)
                {
                    case QuestState.Unavailable:
                        break;
                    case QuestState.Available:
                        break;
                    case QuestState.Active:
                        StartRoute(GameObject.FindGameObjectWithTag("YarnRoute").GetComponent<WaypointRoute>());
                        break;
                    case QuestState.PickedUp:
                        Destroy(transform.parent.gameObject);
                        // transform.parent.gameObject.active = false;
                        //Implement ball being picked up by cat
                        break;
                    case QuestState.Completed:
                        break;
                    default:
                        break;
                }
            }
        }

        public void SaveData(ref GameData gameData)
        {
        }
    }
}