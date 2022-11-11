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

    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // UI POC 1
        _interactBox = root.Q<VisualElement>("interact-box");
        _dialogBox = root.Q<VisualElement>("dialog-box");
        _dialogBoxText = root.Q<Label>("dialog-box-text");
        _dialogBoxChoices = root.Q<GroupBox>("dialog-box-choices");

        _dialogBoxChoiceButton1 = root.Q<Button>("dialog-box-choice-button-1");
        _dialogBoxChoiceButton2 = root.Q<Button>("dialog-box-choice-button-2");
        _dialogBoxChoiceButton3 = root.Q<Button>("dialog-box-choice-button-3");
        _dialogBoxChoiceButton4 = root.Q<Button>("dialog-box-choice-button-4");

        // UI POC 2
        _dialogBoxCharName = root.Q<Label>("dialog-box-char-name");

        _dialogBox.visible = false;

        _dialogTextList = new List<string>();

        _dialogTextList.Add("Hey there!");
        _dialogTextList.Add("You are the traveler right?");
        _dialogTextList.Add("I've heard about you, the slayer of dragons, the dragonborn!");
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

        if (_currentTextNr < _dialogTextList.Count && _choiceClicked != 0)
        {
            Debug.Log(_currentTextNr);
            SetDialogBoxText(_dialogTextList[_currentTextNr]);
        }
        else if (_currentTextNr == _dialogTextList.Count)
        {
            SetDialogBoxInvisible();
            _currentTextNr = 0;
            _choiceClicked = 0;
        }
    }

    private void ShowDialogChoices()
    {
        _dialogBoxCharName.visible = false;
        _dialogBoxText.visible = false;
        _dialogBoxChoices.visible = true;
        _dialogBoxChoiceButton1.text = "choice 1";
        _dialogBoxChoiceButton2.text = "choice 2";
        _dialogBoxChoiceButton3.text = "choice 3";
        _dialogBoxChoiceButton4.text = "choice 4";

        _dialogBoxChoiceButton1.SetEnabled(true);
        _dialogBoxChoiceButton2.SetEnabled(true);
        _dialogBoxChoiceButton3.SetEnabled(true);
        _dialogBoxChoiceButton4.SetEnabled(true);

        _dialogBoxChoiceButton1.clickable.clickedWithEventInfo += ClickedDialogBoxChoiceButton;
        _dialogBoxChoiceButton2.clickable.clickedWithEventInfo += ClickedDialogBoxChoiceButton;
        _dialogBoxChoiceButton3.clickable.clickedWithEventInfo += ClickedDialogBoxChoiceButton;
        _dialogBoxChoiceButton4.clickable.clickedWithEventInfo += ClickedDialogBoxChoiceButton;
    }

    private void ClickedDialogBoxChoiceButton(EventBase tab)
    {
        _dialogBoxChoices.visible = false;
        SetDialogBoxText(_dialogTextList[_currentTextNr]);

        _dialogBoxChoiceButton1.SetEnabled(false);
        _dialogBoxChoiceButton2.SetEnabled(false);
        _dialogBoxChoiceButton3.SetEnabled(false);
        _dialogBoxChoiceButton4.SetEnabled(false);

        Button button = tab.target as Button;
        var buttonNr = Regex.Match(button.name, @"\d+").Value;
        _choiceClicked = Convert.ToInt32(buttonNr);
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
        _currentTextNr++;
    }
}
