using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float _radius;
    [SerializeField] [Range(0, 360)] private float _angle;

    [SerializeField] private GameObject _targetRef;

    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstructionMask;

    [SerializeField] private bool _canSeeTarget;

    public float Radius
    {
        get
        {
            return _radius;
        }
    }

    public float Angle
    {
        get
        {
            return _angle;
        }
    }

    public GameObject TargetRef
    {
        get
        {
            return _targetRef;
        }
    }

    public bool CanSeeTarget
    {
        get
        {
            return _canSeeTarget;
        }
    }

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, _radius, _targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            _targetRef = target.gameObject;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < _angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstructionMask))
                {
                    _canSeeTarget = true;
                }
                else
                {
                    _canSeeTarget = false;
                }
            }
            else
            {
                _canSeeTarget = false;
            }
        }
        else if (_canSeeTarget)
        {
            _canSeeTarget = false;
        }
    }
}
