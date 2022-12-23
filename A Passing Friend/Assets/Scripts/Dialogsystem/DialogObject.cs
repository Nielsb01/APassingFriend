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
    private bool _activateNextCheckpoint;

    private CinemachineVirtualCamera _dialogCamera;
    private DialogChoiceAudioSO _dialogAudio;

    public DialogObject(string dialogChoice, List<string> dialog)
    {
        _dialogChoice = dialogChoice;
        _dialog = dialog;
    }

    public string GetDialogChoice()
    {
        return _dialogChoice;
    }

    public void SetDialogChoice(string dialogChoice)
    {
        _dialogChoice = dialogChoice;
    }

    public List<string> GetDialog()
    {
        return _dialog;
    }

    public void SetEndsConversation(bool endsConversation)
    {
        _endsConversation = endsConversation;
    }

    public bool DoesOptionEndConversation()
    {
        return _endsConversation;
    }

    public void SetDialogCamera(CinemachineVirtualCamera dialogCamera)
    {
        _dialogCamera = dialogCamera;
    }

    public void SetDialogAudio(DialogChoiceAudioSO dialogAudio)
    {
        _dialogAudio = dialogAudio;
    }

    public CinemachineVirtualCamera GetDialogCamera()
    {
        return _dialogCamera;
    }

    public DialogChoiceAudioSO GetDialogAudio()
    {
        return _dialogAudio;
    }

    public void ActivateNextCheckpoint()
    {
        _activateNextCheckpoint = true;
    }

    public bool DoesOptionConversationActivateNextCheckpoint()
    {
        return _activateNextCheckpoint;
    }
}