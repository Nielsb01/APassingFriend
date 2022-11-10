using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovementScript : MonoBehaviour
{
    [SerializeField] private float _acceleration = 0.8f;
    [SerializeField] private float _deceleration = 1.6f;
    [SerializeField] private float _moveSpeed = 1.75f;
    [SerializeField] private float _jumpSpeed = 4.5f;
    [SerializeField] private float _gravity = 9.81f;

    private CharacterController characterController;

    private float _velocityY = 0.0f;
    private float _velocityX = 0.0f;
    private float _maxPositiveVelocity = 2.0f;
    private float _maxNegativeVelocity = -2.0f;

    private Vector2 _moveVector;
    private Vector3 _moveDirection = Vector3.zero;
    private bool _doJump;

    private float _checkValue = 0.01f;

    void Start()
    {
        _doJump = false;
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (_moveVector == null)
        {
            return;
        }

        if (_doJump)
        {
            _moveDirection.y = _jumpSpeed;
            _doJump = false;
        }

        if (_velocityY < _maxPositiveVelocity && _moveVector.y > 0)
        {
            _velocityY += Time.deltaTime * _acceleration;
        }
        else if (_velocityY > 0)
        {
            _velocityY -= Time.deltaTime * _deceleration;
        }

        if (_velocityY > _maxNegativeVelocity && _moveVector.y < 0)
        {
            _velocityY -= Time.deltaTime * _acceleration;
        }
        else if (_velocityY < 0)
        {
            _velocityY += Time.deltaTime * _deceleration;
            if (floatIsBetween(_velocityY, 0, _checkValue))
            {
                _velocityY = 0;
            }
        }

        if (_velocityX < _maxPositiveVelocity && _moveVector.x > 0)
        {
            _velocityX += Time.deltaTime * _acceleration;
        }
        else if (_velocityX > 0)
        {
            _velocityX -= Time.deltaTime * _deceleration;
        }

        if (_velocityX > _maxNegativeVelocity && _moveVector.x < 0)
        {
            _velocityX -= Time.deltaTime * _acceleration;
        }
        else if (_velocityX < 0)
        {
            _velocityX += Time.deltaTime * _deceleration;
            if (floatIsBetween(_velocityX, 0, _checkValue))
            {
                _velocityX = 0;
            }
        }

        _moveDirection.x = _moveSpeed * ((float)Math.Round(_velocityX, 4));
        _moveDirection.z = _moveSpeed * ((float)Math.Round(_velocityY, 4));

        _moveDirection.y -= _gravity * Time.deltaTime;
        characterController.Move(transform.TransformDirection(_moveDirection * Time.deltaTime));
    }

    private static bool floatIsBetween(float number, float min, float max)
    {
        return number >= min && number <= max;
    }


    private void OnMove(InputValue inputValue)
    {
        _moveVector = inputValue.Get<Vector2>();
    }

    private void OnJump()
    {
        if (characterController.isGrounded)
        {
            _doJump = true;
        }
    }
}
