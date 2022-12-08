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
    
    private int _currentDialog = 0;
    private List<String> _dialogOptionsText;
    private DialogObject _chosenOption;
    

    // Update is called once per frame
    void Update()
    {
        if (_chosenOption == null)
        {
            List<DialogObject> dialogObjects = _dialogreader.getAllDialogObjects();
            _chosenOption = dialogObjects[_chosen];
        }

        if (_dialogOptionsText == null)
        {
            _dialogOptionsText = _chosenOption.getDialog();
            setIntroText();
        }
    }

   public void OnFire()
    {
        if (_currentDialog < _dialogOptionsText.Count -1)
        {
            _currentDialog += 1;
            SetDialogText(_currentDialog);
        }
        else
        {
            if (_chosenOption.doesOptionEndConverstation())
            {
                print("ends");
            }
            _currentDialog = 0;
            _chosenOption = null;
            _dialogOptionsText = null;
            setIntroText();
        }
    }

    private void SetDialogText(int optionNumber)
    {
        TextMeshProUGUI textMeshProUgui = _onscreenText.GetComponent<TextMeshProUGUI>();
        textMeshProUgui.text = _dialogOptionsText[optionNumber]; 
    }

    private void setIntroText()
    {
        TextMeshProUGUI textMeshProUgui = _onscreenText.GetComponent<TextMeshProUGUI>();
        textMeshProUgui.text = _dialogreader.getIntroText();
    }
}
