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
            virtualCamera.Priority = 100;
            virtualCamera.Follow = followTarget;
            virtualCamera.LookAt = followTarget;
        }

        public void DeactivateLocalCam()
        {
            virtualCamera.Priority = 1;
        }
    }
}