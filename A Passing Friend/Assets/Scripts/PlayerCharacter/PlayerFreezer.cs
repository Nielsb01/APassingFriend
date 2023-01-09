using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerFreezer
{
    public delegate void PlayerFreezeEvent(bool freezeActive);

    public static event PlayerFreezeEvent FreezeMovementEvent;
    public static event PlayerFreezeEvent FreezeCameraEvent;
    public static event PlayerFreezeEvent FreezeInputManager;

    public static void FreezeMovement()
    {
        FreezeMovementEvent?.Invoke(true);
    }

    public static void ReleaseMovementFreeze()
    {
        FreezeMovementEvent?.Invoke(false);
    }

    public static void FreezeRotation()
    {
        FreezeCameraEvent?.Invoke(true);
    }

    public static void ReleaseRotationFreeze()
    {
        FreezeCameraEvent?.Invoke(false);
    }

    public static void FreezeAllInput()
    {
        FreezeInputManager?.Invoke(true);
    }

    public static void ReleaseAllInputFreeze()
    {
        FreezeInputManager?.Invoke(false);
    }
}