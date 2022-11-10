using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static UnityEngine.GraphicsBuffer;

public class CharacterMovementScript : MonoBehaviour
{
    [SerializeField] private float acceleration = 0.8f;
    [SerializeField] private float deceleration = 1.6f;
    [SerializeField] private float moveSpeed = 1.75f;
    [SerializeField] private float jumpSpeed = 4.5f;
    [SerializeField] private float gravity = 9.81f;

    private CharacterController characterController;
    private new Rigidbody rigidbody;

    private float velocityY = 0.0f;
    private float velocityX = 0.0f;
    private float maxPositiveVelocity = 2.0f;
    private float maxNegativeVelocity = -2.0f;
    
    private Vector2 moveVector;
    private Vector3 moveDirection = Vector3.zero;
    private bool doJump;

    void Start()
    {
        doJump = false;
        characterController = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (doJump)
        {
            moveDirection.y = jumpSpeed;
            doJump = false;
        }

        if (moveVector != null)
        {
            if (velocityY < maxPositiveVelocity && moveVector.y > 0)
            {
                velocityY += Time.deltaTime * acceleration;
            }
            else if (velocityY > 0)
            {
                velocityY -= Time.deltaTime * deceleration;
            }

            if (velocityY > maxNegativeVelocity && moveVector.y < 0)
            {
                velocityY -= Time.deltaTime * acceleration;
            }
            else if (velocityY < 0)
            {
                velocityY += Time.deltaTime * deceleration;
                if (floatIsBetween(velocityY, 0, 0.01f))
                {
                    velocityY = 0;
                }
            }

            if (velocityX < maxPositiveVelocity && moveVector.x > 0)
            {
                velocityX += Time.deltaTime * acceleration;
            }
            else if (velocityX > 0)
            {
                velocityX -= Time.deltaTime * deceleration;
            }

            if (velocityX > maxNegativeVelocity && moveVector.x < 0)
            {
                velocityX -= Time.deltaTime * acceleration;
            }
            else if (velocityX < 0)
            {
                velocityX += Time.deltaTime * deceleration;
                if (floatIsBetween(velocityX,0,0.01f))
                {
                    velocityX = 0;
                }
            }

            moveDirection.x = moveSpeed * ((float)Math.Round(velocityX, 4));
            moveDirection.z = moveSpeed * ((float)Math.Round(velocityY, 4));

            moveDirection.y -= gravity * Time.deltaTime;
            characterController.Move(transform.TransformDirection(moveDirection * Time.deltaTime));
        }
    }

    private static bool floatIsBetween(float number, float min, float max)
    {
        return number >= min && number <= max;
    }


    private void OnMove(InputValue inputValue)
    {
        moveVector = inputValue.Get<Vector2>();
    }

    private void OnJump()
    {
        if (characterController.isGrounded)
        {
            doJump = true;
        }
    }
}
