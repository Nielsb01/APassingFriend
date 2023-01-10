using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float _radius;
    [SerializeField] [Range(0, 360)] private float _angle;
    private float _secondsBetweenChecks = 0.2f;

    [SerializeField] private GameObject _targetRef;

    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstructionMask;

    private bool _canSeeTarget;
    private Transform _pickup;
    private bool _statusCheckedThisUpdate;

    public float Radius
    {
        get { return _radius; }
    }

    public float Angle
    {
        get { return _angle; }
    }

    public GameObject TargetRef
    {
        get
        {
            if (!_statusCheckedThisUpdate)
            {
                FieldOfViewCheck();
            }

            return _targetRef;
        }
    }

    public bool CanSeeTarget
    {
        get
        {
            if (!_statusCheckedThisUpdate)
            {
                FieldOfViewCheck();
            }

            return _canSeeTarget;
        }
    }

    public Transform pickup
    {
        get
        {
            if (!_statusCheckedThisUpdate)
            {
                FieldOfViewCheck();
            }

            return _pickup;
        }
    }

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private void LateUpdate()
    {
        _statusCheckedThisUpdate = false;
    }

    private IEnumerator FOVRoutine()
    {
        var wait = new WaitForSeconds(_secondsBetweenChecks);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        _statusCheckedThisUpdate = true;
        var rangeChecks = Physics.OverlapSphere(transform.position, _radius, _targetMask);

        if (rangeChecks.Length == 0)
        {
            if (_canSeeTarget)
            {
                _canSeeTarget = false;
            }

            if (pickup != null)
            {
                _pickup = null;
            }
        }
        else
        {
            var target = rangeChecks[0].transform;
            _targetRef = target.gameObject;
            var directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < _angle / 2)
            {
                var distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstructionMask))
                {
                    _canSeeTarget = true;
                    if (target.GetComponent<PickupAbleItem>() != null)
                    {
                        _pickup = target;
                    }
                }
                else
                {
                    _canSeeTarget = false;
                    _pickup = null;
                }
            }
            else
            {
                _canSeeTarget = false;
                _pickup = null;
            }
        }
    }
}