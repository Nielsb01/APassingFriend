using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera.TestArea
{
    public class PlayerController : MonoBehaviour
    {
        private Vector3 _movement;
        private Vector2 _turn;
        private CharacterController _characterController;
        public float moveSpeed = 0.3f;
        public float gravity = 0.9f;
        public float turnSpeed = 1.5f;

        private CameraController _cameraController;


        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _cameraController = GetComponent<CameraController>();
        }

        private void FixedUpdate()
        {
            Vector3 movementVector =
                transform.TransformDirection(new Vector3(_movement.z * -1 * moveSpeed, -gravity,
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

        public void OnLook(InputValue lookVector)
        {
            _cameraController.LookAround(lookVector.Get<Vector2>());
        }
    }
}