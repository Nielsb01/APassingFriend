using Npc;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class PickupAbleItem : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Transform _pickUpHandel;
    private Rigidbody _rigidbody;
    public delegate void PickedUpQuestItemEvent(QuestState questState, StyleBackground styleBackground);
    public static event PickedUpQuestItemEvent PickedUpQuestItem;
    [SerializeField] private Texture2D _memory;
    private bool _memoryHasPlayed;
    [SerializeField] private TextAsset _questCompletedText;
    [SerializeField] private Transform _questNpc;

    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Pickup(Transform pickupLocationObject)
    {
        GameObject parrent = null;
        if(transform.parent?.gameObject != null)
        {
            parrent = transform.parent.gameObject;
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
        
        if (_memory != null && !_memoryHasPlayed)
        {
            InvokePickedUpQuestItem(parrent);
        }
    }

    private void InvokePickedUpQuestItem(GameObject parrent)
    {
        PickedUpQuestItem?.Invoke(QuestState.PickedUp, _memory);
        if (name.Equals("Model"))
        {
            Destroy(GetComponent<NpcBallController>());
            if (transform.parent.gameObject != null)
            {
                Destroy(parrent);
            }
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

        if (Vector3.Distance(transform.position, _questNpc.position) <= 2)
        {
            gameObject.SetActive(false);
        }
    }

    public void LoadData(GameData gameData)
    {
        if ((name.Equals("Model") && gameData.questOneState.Equals(QuestState.Completed)) || (name.Equals("Catnip") && gameData.questTwoState.Equals(QuestState.Completed)))
        {
            Destroy(gameObject);
        }
    }

    public void SaveData(ref GameData gameData) {}
}
