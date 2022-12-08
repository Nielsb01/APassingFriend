using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _dialogObject
{
    private readonly string _dialogChoice;
    private readonly List<string> _dialog;
    private bool _endsConversation = false;

    public _dialogObject(string _dialogChoice, List<string> _dialog)
    {
        this._dialogChoice = _dialogChoice;
        this._dialog = _dialog;
    }

    public string get__dialogChoice()
    {
        return this._dialogChoice;
    }

    public List<string> get_dialog()
    {
        return this._dialog;
    }

    public void setEndsConverstation(bool _endsConversation)
    {
        this._endsConversation = _endsConversation;
    }

    public bool doesOptionEndConverstation()
    {
        return _endsConversation;
    }
}
