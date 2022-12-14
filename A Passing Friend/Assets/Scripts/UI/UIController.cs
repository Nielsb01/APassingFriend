using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    // UI
    [Header("Interaction")]
    private GroupBox _interactBox;

    private bool _isInInteractRange;

    private bool _isInInteraction;


    // // Dialog UI
    private VisualElement _dialogBox;

    private GroupBox _dialogBoxDialog;

    private Label _dialogBoxCharName;

    private Label _dialogBoxIntroText;

    private Label _dialogBoxText;

    private GroupBox _dialogBoxChoices;

    private List<Button> _dialogBoxChoiceButtons = new List<Button>();

    private VisualElement _root;

    [Header("Dialog")]
    [SerializeField] private bool _isDialogExitButtonVisible = false;

    private Button _dialogBoxExitButton;


    // // Dialog Builder
    private List<string> _dialogTextList;

    private string _npcName;

    [Header("Dialog Builder")]
    [SerializeField] private DialogBuilder _dialogBuilder;

    private DialogObject _chosenDialogOption;

    private int? _currentTextNr = null; // the current number of text in the text list.

    private int? _choiceClicked = null; // the choice of dialog clicked.

    [SerializeField] private CinemachineVirtualCamera _activeCamera;

    private CinemachineVirtualCamera _npcCamera;

    [SerializeField] private bool _isDialogBuilderSet;


    // Jump charge bar
    private JumpChargeBar _jumpChargeBar;

    [Header("Jump Charge Bar")]
    [SerializeField] private float _minJumpCharge = 0;

    [SerializeField] private float _maxJumpCharge;

    [SerializeField] private float _overchargeJumpModifier = 1.2f; // the modifier used to determine how far the bar overcharges visually, always 0.1 less than the width of the overcharge bar.
  
    [SerializeField] private float _currentJumpCharge = 0; // the current charge on the bar.
    
    [SerializeField] [Range(0, 2)] private float _jumpChargePercent = 0; // the percent of the bar that is filled (1 = 100%).


    // Character Movement
    [Header("External scripts")]
    [SerializeField] private CharacterMovementScript _characterMovementScript;
    
    // HealthController
    [SerializeField] private HealthController _healthController;

    private VisualElement _healthVignette;


    // Screen
    [Header("Screen")]
    [SerializeField] private int _lastScreenWidth;

    [SerializeField] private int _lastScreenHeight;
    
    
    // Event
    public delegate void DialogEvent();
    public static event DialogEvent DialogExited;
    
    private void OnEnable()
    {
        HealthController.Ded += PlayerDies;
    }
    private void OnDisable()
    {
        HealthController.Ded -= PlayerDies;
    }


    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        
        // Interaction
        _interactBox = _root.Q<GroupBox>("interact-box");
        
        // Dialog
        _dialogBox = _root.Q<VisualElement>("dialog-box");
        _dialogBoxDialog = _root.Q<GroupBox>("dialog-box-dialog");
        _dialogBoxCharName = _root.Q<Label>("dialog-box-char-name");
        _dialogBoxIntroText = _root.Q<Label>("dialog-box-intro-text");
        _dialogBoxText = _root.Q<Label>("dialog-box-text");
        _dialogBoxChoices = _root.Q<GroupBox>("dialog-box-choices");
        _dialogBoxExitButton = _root.Q<Button>("dialog-box-exit-button");
        _dialogBox.visible = false;
        _interactBox.visible = false;

        // Jump charge bar
        _jumpChargeBar = _root.Q<JumpChargeBar>("jump-charge-bar");
        _maxJumpCharge = _characterMovementScript.GetOverchargeLevel();
        _jumpChargeBar.value = _currentJumpCharge / _maxJumpCharge;
        _jumpChargeBar.visible = false;

        // Health
        _healthVignette = _root.Q<VisualElement>("health-vignette");

        // Screen
        _lastScreenWidth = Screen.width;
        _lastScreenHeight = Screen.height;

        ChangeFontDynamically();
        ChangeButtonFontDynamically();
    }

    private void FixedUpdate()
    {
        // Unfreeze the player when they are not in interact range with anything.
        if (!_isInInteractRange)
        {
            _characterMovementScript.FreezeMovement(false, false);
        }

    }

    private void Update()
    {
        // Alter the health vignette based on the amount of damage the player got.
        AlterHealthVignette();

        if (_healthController.IsDead)
        {
            return;
        }

        /*
        Set the dialog system invisible when the player is not or no longer in the interaction range of a NPC.
        Currently coded for the dialog system, can be easily adapted for items as well.
        */
        if (!_isInInteractRange)
        {
            _characterMovementScript.FreezeMovement(false, false);
            _isInInteraction = false;

            SetDialogSystemInvisible();
            ResetDialogue();
            UnsetDialogCamera();
            UnsetNpcCamera();
        }
        else
        {
            if (_isInInteraction)
            {
                _dialogBoxExitButton.visible = _isDialogExitButtonVisible;
                _dialogBoxExitButton.SetEnabled(_isDialogExitButtonVisible);
            }

            CheckForScreenResolutionChanges();
        }

        // Charge or decharge the jump bar if the player is currently charging or not.
        if (!_characterMovementScript.GetHoldingDownJump())
        {
            _maxJumpCharge = _characterMovementScript.GetOverchargeLevel();

            ChargeJump();

            if (_currentJumpCharge == _minJumpCharge)
            {
                _jumpChargeBar.visible = false;
            }
        }
        else
        {
            _maxJumpCharge = _characterMovementScript.GetOverchargeLevel();

            _jumpChargeBar.visible = true;

            ChargeJump();
        }
    }

    private void OnValidate()
    {
        // If the jump charge bar is initialized, set its value to the starting value.
        if (_jumpChargeBar != null)
        {
            _jumpChargeBar.value = _jumpChargePercent;
            _currentJumpCharge = (int)(_jumpChargePercent * _maxJumpCharge);
        }
    }

    /* 
     * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
     *                                  INTERACTION
     * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    */

    public void SetIsInInteractRange(bool isInInteractRange)
    {
        _isInInteractRange = isInInteractRange;
    }

    // Set the interact box visible.
    public void SetInteractBoxVisible()
    {
        if (!_isInInteraction)
        {
            _interactBox.visible = true;
        }
    }

    /* 
     * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
     *                                  DIALOG
     * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    */

    public void SetIsDialogBuilderSet(bool isDialogBuilderSet)
    {
        _isDialogBuilderSet = isDialogBuilderSet;
    }

    // Set the dialog system invisible.
    private void SetDialogSystemInvisible()
    {
        _isInInteraction = false;

        _interactBox.visible = false;
        _dialogBox.visible = false;
        _dialogBoxChoices.visible = false;
        _dialogBoxDialog.visible = false;
        _dialogBoxExitButton.visible = false;

        _dialogBoxExitButton.SetEnabled(false);

        foreach (var dialogButton in _dialogBoxChoiceButtons)
        {
            dialogButton.visible = false;
        }
    }

    // Cycle through the dialog.
    public void ContinueDialog()
    {
        if (_healthController.IsDead) return;

        if (_dialogBuilder.GetEndedConversation()) return;

        // If the interaction box is not visible (A.K.A. if the player is not in interaction range with a NPC.) do not start or continue dialog.
        if (!_isInInteractRange)
        {
            SetDialogSystemInvisible();
            return;
        }

        /*
        If the dialog box is not visible, set it visible and start the dialog.
        If there is more than one option for dialog, show the choice menu.
        If there is but one option for dialog, skip the choice menu and go straight to dialog.
        */
        if (!_dialogBox.visible)
        {
            _characterMovementScript.FreezeMovement(true, true);
            _isInInteraction = true;
            _interactBox.visible = false;

            _dialogBox.visible = true;
            _dialogBoxExitButton.SetEnabled(true);
            _dialogBoxExitButton.clickable.clickedWithEventInfo += ClickedDialogBoxExitButton;

            if (_dialogBuilder.GetAllDialogObjects().Count != 1)
            {
                ShowDialogChoices();
            }
            else
            {
                _choiceClicked = 0;
                SetDialogWithChoice();
                _currentTextNr = -1;
            }

            SetNpcCamera();
        }

        // If there is still dialog left (dialogTextList.Count - 1 because the list works upwards from 0) show next dialog line.
        if (_currentTextNr < (_dialogTextList.Count - 1))
        {
            _currentTextNr++;
            SetDialogBoxCharText(_npcName, _dialogTextList[_currentTextNr ?? default(int)]);
        }
        else
        {
            // If the option ends conversation, it sets the dialog box invisible and resets the dialogue choices and cameras.
            if (_chosenDialogOption.DoesOptionEndConversation())
            {
                TurnOffDialog();
                // add event 
            }
            else
            {
                // If the option does not end conversation, reset dialogue.
                ResetDialogue();

                // If the option does not end conversation ánd there is more than one dialog option, show choices.
                if (_dialogBuilder.GetAllDialogObjects().Count != 1)
                {
                    ShowDialogChoices();
                }
                else
                {
                    // If the option does not end conversation ánd there is but one dialog option, reset dialogue.
                    _choiceClicked = 0;
                    SetDialogWithChoice();
                    _currentTextNr = -1;
                }
            }
        }
    }

    private void TurnOffDialog()
    {
        _isInInteraction = false;
        _isDialogBuilderSet = false;

        _characterMovementScript.FreezeMovement(false, false);

        SetDialogSystemInvisible();
        ResetDialogue();
        UnsetDialogCamera();

        if (_npcCamera != null)
        {
            UnsetNpcCamera();
        }
        
        _dialogBuilder.SetEndedConversation(true);
        
        DialogExited?.Invoke();
    }

    // If a dialog choice button is clicked, set the following dialog to that choice.
    private void ClickedDialogBoxExitButton(EventBase tab)
    {
        TurnOffDialog();
    }

    // Show the dialog choices visual element.
    private void ShowDialogChoices()
    {
        _dialogBox.visible = true;
        _dialogBoxChoices.visible = true;

        ChangeFontDynamically();
        ChangeButtonFontDynamically();

        var counter = 0;
        foreach (var dialog in _dialogBuilder.GetAllDialogObjects())
        {
            _dialogBoxChoiceButtons[counter].visible = true;
            _dialogBoxChoiceButtons[counter].text = dialog.GetDialogChoice();
            _dialogBoxChoiceButtons[counter].SetEnabled(true);
            _dialogBoxChoiceButtons[counter].clickable.clickedWithEventInfo += ClickedDialogBoxChoiceButton;
            counter++;
        }
        SetDialogIntroText(_dialogBuilder.GetIntroText());
        UnsetDialogCamera();
    }

    // If a dialog choice button is clicked, set the following dialog to that choice.
    private void ClickedDialogBoxChoiceButton(EventBase tab)
    {
        _dialogBoxIntroText.visible = false;
        _dialogBoxChoices.visible = false;
        SetDialogBoxCharText(_npcName, _dialogTextList[_currentTextNr ?? default(int)]);
        foreach (var dialogButton in _dialogBoxChoiceButtons)
        {
            dialogButton.visible = false;
            dialogButton.SetEnabled(false);
        }
        var button = tab.target as Button;
        var buttonNr = Regex.Match(button.name, @"\d+").Value;
        _choiceClicked = Convert.ToInt32(buttonNr) - 1;
        SetDialogWithChoice();
        SetDialogCamera();
        _currentTextNr = 0;
    }

    // Reset the entire dialog.
    private void ResetDialogue()
    {
        _choiceClicked = null;
        _currentTextNr = null;
        _dialogBoxIntroText.text = " ";
        _dialogBoxText.text = " ";

        foreach (var dialogButton in _dialogBoxChoiceButtons)
        {
            dialogButton.text = " ";
            dialogButton.SetEnabled(false);
        }
    }

    // Set the intro dialog text.
    private void SetDialogIntroText(string text)
    {
        _dialogBoxDialog.visible = true;
        _dialogBoxIntroText.visible = true;
        _dialogBoxIntroText.text = text;
    }

    // Set the actual dialog text and character name.
    private void SetDialogBoxCharText(string charName, string text)
    {
        _dialogBoxDialog.visible = true;
        _dialogBoxCharName.text = charName;
        _dialogBoxText.text = text;
    }

    // Set the dialog builder from the NPC.
    public void SetDialogBuilder(DialogBuilder dialogBuilder)
    {
        if (_isDialogBuilderSet)
        {
            return;
        }

        _dialogBuilder = dialogBuilder;
        SetDialogWithChoice();
        var counter = 1;
        foreach (var dialog in _dialogBuilder.GetAllDialogObjects())
        {
            _dialogBoxChoiceButtons.Add(_root.Q<Button>("dialog-box-choice-button-" + counter));
            counter++;
        }
        foreach (var dialogButton in _dialogBoxChoiceButtons)
        {
            dialogButton.visible = false;
            dialogButton.SetEnabled(false);
        }

        ChangeButtonFontDynamically();

    }

    // Set the dialog of the choice the player clicked on.
    private void SetDialogWithChoice()
    {
        var dialogObjects = _dialogBuilder.GetAllDialogObjects();
        _chosenDialogOption = dialogObjects[_choiceClicked ?? default(int)];
        _dialogTextList = _chosenDialogOption.GetDialog();
        _npcName = _dialogBuilder.GetNameOfNpc();
        _npcCamera = _dialogBuilder.GetNpcCamera();

    }

    /* 
     * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
     *                                  HEALTH
     * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    */

    // Alter the health vignette based on the amount of damage the player got.
    private void AlterHealthVignette()
    {
        _healthVignette.style.unityBackgroundImageTintColor = new Color(Color.white.r, Color.white.g, Color.white.b, _healthController.GetVignetteTransparacy());
    }
    
    private void PlayerDies()
    {
        TurnOffDialog();
    }

    /* 
     * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
     *                                  JUMP CHARGE BAR
     * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    */

    // Charge the jump bar.
    private void ChargeJump()
    {
        _currentJumpCharge = _characterMovementScript.GetJumpCharged();
        _currentJumpCharge = Mathf.Clamp(_currentJumpCharge, _minJumpCharge, (_maxJumpCharge * _overchargeJumpModifier));
        _jumpChargePercent = _currentJumpCharge / _maxJumpCharge;
        _jumpChargeBar.value = _jumpChargePercent;
    }

    /* 
     * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
     *                                  SCREEN & FONTS
     * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    */

    // Check if the screen resolution changes.
    private void CheckForScreenResolutionChanges()
    {
        if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
        {
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;

            ChangeFontDynamically();
            ChangeButtonFontDynamically();
        }
    }

    // Change the font size so it looks/feels dynamic.
    private void ChangeFontDynamically()
    {
        // Full HD
        if (_lastScreenWidth == 1920 && _lastScreenHeight == 1080)
        {
            _dialogBoxCharName.style.fontSize = 35;
            _dialogBoxIntroText.style.fontSize = 50;
            _dialogBoxText.style.fontSize = 50;
        }
        // WXGA
        if (_lastScreenWidth == 1366 && _lastScreenHeight == 768)
        {
            _dialogBoxCharName.style.fontSize = 25;
            _dialogBoxIntroText.style.fontSize = 30;
            _dialogBoxText.style.fontSize = 30;
        }
        // QHD
        if (_lastScreenWidth == 2560 && _lastScreenHeight == 1440)
        {
            _dialogBoxCharName.style.fontSize = 50;
            _dialogBoxIntroText.style.fontSize = 60;
            _dialogBoxText.style.fontSize = 60;
        }
        // 4K UHD
        if (_lastScreenWidth == 3840 && _lastScreenHeight == 2160)
        {
            _dialogBoxCharName.style.fontSize = 70;
            _dialogBoxIntroText.style.fontSize = 80;
            _dialogBoxText.style.fontSize = 80;
        }
    }

    private void ChangeButtonFontDynamically()
    {
        if (_dialogBoxChoiceButtons != null)
        {
            foreach (var dialogButton in _dialogBoxChoiceButtons)
            {
                // Full HD
                if (_lastScreenWidth == 1920 && _lastScreenHeight == 1080)
                {
                    dialogButton.style.fontSize = 30;
                }
                // WXGA
                if (_lastScreenWidth == 1366 && _lastScreenHeight == 768)
                {
                    dialogButton.style.fontSize = 20;
                }
                // QHD
                if (_lastScreenWidth == 2560 && _lastScreenHeight == 1440)
                {
                    dialogButton.style.fontSize = 50;
                }
                // 4K UHD
                if (_lastScreenWidth == 3840 && _lastScreenHeight == 2160)
                {
                    dialogButton.style.fontSize = 70;
                }
            }
        }
    }

    /* 
     * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
     *                                  CAMERA'S
     * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    */

    private void SetNpcCamera()
    {
        if (_npcCamera != null && _npcCamera.Priority != (int)Camera.CameraState.Active)
        {
            _npcCamera.Priority = (int)Camera.CameraState.Active;   
        }
    }

    private void UnsetNpcCamera()
    {
        if (_npcCamera != null && _npcCamera.Priority != (int)Camera.CameraState.Inactive)
        {
            _npcCamera.Priority = (int)Camera.CameraState.Inactive;   
        }
    }

    private void SetDialogCamera()
    {
        _activeCamera = _chosenDialogOption.GetDialogCamera();
        if (_activeCamera != null)
        {
            _activeCamera.Priority = (int)Camera.CameraState.Active;
        }
    }

    private void UnsetDialogCamera()
    {
        if (_activeCamera != null)
        {
            _activeCamera.Priority = (int)Camera.CameraState.Inactive;
        }
    }
}
