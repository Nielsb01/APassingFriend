using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera faceCam;
        [SerializeField] private CinemachineVirtualCamera tailCam;
        [SerializeField] private CinemachineVirtualCamera pointOfViewCam;
        [SerializeField] private CinemachineFreeLook freeLookCam;

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
        public void ActivatePovCam()
        {
            CamErrorHandler.ThrowErrorIfCamIsNotSet(pointOfViewCam);
            pointOfViewCam.Priority = (int)CameraState.Active;
        }

        public void DeactivatePovCam()
        {
            pointOfViewCam.Priority = (int)CameraState.Inactive;
        }


        public void OnFreeLook(InputValue value)
        {
            if (value.isPressed)
            {
                freeLookCam.GetComponent<CinemachineInputProvider>().enabled = true;
                freeLookCam.m_YAxisRecentering.m_enabled = false;
                freeLookCam.m_RecenterToTargetHeading.m_enabled = false;
            }
            else
            {
                freeLookCam.GetComponent<CinemachineInputProvider>().enabled = false;
                freeLookCam.m_YAxisRecentering.m_enabled = true;
                freeLookCam.m_RecenterToTargetHeading.m_enabled = true;
                freeLookCam.m_YAxisRecentering.RecenterNow();
                freeLookCam.m_RecenterToTargetHeading.RecenterNow();
            }
        }
    }
}