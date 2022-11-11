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
            tailCam.Priority = 100;
        }

        public void DeactivateTailCam()
        {
            tailCam.Priority = 1;
        }

        public void ActivateFaceCam()
        {
            CamErrorHandler.ThrowErrorIfCamIsNotSet(tailCam);
            faceCam.Priority = 100;
        }

        public void DeactivateFaceCam()
        {
            faceCam.Priority = 1;
        }
    }
}