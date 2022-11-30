using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    private UIController _ui;

    private const float INTERACT_RANGE = 2.5f;

    private Outline _outline;

    private DialogBuilder _npcDialogBuilder;

    [SerializeField] private LayerMask _npcLayerMask;

    [SerializeField] private FieldOfView playerFov;



    

    private void Start()
    {
        _ui = GameObject.Find("UIDocument").GetComponent<UIController>();
    }

    private void Update()
    {
        if (playerFov.CanSeeTarget)
        {
            _outline = playerFov.TargetRef.transform.GetComponent<Outline>();
            _npcDialogBuilder = playerFov.TargetRef.transform.GetComponent<DialogBuilder>();
        }


        if (_outline != null)
        {
            if (playerFov.CanSeeTarget)
            {
                _outline.enabled = true;
                _ui.SetIsInInteractRange(true);
                _ui.SetInteractBoxVisible();
                _ui.SetDialogBuilder(_npcDialogBuilder);
                _ui.SetIsDialogBuilderSet(true);
            }
            else if (!playerFov.CanSeeTarget)
            {
                _outline.enabled = false;
                _ui.SetIsInInteractRange(false);
                _ui.SetIsDialogBuilderSet(false);
            }
        }
    }

   private void OnInteract()
    {
        if (playerFov.CanSeeTarget)
        {
            _ui.ContinueDialog();
        }

    }
}
