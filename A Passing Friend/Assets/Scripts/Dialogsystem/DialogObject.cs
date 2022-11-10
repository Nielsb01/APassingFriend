using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogObject
{
    private readonly string _dialogChoice;
    private readonly List<string> _dialog;
    private bool _endsConversation = false;

    public DialogObject(string dialogChoice, List<string> dialog)
    {
        this._dialogChoice = dialogChoice;
        this._dialog = dialog;
    }

    public string getDialogChoice()
    {
        return this._dialogChoice;
    }

    public List<string> getDialog()
    {
        return this._dialog;
    }

    public void setEndsConverstation(bool endsConversation)
    {
        this._endsConversation = endsConversation;
    }

    public bool doesOptionEndConverstation()
    {
        return _endsConversation;
    }
}
