using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetText : MonoBehaviour
{
    [SerializeField]
    private DialogBuilder _dialogreader;
    [SerializeField]
    private Transform _onscreenText;
    [SerializeField]
    private int _chosen = 1;
    
    private int currentDialog = 0;
    private List<String> dialogOptionsText;
    private DialogObject chosenOption;
    

    // Update is called once per frame
    void Update()
    {
        if (chosenOption == null)
        {
            List<DialogObject> dialogObjects = _dialogreader.getAllDialogObjects();
            chosenOption = dialogObjects[_chosen];
        }

        if (dialogOptionsText == null)
        {
            dialogOptionsText = chosenOption.getDialog();
            setIntroText();
        }
    }

   public void OnFire()
    {
        if (currentDialog < dialogOptionsText.Count -1)
        {
            currentDialog += 1;
            SetDialogText(currentDialog);
        }
        else
        {
            if (chosenOption.doesOptionEndConverstation())
            {
                print("ends");
            }
            currentDialog = 0;
            chosenOption = null;
            dialogOptionsText = null;
            setIntroText();
        }
    }

    private void SetDialogText(int optionNumber)
    {
        TextMeshProUGUI textMeshProUgui = _onscreenText.GetComponent<TextMeshProUGUI>();
        textMeshProUgui.text = dialogOptionsText[optionNumber]; 
    }

    private void setIntroText()
    {
        TextMeshProUGUI textMeshProUgui = _onscreenText.GetComponent<TextMeshProUGUI>();
        textMeshProUgui.text = _dialogreader.getIntroText();
    }
}
