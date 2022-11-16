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
    private VisualElement _interactBox;

    private VisualElement _dialogBox;

    private GroupBox _dialogBoxDialog;

    private Label _dialogBoxCharName;

    private Label _dialogBoxText;

    private GroupBox _dialogBoxChoices;

    private List<Button> _dialogBoxChoiceButtons = new List<Button>();

    private VisualElement _root;
    
    [SerializeField] private CinemachineVirtualCamera _activeCamera;



    // Dialog Builder

    private List<string> _dialogTextList;
    
    [SerializeField] private DialogBuilder _dialogBuilder;
    
    private DialogObject chosenDialogOption;

    private int? _currentTextNr = null;

    private int? _choiceClicked = null;

    private void Start()
    { 
        _root = GetComponent<UIDocument>().rootVisualElement;
        _interactBox = _root.Q<VisualElement>("interact-box");
        _dialogBox = _root.Q<VisualElement>("dialog-box");
        _dialogBoxDialog = _root.Q<GroupBox>("dialog-box-dialog");
        _dialogBoxCharName = _root.Q<Label>("dialog-box-char-name");
        _dialogBoxText = _root.Q<Label>("dialog-box-text");
        _dialogBoxChoices = _root.Q<GroupBox>("dialog-box-choices");

        _dialogBox.visible = false;
        _interactBox.visible = false;
    }

    // Setters
    public void SetDialogSystemVisible()
    {
        _interactBox.visible = true;
    }

    public void SetDialogSystemInvisible()
    {
        _interactBox.visible = false;
        SetDialogBoxInvisible();
    }

    private void SetDialogBoxInvisible()
    {
        _dialogBox.visible = false;
        _dialogBoxChoices.visible = false;
        _dialogBoxDialog.visible = false;
    }

    // UI Dialog System
    public void ContinueDialog()
    {
        if (!_interactBox.visible)
        {
            SetDialogBoxInvisible();
            ResetTextAndChoice();
            return;
        }

        if (!_dialogBox.visible)
        {
            _dialogBox.visible = true;
            ShowDialogChoices();           
        }

        if (_currentTextNr < (_dialogTextList.Count - 1))
        {
            _currentTextNr++;
            SetDialogBoxCharText("Asha", _dialogTextList[_currentTextNr ?? default(int)]);
        }
        else 
        {
            ResetTextAndChoice();
            ShowDialogChoices();
        }
    }

    private void ShowDialogChoices()
    {
        _dialogBox.visible = true;
        _dialogBoxDialog.visible = false;
        _dialogBoxChoices.visible = true;

        var counter = 0;
        foreach (var dialog in _dialogBuilder.getAllDialogObjects())
        {
            _dialogBoxChoiceButtons[counter].text = dialog.getDialogChoice();
            counter++;
        }
        foreach (var dialogButton in _dialogBoxChoiceButtons)
        {
            dialogButton.SetEnabled(true);
            dialogButton.clickable.clickedWithEventInfo += ClickedDialogBoxChoiceButton;
        }
        
        unsetDialogCamera();
    }

    private void ClickedDialogBoxChoiceButton(EventBase tab)
    {
        _dialogBoxChoices.visible = false;
        SetDialogBoxCharText("Asha", _dialogTextList[_currentTextNr ?? default(int)]);
        foreach (var dialogButton in _dialogBoxChoiceButtons)
        {
            dialogButton.SetEnabled(false);
        }
        Button button = tab.target as Button;
        var buttonNr = Regex.Match(button.name, @"\d+").Value;
        _choiceClicked = Convert.ToInt32(buttonNr) -1;
        setDialogWithChoice();
        _currentTextNr = 0;
    }

    private void ResetTextAndChoice()
    {
        _choiceClicked = null;
        _currentTextNr = null;
    }

    private void SetDialogBoxCharText(string charName, string text)
    {
        _dialogBoxDialog.visible = true;
        _dialogBoxCharName.text = charName;
        _dialogBoxText.text = text;
    }

    public void SetDialogBuilder(DialogBuilder dialogBuilder)
    {
        this._dialogBuilder = dialogBuilder;
        setDialogWithChoice();
        var counter = 1;
        foreach (var dialog in _dialogBuilder.getAllDialogObjects())
        {
            _dialogBoxChoiceButtons.Add(_root.Q<Button>("dialog-box-choice-button-" + counter));
            counter++;
        }
        
    }

    private void setDialogWithChoice()
    {
        var dialogObjects = _dialogBuilder.getAllDialogObjects();
        chosenDialogOption = dialogObjects[_choiceClicked ?? default(int)];
        _dialogTextList = chosenDialogOption.getDialog();
        setDialogCamera();
    }

    private void setDialogCamera()
    {
        CinemachineVirtualCamera dialogCamera = chosenDialogOption.getDialogCamera();
        if (dialogCamera != null)
        {
            _activeCamera.Priority = (int)Camera.CameraState.Active;
        }
    }

    private void unsetDialogCamera()
    {
        CinemachineVirtualCamera dialogCamera = chosenDialogOption.getDialogCamera();
        if (dialogCamera != null)
        {
            _activeCamera.Priority = (int)Camera.CameraState.Inactive;
        }
    }
}
