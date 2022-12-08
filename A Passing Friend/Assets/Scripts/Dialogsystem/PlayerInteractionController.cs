using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    private UIController _ui;

    private Outline _outline;

    private DialogBuilder _npcDialogBuilder;

    [SerializeField] private FieldOfView _playerFov;

    private void Start()
    {
        _ui = GameObject.Find("UIDocument").GetComponent<UIController>();
    }

    private void Update()
    {
        if (_playerFov.CanSeeTarget)
        {
            _outline = _playerFov.TargetRef.transform.GetComponent<Outline>();
            _npcDialogBuilder = _playerFov.TargetRef.transform.GetComponent<DialogBuilder>();
        }


        if (_outline != null)
        {
            if (_playerFov.CanSeeTarget)
            {
                _outline.enabled = true;
                _ui.SetIsInInteractRange(true);
                _ui.SetInteractBoxVisible();
                _ui.SetDialogBuilder(_npcDialogBuilder);
                _ui.SetIsDialogBuilderSet(true);
            }
            else if (!_playerFov.CanSeeTarget)
            {
                _outline.enabled = false;
                _ui.SetIsInInteractRange(false);
                _ui.SetIsDialogBuilderSet(false);
            }
        }
    }

    private void OnInteract()
    {
        if (_playerFov.CanSeeTarget)
        {
            _ui.ContinueDialog();
        }

    }
}
