#region

using System.Collections.Generic;
using TMPro;
using UnityEngine;

#endregion

public class SetText : MonoBehaviour
{
    [SerializeField] private int _chosen = 1;

    [SerializeField] private DialogBuilder _dialogreader;

    [SerializeField] private Transform _onscreenText;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera activeCamera;

    public DialogObject chosenOption;

    private int currentDialog;
    private List<string> dialogOptionsText;


    // Update is called once per frame
    private void Update()
    {
        if (chosenOption == null)
        {
            var dialogObjects = _dialogreader.getAllDialogObjects();
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
        if (currentDialog < dialogOptionsText.Count - 1)
        {
            currentDialog += 1;
            setCamera();
            SetDialogText(currentDialog);
        }
        else
        {
            if (chosenOption.doesOptionEndConverstation())
            {
                print("ends");
            }
            resetCamera();
            currentDialog = 0;
            chosenOption = null;
            dialogOptionsText = null;
            setIntroText();

        }
    }

    private void SetDialogText(int optionNumber)
    {
        var textMeshProUgui = _onscreenText.GetComponent<TextMeshProUGUI>();
        textMeshProUgui.text = dialogOptionsText[optionNumber];
    }

    private void setIntroText()
    {
        var textMeshProUgui = _onscreenText.GetComponent<TextMeshProUGUI>();
        textMeshProUgui.text = _dialogreader.getIntroText();
    }

    private void setCamera()
    {
        mainCamera.gameObject.SetActive(false);
        activeCamera = chosenOption.getDialogCamera();
        activeCamera.gameObject.SetActive(true);
    }

    private void resetCamera()
    {
        activeCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }
}