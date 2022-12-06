using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PickupAbleItem : MonoBehaviour
{
    [SerializeField] private Transform pickUpHandel;
    private Rigidbody _rigidbody;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Pickup(Transform pickupLocationObject)
    {
        // If the object has a pickup handle this is used as an offset while picking it up.
        if (pickUpHandel != null)
        {
            transform.rotation = quaternion.identity;
            pickUpHandel.transform.parent = pickupLocationObject;
            transform.parent = pickUpHandel;
            pickUpHandel.position = pickupLocationObject.position;
        }
        else
        {
            transform.parent = pickupLocationObject;
            transform.position = pickupLocationObject.position;
        }
        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = true;
        }
    }

    public void Drop()
    {
        transform.parent = null;
        if (pickUpHandel != null)
        {
            pickUpHandel.transform.parent = transform;
        }

        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = false;
        } 
    }
}
