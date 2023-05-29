using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{

    AnimatorManager animatorManager;

    Vector3 moveDirection;
    Transform cameraObject;
    CharacterController characterController;


    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;

    [Header("Ground Check and Falling Stuff")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;
    float currentGravity;

    [Header("Movement Speeds")]
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5f;
    public float sprintingSpeed = 7f;
    public float rotationSpeed =15f;

    Quaternion targetRotation;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animatorManager = GetComponent<AnimatorManager>();
        cameraObject = Camera.main.transform; 
    }

    public void HandleAllMovement(Vector2 inputVector, bool isInteracting)
    {
        if (isInteracting) return;

        // Prevent player movement when locked in animation interaction
        //if (isInteracting)
        //{
        //    // set animator moveAmount to 0
        //    animatorManager.UpdateAnimatorValues(0, 0, isSprinting);
        //    targetRotation = transform.rotation;
        //    return;
        //}

        //characterController.enabled = !isInteracting;



        HandleFallingAndLanding();
        HandleMovement(inputVector);
        HandleRotation(inputVector);
    }

    public void SetControl(bool hasControl)
    {
        Debug.Log("has Control " + hasControl);
        if (!hasControl)
        {
            // set animator moveAmount to 0
            animatorManager.UpdateAnimatorValues(0, 0, isSprinting);
            targetRotation = transform.rotation;
        }

        characterController.enabled = hasControl;
    }

    void HandleMovement(Vector2 inputVector)
    {

        //movement direction in front back direction
        moveDirection = cameraObject.forward * inputVector.y;
        //movement direction in right left direction
        moveDirection = moveDirection + cameraObject.right* inputVector.x;
        //Keep the direction but reduce magnitude btwn 0 and 1
        moveDirection.Normalize();
        // Dont want the player moving vertically upwards lmao
        moveDirection.y = currentGravity;

        if (isSprinting)
        {
            moveDirection *= sprintingSpeed;
        }
        else
        {
            // clamping horizontal and vertical value between 0 and 1 for it 
            // to make sense in animation blend tree
            var moveAmount = Mathf.Clamp01(Mathf.Abs(inputVector.x) + Mathf.Abs(inputVector.y));

            // TODO: Check if normalized approach is same as clamp Approach
            //if (inputVector.normalized.magnitude >= 0.5f)

            if (moveAmount>= 0.5f)
            {
                moveDirection *= runningSpeed;
            }
            else
            {
                moveDirection *= walkingSpeed;
            }
        }        

        Vector3 movementVelocity = moveDirection;
        characterController.Move(movementVelocity * Time.deltaTime);       
    }
    
    void HandleRotation(Vector2 inputVector)
    {
        if (isJumping) return;

        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward*inputVector.y;
        targetDirection = targetDirection + cameraObject.right * inputVector.x;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if(targetDirection == Vector3.zero)
        {
            targetDirection =  transform.forward;
        }

        targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed *Time.deltaTime);

        transform.rotation = playerRotation;
    }

    void HandleFallingAndLanding()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            currentGravity = -0.5f;
        }
        else
        {
            currentGravity += Physics.gravity.y * Time.deltaTime;
        }
    }

    // To Show Ground Check Sphere

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = new Color(0, 1, 0, 0.5f); 
    //    Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    //}
}
