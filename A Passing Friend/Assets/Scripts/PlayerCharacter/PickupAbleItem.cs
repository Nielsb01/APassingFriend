using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class PickupAbleItem : MonoBehaviour
{
    [SerializeField] private Transform _pickUpHandel;
    private Rigidbody _rigidbody;

    // Questing
    public delegate void PickedUpQuestItemEvent(QuestState questState, int memoryNr);
    public static event PickedUpQuestItemEvent PickedUpQuestItem;
    [SerializeField] private bool _isQuestItem = false;
    [SerializeField] private int memoryNr = 0;

    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Pickup(Transform pickupLocationObject)
    {
        if (_isQuestItem)
        {
            InvokePickedUpQuestItem();
        }

        // If the object has a pickup handle this is used as an offset while picking it up.
        if (_pickUpHandel != null)
        {
            transform.rotation = quaternion.identity;
            _pickUpHandel.transform.parent = pickupLocationObject;
            transform.parent = _pickUpHandel;
            _pickUpHandel.position = pickupLocationObject.position;
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

        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    private void InvokePickedUpQuestItem()
    {
        PickedUpQuestItem?.Invoke(QuestState.PickedUp, memoryNr);
    }

    public void Drop()
    {
        transform.parent = null;
        if (_pickUpHandel != null)
        {
            _pickUpHandel.transform.parent = transform;
        }

        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = false;
        }

        gameObject.layer = LayerMask.NameToLayer("Pickup");
    }
}
