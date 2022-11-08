using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector3 _movement;
    private Vector2 _viewVector;
    private CharacterController _characterController;
    public float moveSpeed;
    public int gravity;
    public Transform playerLookingAt;
    public GameObject head;


    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        Vector3 movementVector =
            transform.TransformDirection(new Vector3(_movement.z * -1 * moveSpeed, -gravity,
                _movement.x * moveSpeed));
        _characterController.Move(movementVector);

        // head.transform.Rotate(
        //     new Vector3(_viewVector.y, _viewVector.x, 0));
        
        //
        // if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity))
        // {
        //     Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        //     Debug.Log("Did Hit");
        // }
        // else
        // {
        //     Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        //     Debug.Log("Did not Hit");
        //     
        // }

        
    }

    public void OnMove(InputValue moveVector)
    {
        var inputVector = moveVector.Get<Vector2>();
        _movement = new Vector3(inputVector.x, 0, inputVector.y);
        playerLookingAt.position = transform.position;
        MoveBody();
    }

    public void OnLook(InputValue lookVector)
    {
        var vec = lookVector.Get<Vector2>();
        var maxCameraDistance = 10;
        var cameraMovementSpeed = 0.3f;
        _viewVector = new Vector3(vec.x, vec.y);
        var temp = new Vector3(0, vec.y * cameraMovementSpeed, vec.x * cameraMovementSpeed);
        Debug.Log("x " + vec.x);
        Debug.Log("y " + vec.y);
        playerLookingAt.position = playerLookingAt.position + temp;
        if ((transform.position - playerLookingAt.position).magnitude > maxCameraDistance)
        {
            var distance = (playerLookingAt.position - transform.position).magnitude;
            var tooFarDistance = distance - maxCameraDistance;
            playerLookingAt.position = playerLookingAt.position.normalized * maxCameraDistance;
        }
    }

    private void MoveBody()
    {
        transform.Rotate(new Vector3(0, _viewVector.x, 0));
        FloatFocusPointBackToPlayer();
    }

    private void FloatFocusPointBackToPlayer()
    {
        var position = playerLookingAt.position;
        const int resetSpeed = 1000;
        const float snapToPlayerCamThreshold = 0.1f;
        var distance = position - transform.position;
        var distanceDividedByResetspeed =(distance) / resetSpeed;
        if (distanceDividedByResetspeed.magnitude < snapToPlayerCamThreshold)
        {
            position -= distance;
        }
        else
        {
            position -= distanceDividedByResetspeed;
        }
        playerLookingAt.position = position;
    }
}