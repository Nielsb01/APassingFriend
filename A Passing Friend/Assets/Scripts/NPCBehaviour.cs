using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    private GameObject npc;

    private UIController ui;

    private bool playerInInteractRange;

    private float interactRange = 2.5f;

    private Outline outline;

    public Camera mainCam;

    public LayerMask whatIsPlayer;


    private void Start()
    {
        npc = GetComponent<GameObject>();
        outline = GetComponent<Outline>();
        ui = GameObject.Find("UIDocument").GetComponent<UIController>();
    }

    private void Update()
    {
        /* Instead of this method, you can also use the RayCast in the PlayerController script to check if the player is in interact range.
           Preferably only use 1 method, but both also works. */
        playerInInteractRange = Physics.CheckSphere(transform.position, interactRange, whatIsPlayer);

        if (playerInInteractRange && !outline.enabled)
        {
            outline.enabled = true;
            ui.SetInteractButtonVisibility();
        } 
        else if (!playerInInteractRange && outline.enabled)
        {
            outline.enabled = false;
            ui.SetInteractButtonVisibility();
        }
    }
}
