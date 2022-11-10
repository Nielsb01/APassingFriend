using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private VisualElement interactBox;
    private VisualElement dialogBox;
    private Label dialogBoxText;

    private List<string> dialogTextList;
    [SerializeField] private int currentTextNr = 0;

    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        interactBox = root.Q<VisualElement>("interact-box");
        dialogBox = root.Q<VisualElement>("dialog-box");
        dialogBoxText = root.Q<Label>("dialog-box-text");

        dialogBox.visible = false;

        dialogTextList = new List<string>();

        dialogTextList.Add("Hey there!");
        dialogTextList.Add("You are the traveler right?");
        dialogTextList.Add("I've heard about you, the slayer of dragons, the dragonborn!");
    }

    public void SetInteractButtonVisibility()
    {
        interactBox.visible = !interactBox.visible;
    }

    public void ContinueDialog()
    {
        if (interactBox.visible)
        {
            if (!dialogBox.visible)
            {
                dialogBox.visible = true;
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
                dialogBox.visible = false;
                currentTextNr = 0;
            }
        }
    }

    private void SetDialogBoxText(string text)
    {
        dialogBoxText.text = text;
    }
}
