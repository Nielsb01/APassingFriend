#region

using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;

#endregion

public class SetText : MonoBehaviour
{
    [SerializeField] private int _chosen = 1;

    [SerializeField] private DialogBuilder _dialogreader;

    [SerializeField] private Transform _onscreenText;
    [SerializeField] private CinemachineVirtualCamera _activeCamera;

    [SerializeField] private DialogObject _chosenOption;

    private int _currentDialog;
    private List<string> _dialogOptionsText;


    // Update is called once per frame
    private void Update()
    {
        if (_chosenOption == null)
        {
            var dialogObjects = _dialogreader.getAllDialogObjects();
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
        if (_currentDialog < _dialogOptionsText.Count - 1)
        {
            _currentDialog += 1;
            setCamera();
            SetDialogText(_currentDialog);
        }
        else
        {
            print(_chosenOption.getDialogChoice());
            if (_chosenOption.doesOptionEndConverstation())
            {
                print("ends");
            }
            resetCamera();
            _currentDialog = 0;
            _chosenOption = null;
            _dialogOptionsText = null;
            setIntroText();

        }
    }

    private void SetDialogText(int optionNumber)
    {
        var textMeshProUgui = _onscreenText.GetComponent<TextMeshProUGUI>();
        textMeshProUgui.text = _dialogOptionsText[optionNumber];
    }

    private void setIntroText()
    {
        var textMeshProUgui = _onscreenText.GetComponent<TextMeshProUGUI>();
        textMeshProUgui.text = _dialogreader.getIntroText();
    }

    private void setCamera()
    {
        _activeCamera = _chosenOption.getDialogCamera();
        _activeCamera.Priority = (int)Camera.CameraState.Active;
    }

    private void resetCamera()
    {
        _activeCamera.Priority =  (int)Camera.CameraState.Inactive;
    }
}