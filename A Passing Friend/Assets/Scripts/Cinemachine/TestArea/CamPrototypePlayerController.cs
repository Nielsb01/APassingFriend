using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera.TestArea
{
    public class CamPrototypePlayerController : MonoBehaviour
    {
        private Vector3 _movement;
        private Vector2 _turn;
        private CharacterController _characterController;
        [SerializeField] private float moveSpeed = 0.2f;
        [SerializeField] private float gravity = 0.9f;
        [SerializeField] private float turnSpeed = 1.5f;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            Vector3 movementVector =
                transform.TransformDirection(new Vector3(_movement.z * -moveSpeed, -gravity,
                    _movement.x * moveSpeed));
            _characterController.Move(movementVector);
            transform.Rotate(_turn * turnSpeed);
        }

        public void OnMove(InputValue moveVector)
        {
            var inputVector = moveVector.Get<Vector2>();
            _movement = new Vector3(inputVector.x, 0, inputVector.y);
        }

        public void OnTurn(InputValue moveVector)
        {
            var inputVector = moveVector.Get<Vector2>();
            _turn = new Vector3(0, inputVector.x, 0);
        }
    }
}