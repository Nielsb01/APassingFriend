using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    private UIController _ui;
    [SerializeField] private Outline _outline;

    private DialogBuilder _npcDialogBuilder;

    private CharacterController _characterController;

    [SerializeField] private FieldOfView _playerFov;

    // Item pickup
    private Transform _holdingItem;
    [SerializeField] private Transform _pickUpLocation;

    // HealthController
    [SerializeField] private HealthController _healthController;

    private void Start()
    {
        _ui = GameObject.Find("UIDocument").GetComponent<UIController>();
        _characterController = GetComponent<CharacterController>();
        _healthController = GetComponent<HealthController>();
    }

    private void Update()
    {
        ShowPlayerCanInteract();

        if (_playerFov.pickup == null)
        {
            NpcInteracting();
        }
    }

    private void ShowPlayerCanInteract()
    {
        if (_playerFov.CanSeeTarget && _characterController.isGrounded && !_healthController.IsDead)
        {
            _outline = _playerFov.TargetRef.transform.GetComponent<Outline>();
        }

        if (_outline != null)
        {
            if (_playerFov.CanSeeTarget && _characterController.isGrounded && !_healthController.IsDead)
            {
                _outline.enabled = true;
                _ui.SetIsInInteractRange(true);
                _ui.SetInteractBoxVisible();
            }
            else if (!_playerFov.CanSeeTarget || !_characterController.isGrounded)
            {
                _ui.SetIsInInteractRange(false);
                _outline.enabled = false;
            }
        }
    }

    private void NpcInteracting()
    {
        if (_playerFov.CanSeeTarget && _characterController.isGrounded && !_healthController.IsDead)
        {
            _npcDialogBuilder = _playerFov.TargetRef.transform.GetComponent<DialogBuilder>();
        }

        if (_outline != null)
        {
            if (_playerFov.CanSeeTarget && _characterController.isGrounded && !_healthController.IsDead)
            {
                _ui.SetDialogBuilder(_npcDialogBuilder);
                _ui.SetIsDialogBuilderSet(true);
            }
            else if (!_playerFov.CanSeeTarget || !_characterController.isGrounded)
            {
                _ui.SetIsDialogBuilderSet(false);
            }
        }
    }

    private void PickUpItem()
    {
        if (_holdingItem == null)
        {
            PickupAbleItem pickupAbleItemScript = _playerFov.pickup.GetComponent<PickupAbleItem>();
            CallPickupOnItem(pickupAbleItemScript);
        }
        else
        {
            _holdingItem.GetComponent<PickupAbleItem>().Drop();
            _holdingItem = null;
        }
    }

    private void OnInteract()
    {
        if (_playerFov.pickup != null && _playerFov.CanSeeTarget || _holdingItem != null)
        {
            PickUpItem();
        }
        else if (_playerFov.CanSeeTarget)
        {
            _ui.ContinueDialog();
        }
    }

    public Transform GetItemHolding()
    {
        return _holdingItem;
    }

    public void SetItemHolding(Transform holdingItem)
    {
        _holdingItem = holdingItem;
    }

    public void CallPickupOnItem(PickupAbleItem pickupAbleItemScript)
    {
        if (pickupAbleItemScript != null)
        {
            pickupAbleItemScript.Pickup(_pickUpLocation);
            _holdingItem = _playerFov.pickup;
        }
    }
}