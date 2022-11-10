using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogBuilder : MonoBehaviour
{
    private readonly List<DialogObject> _dialogOptions = new();

    // The text asset that contains all dialog.
    [SerializeField] private TextAsset _dialogTextFile;

    private string _introText;

    private void Awake()
    {
        // Read the dialog file and make it into the dialogobjects.
        parseDialog();
    }

    /**
     * Split the dialog in:
     * Intro text
     * Dialog options
     * Dialog
     */
    private void parseDialog()
    {
        var textToSet = getIntroText(_dialogTextFile.ToString());
        var regex = "#";
        var dialogObjects = Regex.Split(textToSet, regex).ToList();
        dialogObjects.Remove(dialogObjects[0]);
        removeSplitStrings(dialogObjects, false);
        createDialogObjects(dialogObjects);
    }

    private void removeSplitStrings(List<string> splittedOptions, bool isSubOptie)
    {
        // Unity's regex wont remove the characters from your text automatically, this is why we remove them by hand here .
        foreach (var option in splittedOptions.ToList())
        {
            // Dialog can't be only a number. This can be checked with a float.tryparse. We need a float for the output this is why this float is made.
            var checkerFloat = 0.0f;
            if ((isSubOptie && option.Contains('*')) || (!isSubOptie && option.Contains('#')) || option == " " ||
                float.TryParse(option, out checkerFloat))
                splittedOptions.Remove(option);
        }
    }

    /**
     * Splitting the dialog file into lose dialog options.
     * Every dialog option is a set of dialog based on the choice made by the player.
     * A dialog option contains the answer that needs to be selected to get this option and a list of all dialog for this option.
     */
    private void createDialogObjects(List<string> dialog)
    {
        var regex = "(\\*)([0-9]+)";
        foreach (var option in dialog)
        {
            var subdialog = Regex.Split(option, regex).ToList();
            var subdialogTrimmed = subdialog.Select(s => s.Trim()).ToList();
            removeSplitStrings(subdialogTrimmed, true);
            var dialogTitle = subdialog[0];
            // Remove the introtext from the dialog options.
            subdialog.Remove(subdialog[0]);
            var dialogObject = new DialogObject(dialogTitle.Replace('$', ' '), subdialogTrimmed);
            if (dialogTitle.Contains('$')) dialogObject.setEndsConverstation(true);
            _dialogOptions.Add(dialogObject);
        }
    }

    /**
     * Get the introduction text from the dialog.
     * The text you are first met with after opening the dialog or after completing the current dialog option.
     */
    private string getIntroText(string text)
    {
        var splitText = text.Split("--");
        try
        {
            _introText = splitText[0];
            return splitText[1];
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.LogWarning("Text seems to be missing something, did you forget to add an intro text?");
            _introText = "Intro text not found";
            return splitText[0];
        }
    }

    /**
     * Getting a single dialog option by index
     */
    public DialogObject getDialogOptionFromIndex(int optionIndex)

    {
        if (optionIndex > _dialogOptions.Count)
        {
            Debug.LogWarning("Dialog option not found, returning the first option.");
            return _dialogOptions[0];
        }

        return _dialogOptions[optionIndex];
    }

    /**
     * Getting all available dialogs from the npc
     */
    public List<DialogObject> getAllDialogObjects()
    {
        return _dialogOptions;
    }

    /**
     * Getting the introduction text from the npc.
     */
    public string getIntroText()
    {
        return _introText;
    }
}