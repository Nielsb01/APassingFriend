using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    // UI
    // // Dialog
    private GroupBox _interactBox;

    private VisualElement _dialogBox;

    private GroupBox _dialogBoxDialog;

    private Label _dialogBoxCharName;

    private Label _dialogBoxText;

    private GroupBox _dialogBoxChoices;

    private List<Button> _dialogBoxChoiceButtons = new List<Button>();

    private VisualElement _root;

    // Dialog Builder

    private List<string> _dialogTextList;

    private string _npcName;

    [SerializeField] private DialogBuilder _dialogBuilder;

    private DialogObject chosenDialogOption;

    private int? _currentTextNr = null; // the current number of text in the text list.

    private int? _choiceClicked = null; // the choice of dialog clicked.

    [SerializeField] private CinemachineVirtualCamera _activeCamera;

    private CinemachineVirtualCamera _npcCamera;

    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _interactBox = _root.Q<GroupBox>("interact-box");
        _dialogBox = _root.Q<VisualElement>("dialog-box");
        _dialogBoxDialog = _root.Q<GroupBox>("dialog-box-dialog");
        _dialogBoxCharName = _root.Q<Label>("dialog-box-char-name");
        _dialogBoxText = _root.Q<Label>("dialog-box-text");
        _dialogBoxChoices = _root.Q<GroupBox>("dialog-box-choices");
        _dialogBox.visible = false;
        _interactBox.visible = false;
    }

    private void FixedUpdate()
    {
        // Set the dialog box invisible when the player is not or no longer in the interaction range of a NPC.
        // Currently coded for the dialog system, can be easily adapted for items as well.

        if (!_interactBox.visible)
        {
            SetDialogBoxInvisible();
            ResetDialogue();
            UnsetDialogCamera();

            if (_npcCamera != null && _npcCamera.Priority == (int)Camera.CameraState.Active)
            {
                UnsetNpcCamera();
            }

            return;
        }
    }

    // Set the dialog system visible.
    public void SetDialogSystemVisible()
    {
        _interactBox.visible = true;
    }

    // Set the dialog system invisible.
    public void SetDialogSystemInvisible()
    {
        _interactBox.visible = false;
        SetDialogBoxInvisible();
    }

    // Set the dialog box invisible.
    private void SetDialogBoxInvisible()
    {
        _dialogBox.visible = false;
        _dialogBoxChoices.visible = false;
        _dialogBoxDialog.visible = false;
    }

    // Cycle through the dialog.
    public void ContinueDialog()
    {
        // If the interaction box is not visible (A.K.A. if the player is not in interaction range with a NPC.) do not start or continue dialog.
        if (!_interactBox.visible)
        {
            return;
        }

        /*
        If the dialog box is not visible, set it visible and start the dialog.
        If there is more than one option for dialog, show the choice menu.
        If there is but one option for dialog, skip the choice menu and go straight to dialog.
        */
        if (!_dialogBox.visible)
        {
            _dialogBox.visible = true;

            if (_dialogBuilder.getAllDialogObjects().Count != 1)
            {
                ShowDialogChoices();
            } else
            {
                _choiceClicked = 0;
                SetDialogWithChoice();
                _currentTextNr = -1;
            }

            SetNpcCamera();
        }

        /*
         If there is still dialog left (dialogTextList.Count - 1 because the list works upwards from 0) show next dialog line.
        */

        if (_currentTextNr < (_dialogTextList.Count - 1))
        {
            _currentTextNr++;
            SetDialogBoxCharText(_npcName, _dialogTextList[_currentTextNr ?? default(int)]);
        }
        else
        {
            // If the option ends conversation, it sets the dialog box invisible and resets the dialogue choices and cameras.
            if (chosenDialogOption.doesOptionEndConverstation())
            {
                SetDialogBoxInvisible();
                ResetDialogue();
                UnsetDialogCamera();
                UnsetNpcCamera();
                return;
            }
            else
            {
                // If the option does not end conversation, reset dialogue.
                ResetDialogue();

                // If the option does not end conversation ánd there is more than one dialog option, show choices.
                if (_dialogBuilder.getAllDialogObjects().Count != 1)
                {
                    ShowDialogChoices();
                } else
                {
                    // If the option does not end conversation ánd there is but one dialog option, reset dialogue.
                    _choiceClicked = 0;
                    SetDialogWithChoice();
                    _currentTextNr = -1;
                }
            }
        }
    }

    // Show the dialog choices visual element.
    private void ShowDialogChoices()
    {
        _dialogBox.visible = true;
        _dialogBoxChoices.visible = true;

        var counter = 0;
        foreach (var dialog in _dialogBuilder.getAllDialogObjects())
        {
            _dialogBoxChoiceButtons[counter].text = dialog.getDialogChoice();
            _dialogBoxChoiceButtons[counter].SetEnabled(true);
            _dialogBoxChoiceButtons[counter].clickable.clickedWithEventInfo += ClickedDialogBoxChoiceButton;
            counter++;
        }
        SetDialogBoxCharText(_npcName, _dialogBuilder.getIntroText());
        UnsetDialogCamera();
    }

    // If a dialog choice button is clicked, set the following dialog to that choice.
    private void ClickedDialogBoxChoiceButton(EventBase tab)
    {
        _dialogBoxChoices.visible = false;
        SetDialogBoxCharText(_npcName, _dialogTextList[_currentTextNr ?? default(int)]);
        foreach (var dialogButton in _dialogBoxChoiceButtons)
        {
            dialogButton.SetEnabled(false);
        }
        Button button = tab.target as Button;
        var buttonNr = Regex.Match(button.name, @"\d+").Value;
        _choiceClicked = Convert.ToInt32(buttonNr) - 1;
        SetDialogWithChoice();
        SetDialogCamera();
        _currentTextNr = 0;
    }

    // Reset the entire dialog.
    private void ResetDialogue()
    {
        _choiceClicked = null;
        _currentTextNr = null;
        _dialogBoxText.text = "-";

        foreach (var dialogButton in _dialogBoxChoiceButtons)
        {
            dialogButton.text = "-";
            dialogButton.SetEnabled(false);
        }
    }

    // Set the actual dialog text and character name.
    private void SetDialogBoxCharText(string charName, string text)
    {
        _dialogBoxDialog.visible = true;
        _dialogBoxCharName.text = charName;
        _dialogBoxText.text = text;
    }

    // Set the dialog builder from the NPC.
    public void SetDialogBuilder(DialogBuilder dialogBuilder)
    {
        this._dialogBuilder = dialogBuilder;
        SetDialogWithChoice();
        var counter = 1;
        foreach (var dialog in _dialogBuilder.getAllDialogObjects())
        {
            _dialogBoxChoiceButtons.Add(_root.Q<Button>("dialog-box-choice-button-" + counter));
            counter++;
        }

    }

    // Set the dialog of the choice the player clicked on.
    private void SetDialogWithChoice()
    {
        var dialogObjects = _dialogBuilder.getAllDialogObjects();
        chosenDialogOption = dialogObjects[_choiceClicked ?? default(int)];
        _dialogTextList = chosenDialogOption.getDialog();
        _npcName = _dialogBuilder.getNameOfNpc();
        _npcCamera = _dialogBuilder.getNpcCamera();

    }

    private void SetNpcCamera()
    {
        _npcCamera.Priority = (int)Camera.CameraState.Active;
    }

    private void UnsetNpcCamera()
    {
        _npcCamera.Priority = (int)Camera.CameraState.Inactive;
    }

    private void SetDialogCamera()
    {
        _activeCamera = chosenDialogOption.getDialogCamera();
        if (_activeCamera != null)
        {
            _activeCamera.Priority = (int)Camera.CameraState.Active;
        }
    }

    private void UnsetDialogCamera()
    {
        if (_activeCamera != null)
        {
            _activeCamera.Priority = (int)Camera.CameraState.Inactive;
        }
    }
}
