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

        public void LockOrientation(float targetOrientation, bool allowMirroredDirectionLock)
        {
            _characterMovementScript.rotationFrozenDueToSpecialArea = true;
            if (!allowMirroredDirectionLock)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, targetOrientation, 0));
                return;
            }

            var playerOrientation = transform.rotation.eulerAngles.y;
            if (playerOrientation is > 90 and < 270 && targetOrientation is > 90 and < 270)
            {
                if (playerOrientation - targetOrientation < 90)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, targetOrientation, 0));
                }
                else
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, targetOrientation - 180, 0));
                }
            }
            else
            {
                if (playerOrientation - targetOrientation > 270 || playerOrientation - targetOrientation < 90)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, targetOrientation, 0));
                }
                else
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, targetOrientation - 180, 0));
                }
            }
        }

        public void UnlockOrientation()
        {
            _characterMovementScript.rotationFrozenDueToSpecialArea = false;
        }
    }
}