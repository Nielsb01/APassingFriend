using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowDad : MonoBehaviour
{
    [SerializeField] private GameObject npc;
    private bool _grounded = false;
    private Rigidbody _rb;
    [SerializeField] private float _bounceStrength = 2;
    [SerializeField] private float _followControllerSpeed = 3;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        TrackControllerGameObject();
        if (_grounded)
        {
            Bounce();
        }
    }

    private void Bounce()
    {
        _rb.AddForce(new Vector3(0,_bounceStrength,0), ForceMode.Impulse);
    }
    
    private void TrackControllerGameObject()
    {
        _rb.AddForce((npc.transform.position - transform.position) * _followControllerSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _grounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        _grounded = false;
    }
}
