using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;

    private UIController _ui;

    private Vector3 _movement;

    [Range(0, 25)]
    private float _speed;

    private float _interactRayDistance = 2.5f;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _ui = GameObject.Find("UIDocument").GetComponent<UIController>();
        _speed = 5.0f;
    }

    private void FixedUpdate()
    {
        Vector3 playerMovementDirectionFoward = transform.forward * _movement.z * _speed * Time.fixedDeltaTime;
        Vector3 playerMovementDirectionSide = transform.right * _movement.x * _speed * Time.fixedDeltaTime;
        playerMovementDirectionFoward.y = 0;

        _characterController.Move(playerMovementDirectionFoward + playerMovementDirectionSide);
    }

    private void OnMove(InputValue movementValue)
    {
        var movementY = movementValue.Get<Vector2>().y;
        var movementX = movementValue.Get<Vector2>().x;

        _movement = new Vector3(movementX, 0.0f, movementY);
    }

    private void OnInteract()
    {
        /* Instead of this method, you can also use the CheckSphere method in the NPCBehaviour script to check if the player is in interact range.
           Preferably only use 1 method, but both also works. */
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.DrawRay(transform.position, Vector3.forward * _interactRayDistance);

        if (Physics.Raycast(ray, out hit, _interactRayDistance))
        {
            if (hit.transform.gameObject.CompareTag("NPC"))
            {
                _ui.ContinueDialog();
            }
        }
    }
}
