using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera sideCamLeft;
        [SerializeField] private CinemachineVirtualCamera sideCamRight;
        [SerializeField] private CinemachineVirtualCamera tailCam;

        public void ActivateTailCam()
        {
            CamErrorHandler.ThrowErrorIfCamIsNotSet(tailCam);
            tailCam.Priority = 100;
        }

        public void DeactivateTailCam()
        {
            tailCam.Priority = 1;
        }

        public void ActivateSideCamLeft()
        {
            CamErrorHandler.ThrowErrorIfCamIsNotSet(sideCamLeft);
            sideCamLeft.Priority = 100;
        }

        public void ActivateSideCamRight()
        {
            CamErrorHandler.ThrowErrorIfCamIsNotSet(sideCamRight);
            sideCamRight.Priority = 100;
        }

        public void DeactivateSideCams()
        {
            sideCamLeft.Priority = 1;
            sideCamRight.Priority = 1;
        }
    }
}