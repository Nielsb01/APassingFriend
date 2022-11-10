using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        public CinemachineVirtualCamera sideCamLeft;
        public CinemachineVirtualCamera sideCamRight;
        public CinemachineVirtualCamera tailCam;

        public void ActivateTailCam()
        {
            tailCam.Priority = 100;
        }

        public void DeactivateTailCam()
        {
            tailCam.Priority = 1;
        }
        
        public void ActivateSideCamLeft()
        {
            sideCamLeft.Priority = 100;
        }

        public void ActivateSideCamRight()
        {
            sideCamRight.Priority = 100;
        }

        public void DeactivateSideCams()
        {
            sideCamLeft.Priority = 1;
            sideCamRight.Priority = 1;
        }
    }
}