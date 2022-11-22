using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Npc
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

        private void Start()
        {
            _variables = GetComponent<Variables>().declarations;
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

                NavigateToNextWaypoint();
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
            if (_waypoints.Count > 0)
            {
                _navMeshAgent.destination = _waypoints.First().transform.position;
                CheckIfNextNodeHasEffect(_waypoints.First());
            }
        }

        private GameObject CreateWaypointOnPosition(Vector3 vector3)
        {
            return Instantiate(_pathNodePrefab, vector3, Quaternion.identity);
        }

        private void CheckIfNextNodeHasEffect(GameObject node)
        {
            Debug.Log(1);
            Debug.Log(_variables);
            Debug.Log(2);
            Debug.Log(_variables.Get("RoundingForThisNode"));
            Debug.Log(3);
            Debug.Log((float)_variables.Get("RoundingForThisNode"));
            ApplyMovementSpeedChange();

            var roundingForThisNode = (float)_variables.Get("RoundingForThisNode");
            SetRoundingForNextWaypoint(roundingForThisNode > 0, roundingForThisNode);

            var waitTimeAtThisNode = (float)_variables.Get("WaitTimeAtThisNode");
            if (waitTimeAtThisNode > 0)
            {
                ApplyWaitTimeAtThisNode(waitTimeAtThisNode);
            }

            var triggerEvent = (bool)_variables.Get("TriggerEvent");
            if (triggerEvent)
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
            var newMovementSpeed = (float)_variables.Get("NewMovementSpeed");
            if (newMovementSpeed > 0)
            {
                _navMeshAgent.speed = newMovementSpeed;
            }
        }

        private void ApplyWaitTimeAtThisNode(float time)
        {
            StartCoroutine(WaitForTimeAtNode(time));
        }

        private void ApplyTriggerEvent()
        {
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