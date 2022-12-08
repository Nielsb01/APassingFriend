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

    [SerializeField] private float _velocityY;
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
        Move();
        Rotate();
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
        _playerAnimator.SetFloat(Y_VELOCITY_ANIMATOR_VARIABLE, _velocityY);
    }

    private static bool floatIsBetween(float number, float min, float max)
    {
        return number >= min && number <= max;
    }

    public void FreezeMovement(bool movementImpaired, bool rotationFrozen)
    {
        _movementImpaired = movementImpaired;
        _rotationFrozenDueToDialog = rotationFrozen;

        if (movementImpaired)
        {
            _velocityY = 0;
            _playerAnimator.SetFloat(Y_VELOCITY_ANIMATOR_VARIABLE, _velocityY);
        }
    }

    private void resetJumpCharge()
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
            _moveVector = Vector3.zero;
            _velocityY = 0;
            _velocityX = 0;
            _moveDirection.x = 0;
            _moveDirection.z = 0;
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
        else  if (_characterController.isGrounded)
        {
            _doJump = true;
        }
        resetJumpCharge();
    }

    private void OnJumpHold()
    {
        if (_chargeJumpUnlocked && _characterController.isGrounded)
        { 
            _holdingDownJump = true;
        }
    }
    
    private void OnJumpFail()
    {
        _moveDirection.y = _failjumpSpeed;
        _velocityY += _failjumpSpeed;
    }

    // Getters for making UI
    public float getJumpCharged()
    {
        return _jumpCharged;
    }

    public float getOverchargeLevel()
    {
        return _jumpOverchargeValue;
    }

    public float GetMinimumChargeJumpValue()
    {   
        return _MinimumChargeJumpValue;
    }

    public bool isInChargeZone()
    {
        return _chargeJumpUnlocked;
    }
}