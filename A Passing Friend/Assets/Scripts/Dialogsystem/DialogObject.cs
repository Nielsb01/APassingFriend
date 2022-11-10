using System.Collections.Generic;

public class DialogObject
{
    private readonly List<string> _dialog;
    private readonly string _dialogChoice;
    private bool _endsConversation;

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
}