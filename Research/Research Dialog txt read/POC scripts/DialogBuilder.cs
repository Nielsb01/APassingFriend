using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogBuilder : MonoBehaviour
{
    //The text asset that contains all dialog.
    [SerializeField] private TextAsset dialogTextFile;

    private String introText;
    private readonly List<DialogObject> dialogOptions = new List<DialogObject>();

    void Awake()
    {
        //Read the dialog file and make it into the dialogobjects.
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
        string textToSet = getIntroText(dialogTextFile.ToString());
        string regex = "#";
        List<string> dialogObjects = Regex.Split(textToSet, regex).ToList();
        dialogObjects.Remove(dialogObjects[0]);
        removeSplitStrings(dialogObjects, false);
        createDialogObjects(dialogObjects);
    }

    private void removeSplitStrings(List<string> splittedOptions, bool isSubOptie)
    {
        //Unity's regex wont remove the characters from your text automatically, this is why we remove them by hand here .
        foreach (var optie in splittedOptions.ToList())
        {
            //Dialog can't be only a number. This can be checked with a float.tryparse. We need a float for the output this is why this float is made.
            float checkerFloat = 0.0f;
            if ((isSubOptie && optie.Contains('*')) || (!isSubOptie && optie.Contains('#')) || optie == " " ||
                float.TryParse(optie, out checkerFloat))
            {
                splittedOptions.Remove(optie);
            }
        }
    }

    /**
     * Splitting the dialog file into lose dialog options.
     * Every dialog option is a set of dialog based on the choice made by the player.
     * A dialog option contains the answer that needs to be selected to get this option and a list of all dialog for this option.
     */
    private void createDialogObjects(List<string> dialog)
    {
        string regex = "(\\*)([0-9]+)";
        foreach (string option in dialog)
        {
            List<string> subdialog = Regex.Split(option, regex).ToList();
            removeSplitStrings(subdialog, true);
            string dialogTitle = subdialog[0];
            //Remove the introtext from the dialog options.
            subdialog.Remove(subdialog[0]);
            DialogObject dialogObject = new DialogObject(dialogTitle, subdialog);
            dialogOptions.Add(dialogObject);
        }
    }

    /**
     * Get the introduction text from the dialog.
     * The text you are first met with after opening the dialog or after completing the current dialog option.
     */
    private String getIntroText(string text)
    {
        string regex = "--";
        string[] splitText = Regex.Split(text, regex);
        try
        {
            introText = splitText[0];
            return splitText[1];
        }
        catch (IndexOutOfRangeException e)
        {
            Console.WriteLine(e);
            Debug.LogWarning("Text seems to be missing something, did you forget to add an intro text?");
            introText = "Intro text not found";
            return splitText[0];
        }
    }

    /**
     * Getting a single dialog option by index
     */
    public DialogObject getDialogOptionFromIndex(int optionIndex)

    {
        if (optionIndex > dialogOptions.Count)
        {
            Debug.LogWarning("Dialog option not found, returning the first option.");
            return dialogOptions[0];
        }

        return dialogOptions[optionIndex];
    }

    /**
     * Getting all available dialogs from the npc
     */
    public List<DialogObject> getAllDialogObjects()
    {
        return dialogOptions;
    }

    /**
     * Getting the introduction text from the npc.
     */
    public String getIntroText()
    {
        return introText;
    }
}