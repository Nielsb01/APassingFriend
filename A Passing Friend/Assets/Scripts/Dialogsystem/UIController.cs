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

    // Dialog Builder

    private List<string> _dialogTextList;

    [SerializeField] private DialogBuilder _dialogBuilder;

    private DialogObject chosenDialogOption;

    private int? _currentTextNr = null;

    private int? _choiceClicked = null;

    [SerializeField] private CinemachineVirtualCamera _activeCamera;
    private CinemachineVirtualCamera _npcCamera;
    private string _npcName;

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

    private void FixedUpdate()
    {
        if (!_interactBox.visible)
        {
            SetDialogBoxInvisible();
            ResetDialogue();
            unsetDialogCamera();
            unsetNpcCamera();
            return;
        }
    }

    // UI Dialog System
    public void ContinueDialog()
    {
        //Debug.Log(_dialogBox.contentRect.width);
        //Debug.Log((_dialogBox.contentRect.width / 100) * 40);
        Debug.Log("test");

        if (!_dialogBox.visible)
        {
            _dialogBox.visible = true;
            ShowDialogChoices();
            SetDialogBoxCharText(_npcName, _dialogBuilder.getIntroText());
            setNpcCamera();
        }

        if (_currentTextNr < (_dialogTextList.Count - 1))
        {
            _currentTextNr++;
            SetDialogBoxCharText(_npcName, _dialogTextList[_currentTextNr ?? default(int)]);
        }
        else
        {
            if (chosenDialogOption.doesOptionEndConverstation())
            {
                SetDialogBoxInvisible();
                ResetDialogue();
                unsetDialogCamera();
                unsetNpcCamera();
                return;
            }
            else
            {
                ResetDialogue();
                ShowDialogChoices();
            }
        }
    }

    private void ShowDialogChoices()
    {
        _dialogBox.visible = true;
        _dialogBoxChoices.visible = true;

        var counter = 0;
        foreach (var dialog in _dialogBuilder.getAllDialogObjects())
        {
            Debug.Log(dialog.getDialogChoice());
            _dialogBoxChoiceButtons[counter].text = dialog.getDialogChoice();
            _dialogBoxChoiceButtons[counter].style.fontSize = 30;
            _dialogBoxChoiceButtons[counter].SetEnabled(true);
            _dialogBoxChoiceButtons[counter].clickable.clickedWithEventInfo += ClickedDialogBoxChoiceButton;
            counter++;
        }
        SetDialogBoxCharText(_npcName, _dialogBuilder.getIntroText());
        unsetDialogCamera();
    }

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
        setDialogWithChoice();
        setDialogCamera();
        _currentTextNr = 0;
    }

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

    private void SetDialogBoxCharText(string charName, string text)
    {
        _dialogBoxDialog.visible = true;
        _dialogBoxCharName.text = charName;
        //_dialogBoxCharName.style.fontSize = 30;
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
        _npcName = _dialogBuilder.getNameOfNpc();
        _npcCamera = _dialogBuilder.getNpcCamera();

    }

    private void setNpcCamera()
    {
        _npcCamera.Priority = (int)Camera.CameraState.Active;
    }

    private void unsetNpcCamera()
    {
        _npcCamera.Priority = (int)Camera.CameraState.Inactive;
    }

    private void setDialogCamera()
    {
        _activeCamera = chosenDialogOption.getDialogCamera();
        if (_activeCamera != null)
        {
            _activeCamera.Priority = (int)Camera.CameraState.Active;
        }
    }

    private void unsetDialogCamera()
    {
        if (_activeCamera != null)
        {
            _activeCamera.Priority = (int)Camera.CameraState.Inactive;
        }
    }
}
