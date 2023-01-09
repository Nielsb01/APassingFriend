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
    public delegate void PickedUpQuestItemEvent(QuestState questState);
    public static event PickedUpQuestItemEvent PickedUpQuestItem;
    [SerializeField] private bool _isQuestItem = false;
    [SerializeField] private TextAsset _questCompletedText;
    private bool _memoryHasPlayed = false;

    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Pickup(Transform pickupLocationObject)
    {
        if (_isQuestItem && !_memoryHasPlayed)
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
        PickedUpQuestItem?.Invoke(QuestState.PickedUp);
        if (name.Equals("Model"))
        {
            FindObjectOfType<DataPersistenceManager>().NextCheckpoint(4);
            GameObject.Find("DevlinWithHat").GetComponent<DialogBuilder>().LoadDialog(_questCompletedText);
        }
        else if (name.Equals("Catnip"))
        {
            FindObjectOfType<DataPersistenceManager>().NextCheckpoint(7);
            GameObject.Find("Rayen").GetComponent<DialogBuilder>().LoadDialog(_questCompletedText);
        }
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
