#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Cinemachine;
using UnityEngine;

#endregion

public class DialogBuilder : MonoBehaviour
{
    [Header("Dialog Options")]
    private readonly List<DialogObject> _dialogOptions = new();

    private string _introText;

    // The text asset that contains all dialog.
    [SerializeField] private TextAsset _dialogTextFile;
    
    private bool _oneTimeConversation;
    private bool _endedConversation;

    [SerializeField] private List<CinemachineVirtualCamera> _eventCameras;
    [SerializeField] private CinemachineVirtualCamera _npcCamera;

    private const string DIALOG_EVENT_REGEX = "\\[((.*?)\\])";
    private const string NUMBER_REGEX = "[^0-9]";
    private const string DIALOG_OPTIONS_REGEX = "(\\*)([0-9]+)";

    [SerializeField] private bool _canSwitchDialog = false;
    
    private NpcAnimationController _npcAnimationController;

    [Header("Sound Settings")]
    [SerializeField] private List<DialogChoiceAudioSO> _eventAudio;


    private void Awake()
    {
        // Read the dialog file and make it into the dialogobjects.
        if (_dialogTextFile != null)
        {
            ParseDialog();
        }
        _canSwitchDialog = false;
        _npcAnimationController = GetComponent<NpcAnimationController>();
    }
    
    public void LoadDialog(TextAsset dialogTextFile)
    {
        _dialogTextFile = dialogTextFile;
        if (_dialogTextFile != null)
        {
            ParseDialog();
        }
    }

    /**
     * Split the dialog in:
     * Intro text
     * Dialog options
     * Dialog
     */
    private void ParseDialog()
    {
        _dialogOptions.Clear();
        _oneTimeConversation = false;
        _endedConversation = false;
        
        _introText = string.Empty;

        var textToSet = GetIntroText(_dialogTextFile.ToString());
        var regex = "#";
        var dialogObjects = Regex.Split(textToSet, regex).ToList();
        dialogObjects.Remove(dialogObjects[0]);
        RemoveSplitStrings(dialogObjects, false);
        CreateDialogObjects(dialogObjects);
    }

    private void RemoveSplitStrings(List<string> splittedOptions, bool isSubOptie)
    {
        // Unity's regex wont remove the characters from your text automatically, this is why we remove them by hand here .
        foreach (var option in splittedOptions.ToList())
        {
            // Dialog can't be only a number. This can be checked with a float.tryparse. We need a float for the output this is why this float is made.
            var checkerFloat = 0.0f;
            if ((isSubOptie && option.Contains('*')) || (!isSubOptie && option.Contains('#')) || option == " " ||
                float.TryParse(option, out checkerFloat))
            {
                splittedOptions.Remove(option);
            }
        }
    }

    /**
     * Splitting the dialog file into lose dialog options.
     * Every dialog option is a set of dialog based on the choice made by the player.
     * A dialog option contains the answer that needs to be selected to get this option and a list of all dialog for this option.
     */
    private void CreateDialogObjects(List<string> dialog)
    {
        foreach (var option in dialog)
        {
            var subdialog = Regex.Split(option, DIALOG_OPTIONS_REGEX).ToList();
            var subdialogTrimmed = subdialog.Select(s => s.Trim()).ToList();
            RemoveSplitStrings(subdialogTrimmed, true);
            var dialogTitle = subdialog[0].Trim();
            // Remove the introtext from the dialog options.
            subdialogTrimmed.Remove(subdialogTrimmed[0]);
            var dialogObject = new DialogObject(dialogTitle.Replace('$', ' '), subdialogTrimmed);
            CreateDialogEventObject(dialogObject);
            if (dialogTitle.Contains('$'))
            {
                dialogObject.SetEndsConversation(true);
            }

            if (dialogTitle.Contains('~'))
            {
                _oneTimeConversation = true;
            }

            _dialogOptions.Add(dialogObject);
        }
    }

    /**
     * Get the introduction text from the dialog.
     * The text you are first met with after opening the dialog or after completing the current dialog option.
     */
    private string GetIntroText(string text)
    {
        var splitText = text.Split("--");
        try
        {
            _introText = splitText[0];
            return splitText[1];
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogWarning("Text seems to be missing something, did you forget to add an intro text?");
            _introText = "Intro text not found";
            return splitText[0];
        }
    }


    /**
     * With this method the builder will extract the camera and audio tags from the dialog
     * and assign the right camera and audio clips to the dialog options
     */
    private void CreateDialogEventObject(DialogObject dialogObject)
    {
        var testTextList = Regex.Split(dialogObject.GetDialogChoice(), DIALOG_EVENT_REGEX);
        foreach (var text in testTextList)
        {
            var checkForCamera = "Camera:";
            var checkForAudio = "Audio:";
            var checkForNextCheckpoint = "Checkpoint";

            if (text.Contains(checkForCamera))
            {
                try
                {
                    var cameraNumberString = Regex.Replace(text, NUMBER_REGEX, "");
                    var cameraNumber = int.Parse(cameraNumberString);
                    dialogObject.SetDialogCamera(_eventCameras[cameraNumber]);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Debug.Log("No camera set for event");
                }
            }

            if (text.Contains(checkForAudio))
            {
                try
                {
                    var audioNumberString = Regex.Replace(text, NUMBER_REGEX, "");
                    var audioNumber = int.Parse(audioNumberString);
                    dialogObject.SetDialogAudio(_eventAudio[audioNumber]);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Debug.Log("No audio set for event");
                }
            }

            if (text.Contains(checkForNextCheckpoint))
            {
                dialogObject.ActivateNextCheckpoint();
            }
        }

        var dialogChoiceWithModulesRemoved = Regex.Replace(dialogObject.GetDialogChoice(), DIALOG_EVENT_REGEX, "");
        dialogObject.SetDialogChoice(dialogChoiceWithModulesRemoved);
    }

    /**
     * Getting a single dialog option by index
     */
    public DialogObject GetDialogOptionFromIndex(int optionIndex)

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
    public List<DialogObject> GetAllDialogObjects()
    {
        return _dialogOptions;
    }

    /**
     * Getting the introduction text from the npc.
     */
    public string GetIntroText()
    {
        return _introText;
    }

    public CinemachineVirtualCamera GetNpcCamera()
    {
        return _npcCamera;
    }

    public string GetNameOfNpc()
    {
        return gameObject.name;
    }
    
    public void SetOneTimeConversation(bool oneTimeConversation)
    {
        _oneTimeConversation = oneTimeConversation;
    }
    
    public bool GetOneTimeConversation()
    {
        return _oneTimeConversation;
    }

    public void SetEndedConversation(bool endedConversation)
    {
        _endedConversation = endedConversation;
    }

    public bool GetEndedConversation()
    {
        return _endedConversation;
    }

    public void SetCanSwitchDialog(bool canSwitchDialog)
    {
        _canSwitchDialog = canSwitchDialog;
    }
    public  NpcAnimationController GetNpcAnimationController()
    {
        return _npcAnimationController;
    }
}