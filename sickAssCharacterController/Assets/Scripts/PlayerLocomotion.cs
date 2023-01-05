using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    InputManager inputManager;
    Rigidbody rb;
    [SerializeField] Vector3 moveDirection;
    Transform cameraObject;

    [Header("Falling Settings")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float raycastHeightOffset = 0.5f;
    private float maxRaycastDistance = 0.5f;
    public LayerMask groundLayer;
    
    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;

    [Header("Movement Speeds")]
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5f;
    public float sprintingSpeed = 7f;
    public float rotationSpeed =15f;

    [Header("Jump Speeds")]
    public float jumpHeight = 3f;
    public float gravityIntensity = -15f;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>();  
        inputManager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform; 
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();

        // Prevent player movement when locked in animation interaction
        if (playerManager.isInteracting)
            return;

        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        if (isJumping) 
        {
            return; 
        } 

        //movement direction in front back direction
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        //movement direction in right left direction
        moveDirection = moveDirection + cameraObject.right* inputManager.horizontalInput;
        //Keep the direction but reduce magnitude btwn 0 and 1
        moveDirection.Normalize();
        // Dont want the player moving vertically upwards lmao
        moveDirection.y = 0;

        if (isSprinting)
        {
            moveDirection *= sprintingSpeed;
        }
        else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection *= runningSpeed;
            }
            else
            {
                moveDirection *= walkingSpeed;
            }
        }        

        if (isGrounded && !isJumping)
        {
            Vector3 movementVelocity = moveDirection;
            rb.velocity = movementVelocity;
        }
    }
    
    void HandleRotation()
    {
        if (isJumping) return;

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

        if (isGrounded && !isJumping)
        {
            transform.rotation = playerRotation;
        }
    }

    void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 raycastOrigin = transform.position;
        raycastOrigin.y += raycastHeightOffset;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }
            inAirTimer += Time.deltaTime;
            rb.AddForce(transform.forward * leapingVelocity);
            rb.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(raycastOrigin, 0.2f, -Vector3.up, out hit, maxRaycastDistance, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Land", true);
            }

            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump",false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            rb.velocity = playerVelocity;
        }
    }
}
