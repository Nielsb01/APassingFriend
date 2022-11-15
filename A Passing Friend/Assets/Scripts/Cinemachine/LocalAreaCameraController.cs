using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class LocalAreaCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        public void ActivateLocalCam(Transform followTarget)
        {
            CamErrorHandler.ThrowErrorIfCamIsNotSet(virtualCamera);
            virtualCamera.Priority = (int)CameraState.Active;
            virtualCamera.Follow = followTarget;
            virtualCamera.LookAt = followTarget;
        }

        public void DeactivateLocalCam()
        {
            virtualCamera.Priority = (int)CameraState.Inactive;
        }
    }
}