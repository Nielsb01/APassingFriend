using System;
using Cinemachine;

namespace Camera
{
    public static class CamErrorHandler
    {
        public static void ThrowErrorIfCamIsNotSet(CinemachineVirtualCamera cam)
        {
            if (cam == null)
            {
                throw new NullReferenceException(
                    "The camera you are trying to use can not be found. The reference may be missing or incorrect.");
            }
        }
    }
}