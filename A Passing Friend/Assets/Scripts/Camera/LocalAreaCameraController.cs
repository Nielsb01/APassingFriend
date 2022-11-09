using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LocalAreaCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    
    public void ActivateLocalCam(Transform followTarget)
    {
        virtualCamera.Priority = 100;
        virtualCamera.Follow = followTarget;
        virtualCamera.LookAt = followTarget;
    }

    public void DeactivateLocalCam()
    {
        virtualCamera.Priority = 1;
    }
}
