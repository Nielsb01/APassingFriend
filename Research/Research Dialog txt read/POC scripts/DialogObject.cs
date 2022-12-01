using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogObject
{
    private readonly string dialogChoice;
    private readonly List<string> dialog;
    private bool endsConversation = false;

    public DialogObject(string dialogChoice, List<string> dialog)
    {
        this.dialogChoice = dialogChoice;
        this.dialog = dialog;
    }

    public string getDialogChoice()
    {
        return this.dialogChoice;
    }

    public List<string> getDialog()
    {
        return this.dialog;
    }

    public void setEndsConverstation(bool endsConversation)
    {
        this.endsConversation = endsConversation;
    }

    public bool doesOptionEndConverstation()
    {
        return endsConversation;
    }
}
