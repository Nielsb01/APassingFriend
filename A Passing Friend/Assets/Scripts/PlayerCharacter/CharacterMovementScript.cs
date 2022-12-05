#region

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

#endregion

public class CharacterMovementScript : MonoBehaviour
{
    [SerializeField] private float _acceleration = 0.8f;
    [SerializeField] private float _deceleration = 1.6f;
    [SerializeField] private float _moveSpeed = 1.75f;
    [SerializeField] private float _jumpSpeed = 4.5f;
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private float _rotationSpeed = 0.3f;

    private CharacterController _characterController;

    private float _velocityY;
    private float _velocityX;
    private readonly float _maxPositiveVelocity = 2.0f;
    private readonly float _maxNegativeVelocity = -2.0f;

    private Vector2 _moveVector;
    private Vector2 _rotation;
    private Vector3 _moveDirection = Vector3.zero;
    private bool _doJump;
    private bool _rotationFrozenDueToFreeLook;
    public bool rotationFrozenDueToSpecialArea;

    private const float CHECK_VALUE = 0.1f;

    //Charge jumping
    [SerializeField] private float _chargeSpeed = 1.0f;
    [SerializeField] private float _jumpOverchargeValue = 90.0f;
    [SerializeField] private float _failjumpSpeed;

    private bool _isInChargeJumpZone;
    private bool _holdingDownJump;
    private float _jumpCharged;

    // Climbing
    private bool _inClimbingZone;
    private static float CLIMB_ZONE_EXIT_JUMP_SPEED = 0.1f;
    private static bool _canClimb = true;
    private bool _isClimbing;
    
    //Animation
    [SerializeField] private Animator _playerAnimator;
    private static string Y_VELOCITY_ANIMATOR_VARIABLE = "velocityY";
    
    private void Start()
    {
        _doJump = false;
        _characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (_inClimbingZone)
        {
            MakePlayerFaceWall();
            CheckCanClimb();
        }
        if (_isClimbing && _canClimb)
        {
            Climb();
        }
        else
        {
            Move();
            Rotate();
        }
        
        
        
    }

    public void OnFreeLook(InputValue value)
    {
        _rotationFrozenDueToFreeLook = value.isPressed;
    }

    private void OnLook(InputValue inputValue)
    {
        var inputVector = inputValue.Get<Vector2>();
        _rotation = Vector3.up * inputVector.x;
    }

    private void Rotate()
    {
        if (_rotationFrozenDueToFreeLook || rotationFrozenDueToSpecialArea) return;
        transform.Rotate(_rotation * _rotationSpeed);
    }

    private void Update()
    {
        if (_holdingDownJump)
        {
            _jumpCharged += _chargeSpeed * Time.deltaTime;
        }
    }

    private void Move()
    {
        if (_moveVector == null)
        {
            return;
        }

        if (_doJump)
        {
            if (!_isInChargeJumpZone)
            {
                _moveDirection.y = _jumpSpeed;
            }

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

        _moveDirection.x = _moveSpeed * (float)Math.Round(_velocityX, 4);
        _moveDirection.z = _moveSpeed * (float)Math.Round(_velocityY, 4);
        _moveDirection.y -= _gravity * Time.deltaTime;
        _characterController.Move(transform.TransformDirection(_moveDirection * Time.deltaTime));
        _playerAnimator.SetFloat(Y_VELOCITY_ANIMATOR_VARIABLE,_velocityY);
    }

    private static bool floatIsBetween(float number, float min, float max)
    {
        return number >= min && number <= max;
    }

    private void resetJumpCharge()
    {
        _holdingDownJump = false;
        _jumpCharged = 0;
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
            }
            else
            {
                _doJump = true;
            }
        }

        if (_isClimbing)
        {
            _canClimb = false;
        }
    }

    private void OnJumpRelease()
    {
        if (_isInChargeJumpZone)
        {
            if (_jumpCharged > _jumpOverchargeValue)
            {
                OnJumpFail();
            }
            else
            {
                _moveDirection.y = _jumpCharged;
                _doJump = true;
            }

            resetJumpCharge();
        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.transform.CompareTag("ChargeJumpZone"))
        {
            _isInChargeJumpZone = true;
        }
        if (trigger.transform.CompareTag("ClimbingZone"))
        {
            _inClimbingZone = true;
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (trigger.transform.CompareTag("ChargeJumpZone"))
        {
            _isInChargeJumpZone = false;
            resetJumpCharge();
        }
        if (trigger.transform.CompareTag("ClimbingZone"))
        {
            _isClimbing = false;
            _inClimbingZone = false;
        }
        
    }
    private void OnJumpFail()
    {
        // TODO implement funny cat animations
        Debug.Log("Jump failed :(");
        PreformSmallJump(_failjumpSpeed);
        _doJump = true;
    }

    private void CheckCanClimb()
    {
        if (_canClimb) return;
        if (_characterController.isGrounded)
        {
            _canClimb = true;
            //Resetting movement direction so the cat won't get slammed into the floor 
            _moveDirection.y = 0;
        }
    }

    private void Climb()
    {
        Vector3 climbingMovementDirection = new Vector3(_moveVector.x, _moveVector.y, 0);
        _characterController.Move(transform.TransformDirection(climbingMovementDirection * Time.deltaTime));
        // If the character is grounded and we press backward, we want to call the normal move to exit the climb zone.
        if (_characterController.isGrounded && _moveVector.y <= -1)
        {
            Move();
        }
    }
    private void MakePlayerFaceWall()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward,out hit,0.3f))
        {
            if (hit.transform.CompareTag("ClimbingWall"))
            {
                _isClimbing = true;
            }
            else if (_isClimbing)
            {
                // if the player isn't looking at the wall anymore exit climbing
                {
                    _isClimbing = false;
                    PreformSmallJump(CLIMB_ZONE_EXIT_JUMP_SPEED);
                }
            }
        }
        else if (_isClimbing)
        {
            {
                _isClimbing = false;
                PreformSmallJump(CLIMB_ZONE_EXIT_JUMP_SPEED);
            }
        }
    }
    // This jumped is preformed when failing a jump, but also when exiting a climb zone.
    private void PreformSmallJump(float jumpPower)
    {
        _moveDirection.y = jumpPower;
        _velocityY += jumpPower;
    }

    // Getters for making UI
    public float GetJumpCharged()
    {
        return _jumpCharged;
    }

    public float GetOverchargeLevel()
    {
        return _jumpOverchargeValue;
    }

    public bool IsInChargeZone()
    {
        return _isInChargeJumpZone;
    }
}