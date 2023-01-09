#region

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

public class CharacterMovementScript : MonoBehaviour, IDataPersistence
{
    [Header("Movement Settings")]
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

    private const float CHECK_VALUE = 0.1f;

    private bool _movementImpaired;

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
    private bool _inClimbingZone;
    private bool _canClimb;
    private bool _isClimbing; 
    private bool _exitingClimbing;
    private static string CLIMBING_ZONE_TAG = "ClimbingZone";
    private static string CLIMBING_WALL_TAG = "ClimbingWall";
    private static float CLIMBING_DISTANCE = 0.3f;
    
    //Animation
    [SerializeField] private Animator _playerAnimator;
    private static string Y_VELOCITY_ANIMATOR_VARIABLE = "velocityY";

    //Interacting
    private PlayerInteractionController _playerInteractionController;

    [Header("Sound Settings")]
    [SerializeField] private FMODUnity.EventReference _footstepsEventPath;
    [SerializeField] private FMODUnity.EventReference _jumpingEventPath;
    [SerializeField] private FMODUnity.EventReference _landingEventPath;
    [SerializeField] private float _minimumDisplacementForSound;
    private Vector3 _prevSoundPosition;
    private float _jumpThresholdSeconds = 1;

    private void Awake()
    {
        _doJump = false;
        _movementImpaired = false;
        _characterController = GetComponent<CharacterController>();
        _playerInteractionController = GetComponent<PlayerInteractionController>();
    }

    private void Start()
    {
        _canClimb = true;
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
            HandleMovementSound();
        }
        SetAnimatorVariables();
    }

    public void OnFreeLook(InputValue value)
    {
        if (_movementImpaired) return;

        _rotationFrozenDueToFreeLook = value.isPressed;
    }

    private void OnLook(InputValue inputValue)
    {
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
            ResetAllMovement();
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
            ResetAllMovement();
        }
    }

    private void ResetAllMovement()
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

    public void LoadData(GameData data)
    {
        _characterController.enabled = false;
        transform.position = data.playerLocation;
        _characterController.enabled = true;
        ResetAllMovement();
        loadHoldingItem(data);
        _chargeJumpUnlocked = data.canChargeJump;
    }

    public void SaveData(ref GameData data)
    {
    }
    
    private void loadHoldingItem(GameData data)
    {
        if (data.ItemHeldByPlayer != null && !data.ItemHeldByPlayer.Equals(String.Empty))
        {
            Transform itemHeldByPlayer = GameObject.Find(data.ItemHeldByPlayer).transform;
            _playerInteractionController.CallPickupOnItem(itemHeldByPlayer.GetComponent<PickupAbleItem>());
            _playerInteractionController.SetItemHolding(itemHeldByPlayer);
        }
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
            _playerAnimator.SetBool("Charge",false);
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

        // Only jump when cat is not dead
        if (_doJump && isActiveAndEnabled)
        {
            // Play jump sound
            StartCoroutine(OnJumpStart());
        }

        ResetJumpCharge();
    }
    
    private void OnJumpHold()
    {
        if (_movementImpaired) return;
        if (_chargeJumpUnlocked && _characterController.isGrounded)
        {
            _playerAnimator.SetBool("Charge",true);
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
        _playerAnimator.SetTrigger("Fall");
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
            if (hit.transform.CompareTag(CLIMBING_WALL_TAG))
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

    private void SetAnimatorVariables()
    {
        _playerAnimator.SetFloat(Y_VELOCITY_ANIMATOR_VARIABLE, _velocityY);
        _playerAnimator.SetFloat("velocityZ",_moveDirection.x);
        _playerAnimator.SetFloat("velocityX",_moveDirection.y);
        _playerAnimator.SetBool("Grounded",_characterController.isGrounded);
        _playerAnimator.SetBool("Climbing",_isClimbing);
        _playerAnimator.SetFloat("ClimbingSpeed",_moveVector.y);
    }

    // Methods for handling sound
    private IEnumerator OnJumpStart()
    {
        var audioEvent = FMODUnity.RuntimeManager.CreateInstance(_jumpingEventPath);
        audioEvent.start();

        // Wait 100 milliseconds for waiting for jump start
        const float delay = 0.1f;
        yield return new WaitForSeconds(delay);
        while (_characterController.isGrounded == false)
        {
            // wait 100 milliseconds between checks
            yield return new WaitForSeconds(_jumpThresholdSeconds);
        }

        OnJumpLand();
        yield return null;
    }

    private void OnJumpLand()
    {
        var audioEvent = FMODUnity.RuntimeManager.CreateInstance(_landingEventPath);
        audioEvent.start();
    }

    private void HandleMovementSound()
    {
        // if player moved
        if (Vector3.Distance(_prevSoundPosition, transform.position) > _minimumDisplacementForSound)
        {
            FMODUnity.RuntimeManager.PlayOneShot(_footstepsEventPath);
           _prevSoundPosition = transform.position;
        }
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

    public bool GetHoldingDownJump()
    {
        return _holdingDownJump;
    }

    public bool IsChargeJumpUnlocked()
    {
        return _chargeJumpUnlocked;
    }
}