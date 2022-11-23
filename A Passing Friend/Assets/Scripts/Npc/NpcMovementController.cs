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
        [SerializeField] private float _waypointRounding = 0.3f;
        [SerializeField] private bool _patrolling = false;
        [SerializeField] private GameObject _pathNodePrefab;
        private NavMeshAgent _navMeshAgent;
        private VariableDeclarations _variables;
        private float _waypointRoundingForNextNode;
        private bool _skipNextNodeActions = true;
        private GameObject _currentTravelDestinationNode;

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            if (_waypointRounding < 0.01f)
            {
                _waypointRounding = 0.01f;
            }

            _waypointRoundingForNextNode = _waypointRounding;
        }

        private void Update()
        {
            var destination = new Vector2(_navMeshAgent.destination.x, _navMeshAgent.destination.z);
            var currentPos = new Vector2(transform.position.x, transform.position.z);
            if ((destination - currentPos).magnitude < _waypointRoundingForNextNode && _waypoints.Count > 0)
            {
                if (!_skipNextNodeActions)
                {
                    ApplyNodeEffects();
                }

                NavigateToNextWaypoint();

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
        }

        private GameObject CreateWaypointOnPosition(Vector3 vector3)
        {
            return Instantiate(_pathNodePrefab, vector3, Quaternion.identity);
        }

        private void CheckIfNextNodeHasEffect(GameObject node)
        {
            _variables = node.GetComponent<Variables>().declarations;
            var roundingForThisNode = GetFloatVariableFromNextNode("RoundingForThisNode");
            SetRoundingForNextWaypoint(roundingForThisNode > 0, roundingForThisNode);
        }

        private void ApplyNodeEffects()
        {
            ApplyMovementSpeedChange();

            var waitTimeAtThisNode = GetFloatVariableFromNextNode("WaitTimeAtThisNode");
            if (waitTimeAtThisNode > 0)
            {
                ApplyWaitTimeAtThisNode(waitTimeAtThisNode);
            }

            if (GetBoolVariableFromNextNode("TriggerEvent"))
            {
                ApplyTriggerEvent();
            }
        }

        private void SetRoundingForNextWaypoint(bool customRounding, float rounding)
        {
            _waypointRoundingForNextNode = customRounding ? rounding : _waypointRounding;
        }

        private void ApplyMovementSpeedChange()
        {
            var newMovementSpeed = GetFloatVariableFromNextNode("NewMovementSpeed");
            if (newMovementSpeed > 0)
            {
                _navMeshAgent.speed = newMovementSpeed;
            }
        }

        private float GetFloatVariableFromNextNode(string variableName)
        {
            var value = GetVariableFromNextNode(variableName);
            if (value == null)
            {
                return 0f;
            }

            return (float)value;
        }

        private bool GetBoolVariableFromNextNode(string variableName)
        {
            var value = GetVariableFromNextNode(variableName);
            if (value == null)
            {
                return false;
            }

            return (bool)value;
        }


        private object GetVariableFromNextNode(string variableName)
        {
            try
            {
                return _variables.Get(variableName);
            }
            catch
            {
                Debug.Log("Could not find var");
                return null;
            }
        }

        private void ApplyWaitTimeAtThisNode(float time)
        {
            StartCoroutine(WaitForTimeAtNode(time));
        }

        private void ApplyTriggerEvent()
        {
            try
            {
                _currentTravelDestinationNode.GetComponent<NpcMoveNodeTrigger>().Trigger();
            }
            catch (Exception e)
            {
                Debug.Log("Could not run trigger: " + e);
            }
        }

        private IEnumerator WaitForTimeAtNode(float time)
        {
            var speed = _navMeshAgent.speed;
            _navMeshAgent.speed = 0;
            yield return new WaitForSeconds(time);
            _navMeshAgent.speed = speed;
        }
    }
}