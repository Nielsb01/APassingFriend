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
    [SerializeField] private float _rotationSpeed = 0.3f;

    private CharacterController _characterController;

    private float _velocityY = 0.0f;
    private float _velocityX = 0.0f;
    private float _maxPositiveVelocity = 2.0f;
    private float _maxNegativeVelocity = -2.0f;

    private Vector2 _moveVector;
    private Vector2 _rotation;
    private Vector3 _moveDirection = Vector3.zero;
    private bool _doJump;
    private bool _rotationFrozen;

    [SerializeField] private bool _movementImpaired;

    private const float CHECK_VALUE = 0.1f;

    void Start()
    {
        _doJump = false;
        _movementImpaired = false;
        _characterController = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        Move();
        Rotate();
    }

    public void OnFreeLook(InputValue value)
    {
        _rotationFrozen = value.isPressed;
    }

    void OnLook(InputValue inputValue)
    {
        var inputVector = inputValue.Get<Vector2>();
        _rotation = Vector3.up * inputVector.x;
    }

    private void Rotate()
    {
        if (_rotationFrozen) return;
        transform.Rotate(_rotation * _rotationSpeed);
    }

    private void Move()
    {
        if (!_movementImpaired)
        {
            return;
        }

        if (_moveVector == null)
        {
            return;
        }

        if (_doJump)
        {
            _moveDirection.y = _jumpSpeed;
            _doJump = false;
        }
        else if (_characterController.isGrounded)
        {
            _moveDirection.y = 0;
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
            if (floatIsBetween(_velocityY, 0, CHECK_VALUE))
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
            if (floatIsBetween(_velocityX, 0, CHECK_VALUE))
            {
                _velocityX = 0;
            }
        }

        _moveDirection.x = _moveSpeed * ((float)Math.Round(_velocityX, 4));
        _moveDirection.z = _moveSpeed * ((float)Math.Round(_velocityY, 4));
        _moveDirection.y -= _gravity * Time.deltaTime;
        _characterController.Move(transform.TransformDirection(_moveDirection * Time.deltaTime));
    }

    private static bool floatIsBetween(float number, float min, float max)
    {
        return number >= min && number <= max;
    }

    public void FreezeMovement(bool movementImpaired, bool rotationFrozen)
    {
        _movementImpaired = movementImpaired;
        _rotationFrozen = rotationFrozen;
    }


    private void OnMove(InputValue inputValue)
    {
        if (!_movementImpaired)
        {
            _moveVector = inputValue.Get<Vector2>();
        } else
        {
            _moveVector = Vector3.zero;
            _moveDirection.x = 0;
            _moveDirection.z = 0;
        }

    }

    private void OnJump()
    {
        if (_characterController.isGrounded && !_movementImpaired)
        {
            _doJump = true;
        }
    }
}