using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class LocalAreaCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;

        public void ActivateLocalCam(Transform followTarget)
        {
            CamErrorHandler.ThrowErrorIfCamIsNotSet(_virtualCamera);
            _virtualCamera.Priority = (int)CameraState.Active;
            _virtualCamera.Follow = followTarget;
            _virtualCamera.LookAt = followTarget;
        }

        public void DeactivateLocalCam()
        {
            _virtualCamera.Priority = (int)CameraState.Inactive;
        }
    }
}