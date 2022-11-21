using System;
using System.Collections;
using System.Collections.Generic;
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

    private const float CHECK_VALUE = 0.1f;

    //Charge jumping
    private List<MovementModule> movementModules;
    [SerializeField] private bool _isInChargeJumpZone;

    //TODO Alleen voor testen haar serialize hier weg.
    [SerializeField] private float _jumpCharged = 0.0f;
    private bool _holdingDownJump = false;
    [SerializeField] private float _chargeSpeed = 1.0f;
    [SerializeField] private float _jumpFailvalue = 90.0f;

    void Start()
    {
        foreach (var module in movementModules)
        {
            module.setUpModule();
        }
        _doJump = false;
        _characterController = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        Move();
        Rotate();
        foreach (var module in movementModules)
        {
            module.RunModule();
        }
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
        if (_moveVector == null)
        {
            return;
        }

        if (_doJump)
        {
            _moveDirection.y = _jumpSpeed;
            _doJump = false;
        }
        else if (_characterController.isGrounded && !_isInChargeJumpZone)
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

    private IEnumerator chargeJump()
    {
        while (_holdingDownJump)
        {
            _jumpCharged += _chargeSpeed * Time.deltaTime;
            yield return (null);
        }

        StopCoroutine("chargeJump");
    }

    private void OnMove(InputValue inputValue)
    {
        _moveVector = inputValue.Get<Vector2>();
    }

    private void OnJump()
    {
        if (_characterController.isGrounded)
        {
            if (_isInChargeJumpZone)
            {
                _holdingDownJump = true;
                StartCoroutine("chargeJump");
            }
            else
            {
                _doJump = true;
            }
        }
    }

    private void OnJumpRelease()
    {
        if (_isInChargeJumpZone)
        {
            print("jumpReleased");
            if (_jumpCharged > _jumpFailvalue)
            {
                OnJumpFail();
            }
            else
            {
                _moveDirection.y = _jumpCharged;
            }
            _holdingDownJump = false;
            _jumpCharged = 0;
        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        print("test");
        if (trigger.transform.tag == "ChargeJumpZone")
        {
            _isInChargeJumpZone = true;
        }
    }

    void OnTriggerExit(Collider trigger)
    {
        if (trigger.transform.tag == "ChargeJumpZone")
        {
            _isInChargeJumpZone = false;
        }
    }

    private void OnJumpFail()
    {
        print("Jump failed :(");
    }

    private float getJumpCharged()
    {
        return _jumpCharged;
    }
}