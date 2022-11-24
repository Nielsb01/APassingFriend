using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _faceCam;
        [SerializeField] private CinemachineVirtualCamera _tailCam;
        [SerializeField] private CinemachineVirtualCamera _pointOfViewCam;
        [SerializeField] private CinemachineFreeLook _freeLookCam;
        private CharacterMovementScript _characterMovementScript;

        private void Start()
        {
            _characterMovementScript = GetComponent<CharacterMovementScript>();
        }

        public void ActivateTailCam()
        {
            CamErrorHandler.ThrowErrorIfCamIsNotSet(_tailCam);
            _tailCam.Priority = (int)CameraState.Active;
        }

        public void DeactivateTailCam()
        {
            _tailCam.Priority = (int)CameraState.Inactive;
        }

        public void ActivateFaceCam()
        {
            CamErrorHandler.ThrowErrorIfCamIsNotSet(_tailCam);
            _faceCam.Priority = (int)CameraState.Active;
        }

        public void DeactivateFaceCam()
        {
            _faceCam.Priority = (int)CameraState.Inactive;
        }

        public void ActivatePovCam()
        {
            CamErrorHandler.ThrowErrorIfCamIsNotSet(_pointOfViewCam);
            _pointOfViewCam.Priority = (int)CameraState.Active;
        }

        public void DeactivatePovCam()
        {
            _pointOfViewCam.Priority = (int)CameraState.Inactive;
        }


        public void OnFreeLook(InputValue value)
        {
            if (value.isPressed)
            {
                _freeLookCam.GetComponent<CinemachineInputProvider>().enabled = true;
                _freeLookCam.m_YAxisRecentering.m_enabled = false;
                _freeLookCam.m_RecenterToTargetHeading.m_enabled = false;
            }
            else
            {
                _freeLookCam.GetComponent<CinemachineInputProvider>().enabled = false;
                _freeLookCam.m_YAxisRecentering.m_enabled = true;
                _freeLookCam.m_RecenterToTargetHeading.m_enabled = true;
                _freeLookCam.m_YAxisRecentering.RecenterNow();
                _freeLookCam.m_RecenterToTargetHeading.RecenterNow();
            }
        }

        public void LockOrientation(float orientation)
        {
            var newRotation = transform.eulerAngles;
            newRotation.y = orientation;
            transform.Rotate(newRotation);
            _characterMovementScript.rotationFrozenDueToSpecialArea = true;
        }

        public void UnlockOrientation()
        {
            _characterMovementScript.rotationFrozenDueToSpecialArea = false;
        }
    }
}