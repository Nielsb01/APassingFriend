using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    private GameObject npc;

    private bool playerInInteractRange;

    private float interactRange = 2.5f;

    private Outline outline;

    public Camera mainCam;

    public LayerMask whatIsPlayer;


    private void Start()
    {
        npc = GetComponent<GameObject>();
        outline = GetComponent<Outline>();
    }

    private void Update()
    {
        playerInInteractRange = Physics.CheckSphere(transform.position, interactRange, whatIsPlayer);

        if (playerInInteractRange)
        {
            outline.enabled = true;
        } 
        else if (outline.enabled)
        {
            outline.enabled = false;
        }
    }
}
