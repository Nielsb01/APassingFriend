using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Npc
{
    public class NpcController : MonoBehaviour
    {
        [SerializeField] private List<Transform> _waypoints;
        [SerializeField] private float _waypointRounding = 0.3f;
        [SerializeField] private bool _patrolling = false;
        [SerializeField] private GameObject _pathNodePrefab;
        private NavMeshAgent _navMeshAgent;

        void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            if (_waypointRounding < 0.01f)
            {
                _waypointRounding = 0.01f;
            }
        }

        void Update()
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

        public void SetPathByWaypoint(List<Transform> pathNodes)
        {
            _waypoints = pathNodes;
        }

        public void SetPathByWaypoint(List<Vector3> pathNodes)
        {
            _waypoints = new List<Transform>();
            foreach (var node in pathNodes)
            {
                _waypoints.Add(TurnVector3IntoTransform(node));
            }
        }

        public void SetPathByWaypoint(Transform pathNode)
        {
            _waypoints = new List<Transform> { pathNode };
        }

        public void SetPathByWaypoint(Vector3 pathNode)
        {
            _waypoints = new List<Transform>
            {
                TurnVector3IntoTransform(pathNode)
            };
        }

        private void NavigateToNextWaypoint()
        {
            if (_waypoints.Count > 0)
            {
                _navMeshAgent.destination = _waypoints.First().position;
            }
        }

        private Transform TurnVector3IntoTransform(Vector3 vector3)
        {
            return new GameObject("PathNode", typeof(Transform))
            {
                transform =
                {
                    position = vector3
                }
            }.transform;
        }
    }
}