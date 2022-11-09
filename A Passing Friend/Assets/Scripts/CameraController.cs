using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 _previousPosition;
    private Coroutine _camResetCoroutine;
    private CharacterController _characterController;
    public Transform playerLookingAt;
    public float maxCameraDistance = 10f;
    public float lookAroundSensitivity = 0.3f;
    public int resetSpeed = 500;
    public float snapToPlayerCamThreshold = 0.5f;
    private bool _focusPointIsLockedOnPlayer = false;

    public CinemachineVirtualCamera sideCamLeft;
    public CinemachineVirtualCamera sideCamRight;
    public CinemachineVirtualCamera tailCam;
    public CinemachineVirtualCamera dollyCam;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        StartCoroutine(EnforceCameraMaxDistanceFromPlayer());
    }

    private void FixedUpdate()
    {
        if (transform.position != _previousPosition)
        {
            if (_camResetCoroutine == null)
            {
                _camResetCoroutine = StartCoroutine(FloatFocusPointBackToPlayer());
            }
        }
    }

    public void LookAround(Vector2 lookVector)
    {
        _focusPointIsLockedOnPlayer = false;
        playerLookingAt.position +=
            new Vector3(0, lookVector.y * lookAroundSensitivity, lookVector.x * lookAroundSensitivity);

        if (_camResetCoroutine != null)
        {
            StopCoroutine(_camResetCoroutine);
            _camResetCoroutine = null;
        }
    }

    private IEnumerator EnforceCameraMaxDistanceFromPlayer()
    {
        while (true)
        {
            if (_focusPointIsLockedOnPlayer)
            {
                playerLookingAt.position = transform.position;
            }
            else
            {
                var cameraDistanceFromPlayer = (playerLookingAt.position - transform.position).magnitude;
                if (cameraDistanceFromPlayer > maxCameraDistance)
                {
                    playerLookingAt.position =
                        transform.position + 
                        (playerLookingAt.position - transform.position).normalized * maxCameraDistance;
                }
            }

            yield return null;
        }
    }

    private IEnumerator FloatFocusPointBackToPlayer()
    {
        while (true)
        {
            var cameraFocusPosition = playerLookingAt.position;
            var distanceBetweenCameraFocusAndPlayer = cameraFocusPosition - transform.position;
            var distanceDividedByResetSpeed = (distanceBetweenCameraFocusAndPlayer) / resetSpeed;
            var playerVelocity = _characterController.velocity.magnitude;
            if (playerVelocity < 1)
            {
                playerVelocity = 1;
            }

            var closingRateBetweenCamAndPlayer = distanceDividedByResetSpeed * playerVelocity;
            if (distanceBetweenCameraFocusAndPlayer.magnitude < snapToPlayerCamThreshold)
            {
                playerLookingAt.position -= transform.position;
                _focusPointIsLockedOnPlayer = true;
                yield break;
            }

            playerLookingAt.position = cameraFocusPosition - closingRateBetweenCamAndPlayer;

            yield return null;
        }
    }

    public void ActivateTailCam()
    {
        tailCam.Priority = 100;
    }

    public void DeactivateTailCam()
    {
        tailCam.Priority = 1;
    }

    public void ActivateDollyCam()
    {
        dollyCam.Priority = 100;
    }

    public void DeactivateDollyCam()
    {
        dollyCam.Priority = 1;
    }

    public void ActivateSideCamLeft()
    {
        sideCamLeft.Priority = 100;
    }
    public void ActivateSideCamRight()
    {
        sideCamRight.Priority = 100;
    }

    public void DeactivateSideCams()
    {
        sideCamLeft.Priority = 1;
        sideCamRight.Priority = 1;
    }
}