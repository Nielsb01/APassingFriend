#region

using System.Collections.Generic;
using UnityEngine;

#endregion

public class DialogObject
{
    private readonly List<string> _dialog;
    private readonly string _dialogChoice;
    private bool _endsConversation;

    private Camera _dialogCamera;
    private AudioClip _dialogAudio;

    public DialogObject(string dialogChoice, List<string> dialog)
    {
        _dialogChoice = dialogChoice;
        _dialog = dialog;
    }

    public string getDialogChoice()
    {
        return _dialogChoice;
    }

    public List<string> getDialog()
    {
        return _dialog;
    }

    public void setEndsConverstation(bool endsConversation)
    {
        _endsConversation = endsConversation;
    }

    public bool doesOptionEndConverstation()
    {
        return _endsConversation;
    }

    public void setDialogCamera(Camera dialogCamera)
    {
        this._dialogCamera = dialogCamera;
        Debug.Log("Camera set: " + dialogCamera.name + " for " + _dialogChoice);
    }

    public void setDialogAudio(AudioClip dialogAudio)
    {
        this._dialogAudio = dialogAudio;
        Debug.Log("Audio set: " + dialogAudio.name + " for " + _dialogChoice);
    }

    public Camera getDialogCamera()
    {
        return this._dialogCamera;
    }

    public AudioClip getDialogAudio()
    {
        return this._dialogAudio;
    }
}