using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeJumpTest : MonoBehaviour, MovementModule
{
    [SerializeField]
    private bool _isInChargeJumpZone;
    
    private CharacterController _characterController;
    //TODO Alleen voor testen haar serialize hier weg.
    [SerializeField]
    private float _jumpCharged = 0.0f;
    
    [SerializeField]
    private float _chargeSpeed = 1.0f;

    public void setUpModule()
    {
    }

    public void RunModule()
    {
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
    private void OnJump()
    {
        if (_characterController.isGrounded && _isInChargeJumpZone)
        {
                _jumpCharged += _chargeSpeed * Time.deltaTime;
        }
    }
}
