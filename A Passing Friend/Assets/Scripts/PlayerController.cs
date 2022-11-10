using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;

    private UIController ui;

    private Vector3 movement;
    [Range(0, 25)] public float speed;

    private float interactRayDistance = 2.5f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        ui = GameObject.Find("UIDocument").GetComponent<UIController>();
        speed = 5.0f;
    }

    private void FixedUpdate()
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

    private void OnInteract()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.DrawRay(transform.position, Vector3.forward * interactRayDistance);

        if (Physics.Raycast(ray, out hit, interactRayDistance))
        {
            if (hit.transform.gameObject.CompareTag("NPC"))
            {
                ui.ContinueDialog();
            }
        }
    }
}
