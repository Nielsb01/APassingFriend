using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera faceCam;
        [SerializeField] private CinemachineVirtualCamera tailCam;

        public void ActivateTailCam()
        {
            CamErrorHandler.ThrowErrorIfCamIsNotSet(tailCam);
            tailCam.Priority = (int)CameraState.Active;
        }

        public void DeactivateTailCam()
        {
            tailCam.Priority = (int)CameraState.Inactive;
        }

        public void ActivateFaceCam()
        {
            CamErrorHandler.ThrowErrorIfCamIsNotSet(tailCam);
            faceCam.Priority = (int)CameraState.Active;
        }

        public void DeactivateFaceCam()
        {
            faceCam.Priority = (int)CameraState.Inactive;
        }
    }
}