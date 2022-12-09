#region

using System;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

public class CharacterMovementScript : MonoBehaviour, IDataPersistence
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
    private bool _rotationFrozenDueToDialog;
    [HideInInspector]
    public bool rotationFrozenDueToSpecialArea;

    [SerializeField] private bool _movementImpaired;

    private const float CHECK_VALUE = 0.1f;


    //Charge jumping
    [SerializeField] private float _chargeSpeed = 1.0f;
    [SerializeField] private float _jumpOverchargeValue = 90.0f;
    [SerializeField] private float _failjumpSpeed;
    [SerializeField] private bool _chargeJumpUnlocked;
    [SerializeField] private float _jumpCharged;
    [SerializeField] private float _MinimumChargeJumpValue = 0.3f;
    private bool _doChargeJump;
    private bool _holdingDownJump;

    // Climbing
    [SerializeField] private  float _climbZoneExitJumpSpeed = 0.1f;
    [SerializeField] private bool _inClimbingZone;
    private bool _canClimb = true;
    [SerializeField] private bool _isClimbing; 
    private bool _exitingClimbing;
    private static string CLIMBING_ZONE_TAG = "ClimbingZone";
    private static float CLIMBING_DISTANCE = 0.3f;
    
    //Animation
    [SerializeField] private Animator _playerAnimator;
    private static string Y_VELOCITY_ANIMATOR_VARIABLE = "velocityY";

    private void Awake()
    {
        _doJump = false;
        _movementImpaired = false;
        _characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (_inClimbingZone)
        {
            CheckCanClimb();

            if (_canClimb)
            {
                ClimbWall();
            }
        }

        if (_isClimbing)
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
        if (_movementImpaired) return;

        _rotationFrozenDueToFreeLook = value.isPressed;
    }

    private void OnLook(InputValue inputValue)
    {
        if (_movementImpaired) return;

        var inputVector = inputValue.Get<Vector2>();
        _rotation = Vector3.up * inputVector.x;
    }

    private void Rotate()
    {
        if (_rotationFrozenDueToFreeLook || rotationFrozenDueToSpecialArea || _rotationFrozenDueToDialog) return;
        transform.Rotate(_rotation * _rotationSpeed);
    }

    private void Update()
    {
        if (_movementImpaired) return;

        if (_holdingDownJump)
        {
            _jumpCharged += _chargeSpeed * Time.deltaTime;
        }
        if (!_characterController.isGrounded && _jumpCharged > 0)
        {
            _jumpCharged = 0;
        }
    }

    private void Move()
    {
        if (_movementImpaired) return;

        if (_moveVector == null)
        {
            return;
        }

        if (_doJump)
        {
            if (!_doChargeJump)
            {
                _moveDirection.y = _jumpSpeed;
            }

            _doJump = false;
            _doChargeJump = false;
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
            if (FloatIsBetween(_velocityY, 0, CHECK_VALUE))
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
            if (FloatIsBetween(_velocityX, 0, CHECK_VALUE))
            {
                _velocityX = 0;
            }
        }

        _moveDirection.x = _moveSpeed * (float)Math.Round(_velocityX, 4);
        _moveDirection.z = _moveSpeed * (float)Math.Round(_velocityY, 4);
        _moveDirection.y -= _gravity * Time.deltaTime;
        _characterController.Move(transform.TransformDirection(_moveDirection * Time.deltaTime));
        _playerAnimator.SetFloat(Y_VELOCITY_ANIMATOR_VARIABLE, _velocityY);
    }

    private static bool FloatIsBetween(float number, float min, float max)
    {
        return number >= min && number <= max;
    }

    public void FreezeMovement(bool movementImpaired, bool rotationFrozen)
    {
        _movementImpaired = movementImpaired;
        _rotationFrozenDueToDialog = rotationFrozen;

        if (_movementImpaired || _rotationFrozenDueToDialog)
        {
            ResetJumpCharge();
            _doChargeJump = false;
            _doJump = false;
            _moveVector = Vector3.zero;
            _moveDirection.y = 0;
            _moveDirection.x = 0;
            _moveDirection.z = 0;
            _velocityY = 0;
            _velocityX = 0;
            _playerAnimator.SetFloat(Y_VELOCITY_ANIMATOR_VARIABLE, _velocityY);
        }
    }

    private void ResetJumpCharge()
    {
        _holdingDownJump = false;
        _jumpCharged = 0;
    }

    private void OnMove(InputValue inputValue)
    {
        if (!_movementImpaired)
        {
            _moveVector = inputValue.Get<Vector2>();
        }
        else
        {
            ResetJumpCharge();
            _doChargeJump = false;
            _doJump = false;
            _moveVector = Vector3.zero;
            _moveDirection.y = 0;
            _moveDirection.x = 0;
            _moveDirection.z = 0;
            _velocityY = 0;
            _velocityX = 0;
        }
    }

    public void LoadData(GameData data)
    {
        _characterController.enabled = false;
        this.transform.position = data.PlayerLocation;
        _characterController.enabled = true;
    }

    public void SaveData(ref GameData data)
    {
        data.PlayerLocation = this.transform.position;
    }

    private void OnJumpRelease()
    {
        if (_movementImpaired) return;

        if (_chargeJumpUnlocked && _jumpCharged > _MinimumChargeJumpValue)
        {
            if (_characterController.isGrounded)
            {
                // Minimum charge value determines how long the jump key should be held down, we want to subtract this from the charge so everything before that
                // threshold wont matter for the jump
                _jumpCharged -= _MinimumChargeJumpValue;
                if (_jumpCharged > _jumpOverchargeValue)
                {
                    OnJumpFail();
                }
                else
                {
                    _moveDirection.y = _jumpCharged;
                    _doJump = true;
                    _doChargeJump = true;
                }
            }

        }
        else if (_characterController.isGrounded)
        {
            _doJump = true;
        }

        if (_isClimbing)
        {
            _canClimb = false;
            _isClimbing = false;
        }
        ResetJumpCharge();
    }
    
    private void OnJumpHold()
    {
        if (_movementImpaired) return;

        if (_chargeJumpUnlocked && _characterController.isGrounded)
        {
            _holdingDownJump = true;
        }
    }
    
    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.transform.CompareTag(CLIMBING_ZONE_TAG))
        {
            _inClimbingZone = true;
        }
    }
    
    private void OnTriggerExit(Collider trigger)
    {
        if (trigger.transform.CompareTag(CLIMBING_ZONE_TAG))
        {
            _isClimbing = false;
            _inClimbingZone = false;
        }
    }

    private void OnJumpFail()
    {
        _moveDirection.y = _failjumpSpeed;
        _velocityY += _failjumpSpeed;
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
            _exitingClimbing = true;
            _isClimbing = false;
        }
    }
    private void ClimbWall()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward,out hit,CLIMBING_DISTANCE))
        {
            if (hit.transform.CompareTag("ClimbingWall"))
            {
                transform.forward = Vector3.Lerp(transform.forward,
                    -hit.normal,
                    10f * Time.fixedDeltaTime);
                if (!_exitingClimbing)
                {
                    _isClimbing = true;
                }
            }
            else if (_isClimbing)
            {
                // if the player isn't looking at the wall anymore exit climbing
                {
                    ExitClimbing();
                }
            }
        }
        else
        {
            if (_exitingClimbing)
            {
                _exitingClimbing = false;
            }
            else if(_isClimbing)
            {
                _isClimbing = false;
            }
        }
    }
    // This jumped is performed when failing a jump, but also when exiting a climb zone.
    private void PerformSmallJump(float jumpPower)
    {
        _moveDirection.y = jumpPower;
        _velocityY += jumpPower;
    }

    private void ExitClimbing()
    {
        _isClimbing = false;
        _velocityX = 0;
        _velocityY = 0;
        PerformSmallJump(_climbZoneExitJumpSpeed);
        _exitingClimbing = false;
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

    public float GetMinimumChargeJumpValue()
    {
        return _MinimumChargeJumpValue;
    }

    public bool IsInChargeZone()
    {
        return _chargeJumpUnlocked;
    }
}