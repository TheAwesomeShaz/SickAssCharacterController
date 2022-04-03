using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;
    Rigidbody rb;
    Vector3 moveDirection;
    Transform cameraObject;

    public float movementSpeed = 7f;
    private float rotationSpeed =15f;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform; 
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        //movement direction in front back direction
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        //movement direction in right left direction
        moveDirection = moveDirection + cameraObject.right* inputManager.horizontalInput;
        //Keep the direction but reduce magnitude btwn 0 and 1
        moveDirection.Normalize();
        // Dont want the player moving vertically upwards lmao
        moveDirection.y = 0;

        Vector3 movementVelocity = moveDirection*movementSpeed;
        rb.velocity = movementVelocity;
    }
    
    void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward*inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if(targetDirection == Vector3.zero)
        {
            targetDirection =  transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed *Time.deltaTime);

        transform.rotation = playerRotation;
    }
}
