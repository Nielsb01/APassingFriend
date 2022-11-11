using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    // UI POC 1
    private VisualElement _interactBox;

    private VisualElement _dialogBox;

    private Label _dialogBoxText;

    private GroupBox _dialogBoxChoices;

    private Button _dialogBoxChoiceButton1;

    private Button _dialogBoxChoiceButton2;

    private Button _dialogBoxChoiceButton3;

    private Button _dialogBoxChoiceButton4;

    // UI POC 2, please comment when not using.
    private Label _dialogBoxCharName;

    private List<string> _dialogTextList;

    private int _currentTextNr = 0;

    private int _choiceClicked = 0;
    
    [SerializeField] private DialogBuilder _dialogBuilder;
    
    private DialogObject chosenDialogOption;
    
    private List<Button> _dialogBoxChoiceButtons = new List<Button>();
    
    private VisualElement root;

    private void Start()
    { 
        root = GetComponent<UIDocument>().rootVisualElement;
        _interactBox = root.Q<VisualElement>("interact-box");
        _dialogBox = root.Q<VisualElement>("dialog-box");
        _dialogBoxText = root.Q<Label>("dialog-box-text");
        _dialogBoxChoices = root.Q<GroupBox>("dialog-box-choices");
        _dialogBox.visible = false;
        _dialogBoxCharName = root.Q<Label>("dialog-box-char-name");
    }

    public void SetInteractButtonVisibility()
    {
        _interactBox.visible = !_interactBox.visible;
    }

    public void ContinueDialog()
    {
        if (!_interactBox.visible)
        {
            SetDialogBoxInvisible();
            return;
        }

        if (!_dialogBox.visible)
        {
            _dialogBox.visible = true;
            ShowDialogChoices();           
        }

        if (_currentTextNr < _dialogTextList.Count -1)
        {
            _currentTextNr++;
            SetDialogBoxText(_dialogTextList[_currentTextNr]);
        }

        else 
        {
            _currentTextNr = 0;
            _choiceClicked = 0;
            ShowDialogChoices();
        }
    }

    private void ShowDialogChoices()
    {
        _dialogBox.visible = true;
        _dialogBoxCharName.visible = false;
        _dialogBoxText.visible = false;
        _dialogBoxChoices.visible = true;
        var counter = 0;
        foreach (var dialog in _dialogBuilder.getAllDialogObjects())
        {
            _dialogBoxChoiceButtons[counter].text = dialog.getDialogChoice();
            counter += 1;
        }
        foreach (var dialogButton in _dialogBoxChoiceButtons)
        {
            dialogButton.SetEnabled(true);
            dialogButton.clickable.clickedWithEventInfo += ClickedDialogBoxChoiceButton;
        }
    }

    private void ClickedDialogBoxChoiceButton(EventBase tab)
    {
        _dialogBoxChoices.visible = false;
        SetDialogBoxText(_dialogTextList[_currentTextNr]);
        foreach (var dialogButton in _dialogBoxChoiceButtons)
        {
            dialogButton.SetEnabled(false);
        }
        Button button = tab.target as Button;
        var buttonNr = Regex.Match(button.name, @"\d+").Value;
        _choiceClicked = Convert.ToInt32(buttonNr) -1;
        setDialogWithChoice();
    }

    private void SetDialogBoxInvisible()
    {
        _dialogBox.visible = false;
        _dialogBoxChoices.visible = false;
        _dialogBoxCharName.visible = false;
        _dialogBoxText.visible = false;
    }

    private void SetDialogBoxText(string text)
    {
        _dialogBoxCharName.visible = true;
        _dialogBoxText.visible = true;
        _dialogBoxText.text = text;
    }
    public void SetDialogBuilder(DialogBuilder dialogBuilder)
    {
        this._dialogBuilder = dialogBuilder;
        setDialogWithChoice();
        var counter = 1;
        _currentTextNr = 0;
        foreach (var dialog in _dialogBuilder.getAllDialogObjects())
        {
            _dialogBoxChoiceButtons.Add(root.Q<Button>("dialog-box-choice-button-" + counter));
            counter += 1;
        }
        
    }

    private void setDialogWithChoice()
    {
        var dialogObjects = _dialogBuilder.getAllDialogObjects();
        chosenDialogOption = dialogObjects[_choiceClicked];
        _dialogTextList = chosenDialogOption.getDialog();
    }
}
