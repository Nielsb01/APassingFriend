using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;

    private Vector3 movement;
    [Range(0, 25)] public float speed = 5.0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        speed = 5.0f;
    }

    void FixedUpdate()
    {
        Vector3 playerMovementDirectionFoward = transform.forward * movement.z * speed * Time.fixedDeltaTime;
        Vector3 playerMovementDirectionSide = transform.right * movement.x * speed * Time.fixedDeltaTime;
        playerMovementDirectionFoward.y = 0;

        characterController.Move(playerMovementDirectionFoward + playerMovementDirectionSide);
    }

    private void OnMove(InputValue movementValue)
    {
        var movementY = movementValue.Get<Vector2>().y;
        var movementX = movementValue.Get<Vector2>().x;

        movement = new Vector3(movementX, 0.0f, movementY);
    }
}
