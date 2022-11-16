#region

using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

#endregion

public class DialogObject
{
    private readonly List<string> _dialog;
    private string _dialogChoice;
    private bool _endsConversation;

    private CinemachineVirtualCamera _dialogCamera;
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

    public void setDialogChoice(string dialogChoice)
    {
        this._dialogChoice = dialogChoice;
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

    public void setDialogCamera(CinemachineVirtualCamera dialogCamera)
    {
        this._dialogCamera = dialogCamera;
    }

    public void setDialogAudio(AudioClip dialogAudio)
    {
        this._dialogAudio = dialogAudio;
    }

    public CinemachineVirtualCamera getDialogCamera()
    {
        return this._dialogCamera;
    }

    public AudioClip getDialogAudio()
    {
        return this._dialogAudio;
    }
}