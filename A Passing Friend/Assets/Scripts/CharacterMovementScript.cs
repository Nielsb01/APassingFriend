using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class CharacterMovementScript : MonoBehaviour
{
    [SerializeField] private float acceleration = 0.8f;
    [SerializeField] private float deceleration = 1.6f;
    [SerializeField] private float moveSpeed = 1.75f;

    private CharacterController characterController;

    private float velocityY = 0.0f;
    private float velocityX = 0.0f;
    private float maxPositiveVelocity = 2.0f;
    private float maxNegativeVelocity = -2.0f;

    private Vector2 moveVector;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
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

            velocityX = (float)Math.Round(velocityX, 4);
            velocityY = (float)Math.Round(velocityY, 4);
            //Debug.Log("Vx: " + velocityX + " vY: " + velocityY);
            characterController.Move(transform.TransformDirection(new Vector3(moveSpeed * velocityX * Time.deltaTime, 0, moveSpeed * velocityY * Time.deltaTime)));
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
}
