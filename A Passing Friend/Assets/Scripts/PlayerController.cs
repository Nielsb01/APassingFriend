using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private Vector3 _movement;
    private Vector2 _turn;
    private Vector2 _viewVector;
    private CharacterController _characterController;
    public float moveSpeed = 0.3f;
    public float gravity = 0.9f;
    public Transform playerLookingAt;
    public float maxCameraDistance = 10f;
    public float lookAroundSensitivity = 0.3f;
    public int resetSpeed = 1000;
    public float snapToPlayerCamThreshold = 0.1f;
    private Coroutine _camResetCoroutine;


    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        StartCoroutine(EnforceCameraMaxDistanceFromPlayer());
    }

    private void FixedUpdate()
    {
        Vector3 movementVector =
            transform.TransformDirection(new Vector3(_movement.z * -1 * moveSpeed, -gravity,
                _movement.x * moveSpeed));
        _characterController.Move(movementVector);
        transform.Rotate(_turn);
        if (_movement.z != 0 || _movement.x != 0)
        {
            if (_viewVector.x == 0 && _viewVector.y == 0)
            {
                if (_camResetCoroutine == null)
                {
                    _camResetCoroutine = StartCoroutine(FloatFocusPointBackToPlayer());
                }
            }
        }
    }

    public void OnMove(InputValue moveVector)
    {
        var inputVector = moveVector.Get<Vector2>();
        _movement = new Vector3(inputVector.x, 0, inputVector.y);
    }


    public void OnTurn(InputValue moveVector)
    {
        var inputVector = moveVector.Get<Vector2>();
        _turn = new Vector3(0, inputVector.x, 0);
    }

    public void OnLook(InputValue lookVector)
    {
        LookAround(lookVector.Get<Vector2>());
    }

    private void LookAround(Vector2 lookVector)
    {
        _viewVector = new Vector3(lookVector.x, lookVector.y);
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
            var cameraDistanceFromPlayer = (playerLookingAt.position - transform.position).magnitude;
            if (cameraDistanceFromPlayer > maxCameraDistance)
            {
                playerLookingAt.position =
                    transform.position + (playerLookingAt.position - transform.position).normalized * maxCameraDistance;
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

            var closingNumber = distanceDividedByResetSpeed * playerVelocity;
            if (distanceBetweenCameraFocusAndPlayer.magnitude < snapToPlayerCamThreshold)
            {
                cameraFocusPosition -= distanceBetweenCameraFocusAndPlayer;
                yield break;
            }
            else
            {
                cameraFocusPosition -= closingNumber;
            }

            playerLookingAt.position = cameraFocusPosition;

            yield return null;
        }
    }
}