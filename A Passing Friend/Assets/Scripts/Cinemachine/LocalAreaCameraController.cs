using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class LocalAreaCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private float _lockOrientation = -1;
        [SerializeField] private bool _allowMirroredDirectionLock = true;
        private CameraController _cameraController;
        

        public void ActivateLocalCam(Transform followTarget, CameraController cameraController)
        {
            CamErrorHandler.ThrowErrorIfCamIsNotSet(_virtualCamera);
            _virtualCamera.Priority = (int)CameraState.Active;
            _virtualCamera.Follow = followTarget;
            _virtualCamera.LookAt = followTarget;
            _cameraController = cameraController;
            if (_lockOrientation > -1)
            {
                _cameraController.LockOrientation(_lockOrientation, _allowMirroredDirectionLock);
            }
        }

        public void DeactivateLocalCam()
        {
            _virtualCamera.Priority = (int)CameraState.Inactive;
            _cameraController.UnlockOrientation();
        }
    }
}