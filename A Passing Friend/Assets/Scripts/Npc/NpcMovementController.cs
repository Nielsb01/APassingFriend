using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Npc
{
    public class NpcMovementController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _waypoints;
        [SerializeField] private float _defaultWaypointRounding = 0.3f;
        [SerializeField] private bool _patrolling = false;
        [SerializeField] private GameObject _pathNodePrefab;
        private NavMeshAgent _navMeshAgent;
        private VariableDeclarations _variables;
        private float _waypointRoundingForNextNode;
        private bool _skipNextNodeActions = true;
        private GameObject _currentTravelDestinationNode;
        private const float MINIMUM_ROUNDING = 0.1f;
        private const string ROUNDING_VARIABLE_NAME = "RoundingForThisNode";
        private const string TRIGGER_VARIABLE_NAME = "TriggerEvent";
        private const string SPEED_CHANGE_VARIABLE_NAME = "NewMovementSpeed";
        private const string WAIT_TIME_VARIABLE_NAME = "WaitTimeAtThisNode";

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            if (_defaultWaypointRounding < MINIMUM_ROUNDING)
            {
                _defaultWaypointRounding = MINIMUM_ROUNDING;
            }

            _waypointRoundingForNextNode = _defaultWaypointRounding;
        }

        private void Update()
        {
            var destination = new Vector2(_navMeshAgent.destination.x, _navMeshAgent.destination.z);
            var currentPos = new Vector2(transform.position.x, transform.position.z);
            if (!((destination - currentPos).magnitude < _waypointRoundingForNextNode) || _waypoints.Count <= 0) return;

            if (!_skipNextNodeActions)
            {
                ExecuteNodeEffects();
            }

            NavigateToNextWaypoint();
        }

        public void SetPathWaypoint(List<GameObject> pathNodes)
        {
            _waypoints = pathNodes;
        }

        public void SetPathWaypoint(List<Vector3> pathNodes)
        {
            _waypoints = new List<GameObject>();
            foreach (var node in pathNodes)
            {
                _waypoints.Add(CreateWaypointOnPosition(node));
            }
        }

        public void SetPathWaypoint(GameObject pathNode)
        {
            _waypoints = new List<GameObject> { pathNode };
        }

        public void SetPathWaypoint(Vector3 pathNode)
        {
            _waypoints = new List<GameObject>
            {
                CreateWaypointOnPosition(pathNode)
            };
        }

        private void NavigateToNextWaypoint()
        {
            if (_waypoints.Count <= 0) return;

            _navMeshAgent.destination = _waypoints.First().transform.position;
            CheckIfNextNodeHasEffect(_waypoints.First());
            _currentTravelDestinationNode = _waypoints.First();

            if (_patrolling)
            {
                var point = _waypoints.First();
                _waypoints.Remove(point);
                _waypoints.Add(point);
            }
            else
            {
                _waypoints.Remove(_waypoints.First());
            }

            _skipNextNodeActions = false;
        }

        private GameObject CreateWaypointOnPosition(Vector3 vector3)
        {
            return Instantiate(_pathNodePrefab, vector3, Quaternion.identity);
        }

        private void CheckIfNextNodeHasEffect(GameObject node)
        {
            _variables = node.GetComponent<Variables>().declarations;
            var roundingForThisNode = GetFloatVariableFromNextNode(ROUNDING_VARIABLE_NAME);
            if (roundingForThisNode == 0)
            {
                _waypointRoundingForNextNode = _defaultWaypointRounding;
            }
            else
            {
                _waypointRoundingForNextNode =
                    roundingForThisNode >= MINIMUM_ROUNDING ? roundingForThisNode : MINIMUM_ROUNDING;
            }
        }

        private void ExecuteNodeEffects()
        {
            ExecuteMovementSpeedChange();

            var waitTimeAtThisNode = GetFloatVariableFromNextNode(WAIT_TIME_VARIABLE_NAME);
            if (waitTimeAtThisNode > 0)
            {
                StartCoroutine(WaitForSeconds(waitTimeAtThisNode));
            }

            if (GetBoolVariableFromNextNode(TRIGGER_VARIABLE_NAME))
            {
                ApplyTriggerEvent();
            }
        }

        private void ExecuteMovementSpeedChange()
        {
            var newMovementSpeed = GetFloatVariableFromNextNode(SPEED_CHANGE_VARIABLE_NAME);
            if (newMovementSpeed > 0)
            {
                _navMeshAgent.speed = newMovementSpeed;
            }
        }

        private float GetFloatVariableFromNextNode(string variableName)
        {
            return (float)(_variables.Get(variableName) ?? 0.1f);
        }

        private bool GetBoolVariableFromNextNode(string variableName)
        {
            return (bool)(_variables.Get(variableName) ?? false);
        }

        private void ApplyTriggerEvent()
        {
            try
            {
                _currentTravelDestinationNode.GetComponent<NpcMoveNodeTrigger>().Trigger();
            }
            catch (Exception e)
            {
                Debug.LogError("Could not run trigger: " + e);
            }
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