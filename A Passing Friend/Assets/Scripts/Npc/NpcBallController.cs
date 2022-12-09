using UnityEngine;
using UnityEngine.AI;

namespace Npc
{
    public class NpcBallController : MonoBehaviour
    {
        [SerializeField] private GameObject npc;
        private NavMeshAgent _npcAgent;
        private bool _grounded = false;
        private Rigidbody _rb;
        private bool _lockedToController = false;
        [SerializeField] private float _bounceStrength = 2;
        [SerializeField] private float _followControllerSpeed = 3;

        public bool LockedToController
        {
            get => _lockedToController;
            set => _lockedToController = value;
        }

        public float BounceStrength
        {
            get => _bounceStrength;
            set => _bounceStrength = value;
        }

        public float FollowControllerSpeed
        {
            get => _followControllerSpeed;
            set => _followControllerSpeed = value;
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _npcAgent = npc.GetComponent<NavMeshAgent>();
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
            var npcPos = npc.transform.position;
            transform.position = new Vector3(npcPos.x, transform.position.y, npcPos.z);
            _rb.velocity = _npcAgent.velocity;
        }

        private void Bounce()
        {
            _rb.AddForce(new Vector3(0, _bounceStrength, 0), ForceMode.Impulse);
        }

        private void TrackControllerGameObject()
        {
            _rb.AddForce((npc.transform.position - transform.position) * _followControllerSpeed);
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
            transform.position = npc.transform.position;
        }
    }
}