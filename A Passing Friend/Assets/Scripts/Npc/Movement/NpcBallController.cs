using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Npc
{
    public class NpcBallController : MonoBehaviour
    {
        [SerializeField] private GameObject _npc;
        private NavMeshAgent _npcAgent;
        private bool _grounded;
        private Rigidbody _rb;
        private bool _lockedToController;
        [SerializeField] private float _bounceStrength = 2;
        [SerializeField] private float _followControllerSpeed = 3;

        public bool LockedToController
        {
            set => _lockedToController = value;
        }

        public float BounceStrength
        {
            set => _bounceStrength = value;
        }

        public float FollowControllerSpeed
        {
            set => _followControllerSpeed = value;
        }

        public void UnlockFromController()
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            _lockedToController = false;
        }
        public void StartLockToController()
        {
            _lockedToController = true;
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _npcAgent = _npc.GetComponent<NavMeshAgent>();
        }

        private void FixedUpdate()
        {
            if (_lockedToController)
            {
                LockToController();
                return;
            }

            TrackControllerGameObject();

            if (_grounded)
            {
                Bounce();
            }
        }

        private void LockToController()
        {
            var npcPos = _npc.transform.position;
            transform.position = new Vector3(npcPos.x, transform.position.y, npcPos.z);
            _rb.velocity = _npcAgent.velocity;
        }

        private void Bounce()
        {
            _rb.AddForce(Vector3.up * _bounceStrength, ForceMode.Impulse);
        }

        private void TrackControllerGameObject()
        {
            _rb.AddForce((_npc.transform.position - transform.position) * _followControllerSpeed);
        }

        private void OnCollisionEnter(Collision collision)
        {
            _grounded = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            _grounded = false;
        }

        public void TeleportToController()
        {
            transform.position = _npc.transform.position;
        }
    }
}