using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class PickupAbleItem : MonoBehaviour
{
    [SerializeField] private Transform _pickUpHandel;
    private Rigidbody _rigidbody;
    public delegate void PickedUpQuestItemEvent(QuestState questState, StyleBackground styleBackground);
    public static event PickedUpQuestItemEvent PickedUpQuestItem;
    [SerializeField] private Texture2D _memory;
    private bool _memoryHasPlayed;

    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Pickup(Transform pickupLocationObject)
    {
        if (_memory != null && !_memoryHasPlayed)
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
        PickedUpQuestItem?.Invoke(QuestState.PickedUp, _memory);
        _memoryHasPlayed = true;
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
