using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSound : MonoBehaviour
{
    [Header("FMOD Settings")]
    [SerializeField] private FMODUnity.EventReference FootstepsEventPath;
    [SerializeField] private FMODUnity.EventReference JumpingEventPath;
    [SerializeField] private FMODUnity.EventReference LandingEventPath;
    [SerializeField] private string MaterialParameterName;                      // This contains which material the player is currently walking on.
    [SerializeField] private string SpeedParameterName;                         // This contains which footstep speed needs to be heard.
    [SerializeField] private string JumpOrLandParameterName;                    // This controlls whether or not a jumping or a landing sound needs to be heard.
    [Header("Playback Settings")]
    [SerializeField] private float StepDistance = 2.0f;                         // Minimum displacement required for emitting sound.
    [SerializeField] private float RayDistance = 1.2f;                          // Raycast maximum distance
    [SerializeField] private float StartRunningTime = 0.3f;                     // Set a tmie. If the time between each step the player takes is less than this value, the player will start to hear running footsteps.
    [SerializeField] private string JumpInputName = "Jump";                              // Contains the jumping event name set using the new input system.
    public string[] MaterialTypes;                                              // This is an array of strings. In the inspector we can decide how many Material types we have in FMOD by setting the size of this array. Depending on the size, the array will then create a certain amount of strings for us to fill in with the name of each of our footstep materials for our scripts to use. This will then remain a constant and will not change.
    //[HideInInspector] public int DefulatMaterialValue;                          // 


    //These variables are used to control when the player executes a footstep.
    private float StepRandom;                                                   // This will be set as random number, which will later be added to the StepDistance to add a little variaiton to the length in steps.
    private Vector3 PrevPos;                                                    // This will old the co-ordinates of the previous postion the player was in during the last frame.
    private float DistanceTravelled;                                            // This will hold a value that how represnt how far the player has travlled since they last took a step.
    //These variables are used when checking the Material type the player is on top of.
    private RaycastHit hit;                                                     // Will holds information about the GameObject that the raycast hits.
    private int F_MaterialValue;                                                // We'll use this to set the value of our FMOD Material Parameter.
    //These booleans will hold values that tell us if the player is touching the ground currently and if they were touching it during the last frame.
    private bool PlayerTouchingGround;                                          // This will hold a value to represent whether or not the player is touching the ground. True = Grounded, False = Not Grounded.
    private bool PreviosulyTouchingGround;                                      // This will hold a value to represent whether or not the player was touching the ground during the last frame. True = Was Grounded, False = Wasn't Grounded.
    //These floats help us determine when the player should be running.
    private float TimeTakenSinceStep;                                           // We'll use this as a timer, to track the time it takes between each step.
    private int F_PlayerRunning;                                                // We'll use to set the value of our FMOD Switch Speed Parameter.

    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("HandleSound", 0, 750);
    }

    private void OnMove(InputValue inputValue)
    {
        Vector2 moveVector = inputValue.Get<Vector2>();
        Debug.Log("move");
    }

    private void OnJump()
    {
        StartCoroutine("HandleJumpingSound", 0);
    }

    private void HandleMovementSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot(FootstepsEventPath);
    }

    private void HandleJumpingSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot(JumpingEventPath);
        
        FMODUnity.RuntimeManager.PlayOneShot(LandingEventPath);

    }
}
