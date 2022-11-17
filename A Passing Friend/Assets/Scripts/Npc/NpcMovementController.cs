using System.Collections.Generic;
using System.Linq;
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

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            if (_waypointRounding < 0.01f)
            {
                _waypointRounding = 0.01f;
            }
        }

        private void Update()
        {
            var destination = new Vector2(_navMeshAgent.destination.x, _navMeshAgent.destination.z);
            var currentPos = new Vector2(transform.position.x, transform.position.z);
            if ((destination - currentPos).magnitude < _waypointRounding && _waypoints.Count > 0)
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
            }
        }

        private GameObject CreateWaypointOnPosition(Vector3 vector3)
        {
            return Instantiate(_pathNodePrefab, vector3, Quaternion.identity);
        }
    }
}