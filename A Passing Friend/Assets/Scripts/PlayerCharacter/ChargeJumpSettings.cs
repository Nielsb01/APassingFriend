using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeJumpSettings : ScriptableObject
{
    [SerializeField] private float _chargeSpeed = 1.0f;
    [SerializeField] private float _jumpFailvalue = 90.0f;
    
    public float ChargeSpeed
    {
        get => _chargeSpeed;
        set => _chargeSpeed = value;
    }

    public float JumpFailvalue
    {
        get => _jumpFailvalue;
        set => _jumpFailvalue = value;
    }
}
