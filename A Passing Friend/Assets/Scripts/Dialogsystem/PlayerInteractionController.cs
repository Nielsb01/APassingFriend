using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    private UIController _ui;

    private Outline _outline;

    private DialogBuilder _npcDialogBuilder;

    private CharacterController _characterController;

    [SerializeField] private FieldOfView _playerFov;
    
    // Item pickup
    private Transform _holdingItem;
    [SerializeField] private Transform _pickUpLocation;

    private void Start()
    {
        _ui = GameObject.Find("UIDocument").GetComponent<UIController>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
     NpcInteracting();
    }

    private void NpcInteracting()
    {
        if (_playerFov.CanSeeTarget && _characterController && _playerFov.pickup)
        {
            _outline = _playerFov.TargetRef.transform.GetComponent<Outline>();
            _npcDialogBuilder = _playerFov.TargetRef.transform.GetComponent<DialogBuilder>();
        }


        if (_outline != null)
        {
            if (_playerFov.CanSeeTarget && _characterController.isGrounded)
            {
                _outline.enabled = true;
                _ui.SetIsInInteractRange(true);
                _ui.SetInteractBoxVisible();
                _ui.SetDialogBuilder(_npcDialogBuilder);
                _ui.SetIsDialogBuilderSet(true);
            }
            else if (!_playerFov.CanSeeTarget || !_characterController.isGrounded)
            {
                _outline.enabled = false;
                _ui.SetIsInInteractRange(false);
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
