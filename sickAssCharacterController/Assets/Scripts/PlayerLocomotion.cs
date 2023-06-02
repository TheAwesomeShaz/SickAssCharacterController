using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{

    AnimatorManager animatorManager;
    EnvironmentScanner envScanner;

    Vector3 moveDirection;
    Transform cameraObject;
    CharacterController characterController;


    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;
    [field: SerializeField] public bool IsOnLedge { get; set; }

    [Header("Ground Check and Falling Stuff")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float gravityForce = -10;
    [field: SerializeField] public LedgeHitData LedgeHitData { get; set; }

    float currentGravity;

    [Header("Movement Speeds")]
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5f;
    public float sprintingSpeed = 7f;
    [Tooltip("The amount of speed which will get added/subtracted every frame to increase/decrease speed smoothly")]
    [SerializeField] float movementSpeedDampingValue = 0.5f;

    [field:SerializeField] public float RotationSpeed { get;private set; } = 15f;

    Quaternion targetRotation;
    [SerializeField] float currentSpeed;


    private void Start()
    {
        
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animatorManager = GetComponent<AnimatorManager>();
        envScanner = GetComponent<EnvironmentScanner>();    
        cameraObject = Camera.main.transform;

        animatorManager.OnSetIsOnLedge += (value) => IsOnLedge = value;

    }

    public void HandleAllMovement(Vector2 inputVector, bool isInteracting)
    {
        // Prevent player movement when locked in animation interaction
        if (isInteracting) return;

       
        HandleFallingAndLanding(isInteracting);
        HandleMovement(inputVector);
        HandleRotation(inputVector);
    }

    public void SetControl(bool hasControl)
    {
        Debug.Log(hasControl);
        if (!hasControl)
        {
            // set animator moveAmount to 0
            animatorManager.UpdateAnimatorValues(0, 0, isSprinting);
            targetRotation = transform.rotation;
        }

        // speed should go down to zero when player doesnt have control
        // but it should only happen when not climbing so not the thing below
        // currentSpeed = hasControl ? currentSpeed : 0f;

        characterController.enabled = hasControl;
    }

    //TODO: imrpove this mess of animation event functions later
    public void SetControlTrue()
    {
        SetControl(true);
    }
    public void SetControlFalse()
    {
        SetControl(false);
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


        Vector3 movementVelocity = Vector3.zero;

        SetMovementSpeed(inputVector);
        //Debug.Log(currentSpeed);

        if (isGrounded)
        {
            movementVelocity = moveDirection * currentSpeed;
        }
        else
        {
            // move forward while jumping, forward jumpinf speed is currentSpeed/2
            Vector3 forwardJumpForce = transform.forward * currentSpeed / 1.2f;
            Vector3 downwardGravityForce = Vector3.up * moveDirection.y;

            movementVelocity = forwardJumpForce+downwardGravityForce;
        }
        
        characterController.Move(movementVelocity * Time.deltaTime);       
    }

    void HandleRotation(Vector2 inputVector)
    {

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
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed *Time.deltaTime);

        transform.rotation = playerRotation;
    }

    void HandleFallingAndLanding(bool isInteracting)
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);

        animatorManager.SetAnimatorBool(animatorManager.IsGroundedHash, isGrounded);

        // Root motion should be false when not grounded in free fall state
        // since we need to apply gravity by ourselves and not by the animation
        animatorManager.animator.applyRootMotion = !isInteracting && isGrounded;

        if (isGrounded)
        {
            currentGravity = -0.5f;
            IsOnLedge = envScanner.LedgeCheck(moveDirection,out LedgeHitData ledgeHitData);              

            if(IsOnLedge) 
            {
                LedgeHitData = ledgeHitData; 
            }
        }
        else
        {
            currentGravity += gravityForce * Time.deltaTime;
            //Debug.Log("Applying Gravity");
        }
    }

    //TODO:limit ledge movement
    void HandleLedgeMovement()
    {
        float ledgeNormalMoveAngle = Vector3.Angle(LedgeHitData.ledgeFaceHit.normal, moveDirection);

        if(ledgeNormalMoveAngle < 90)
        {

        }
    }
    
    void SetMovementSpeed(Vector2 inputVector)
    {
        if (isSprinting)
        {
            //currentSpeed = sprintingSpeed;
            
            // for smoothly increasing speed to sprinting
            if (currentSpeed < sprintingSpeed)
            {
                currentSpeed+=movementSpeedDampingValue;
            }
        }
        else
        {
            // clamping horizontal and vertical value between 0 and 1 for it 
            // to make sense in animation blend tree
            var moveAmount = Mathf.Clamp01(Mathf.Abs(inputVector.x) + Mathf.Abs(inputVector.y));

            // TODO: Check if normalized approach is same as clamp Approach
            //if (inputVector.normalized.magnitude >= 0.5f)

            if (moveAmount >= 0.5f)
            {

                //currentSpeed = runningSpeed;

                // for smoothly decreasing speed from sprinting to running
                if (currentSpeed > runningSpeed)
                {
                    currentSpeed-=movementSpeedDampingValue;
                }
                // for smoothly increasing speed from walking to running
                else if (currentSpeed < runningSpeed)
                {
                    currentSpeed+=movementSpeedDampingValue;
                }
            }
            else
            {
                //currentSpeed = walkingSpeed;

                // for smoothly decreasing speed to walking
                if(currentSpeed > walkingSpeed)
                {
                    currentSpeed-=movementSpeedDampingValue;
                }
                // immediately set to walking speed or else it wont feel responsive
                else
                {
                    currentSpeed = walkingSpeed;
                }
            }
        }
    }

    // To Show Ground Check Sphere

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = new Color(0, 1, 0, 0.5f);
    //    Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    //}
}
