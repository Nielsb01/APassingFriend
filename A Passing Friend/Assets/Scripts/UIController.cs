using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    // UI POC 1
    private VisualElement _interactBox;

    private VisualElement _dialogBox;

    private Label _dialogBoxText;

    // UI POC 2, please comment when not using.
    private Label _dialogBoxCharName;

    private List<string> dialogTextList;

    [SerializeField] 
    private int currentTextNr = 0;

    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // UI POC 1
        _interactBox = root.Q<VisualElement>("interact-box");
        _dialogBox = root.Q<VisualElement>("dialog-box");
        _dialogBoxText = root.Q<Label>("dialog-box-text");

        // UI POC 2
        _dialogBoxCharName = root.Q<Label>("dialog-box-char-name");

        _dialogBox.visible = false;

        dialogTextList = new List<string>();

        dialogTextList.Add("Hey there!");
        dialogTextList.Add("You are the traveler right?");
        dialogTextList.Add("I've heard about you, the slayer of dragons, the dragonborn!");
    }

    public void SetInteractButtonVisibility()
    {
        _interactBox.visible = !_interactBox.visible;
    }

    public void ContinueDialog()
    {
        if (!_interactBox.visible)
        {
            return;
        }

        if (!_dialogBox.visible)
        {
            _dialogBox.visible = true;
            currentTextNr = 0;
        }

        if (currentTextNr < (dialogTextList.Count))
        {
            SetDialogBoxText(dialogTextList[currentTextNr]);
            currentTextNr++;
            Debug.Log(currentTextNr);
        }
        else if (currentTextNr == dialogTextList.Count)
        {
            _dialogBox.visible = false;
            currentTextNr = 0;
        }
    }

    private void SetDialogBoxText(string text)
    {
        _dialogBoxText.text = text;
    }
}
