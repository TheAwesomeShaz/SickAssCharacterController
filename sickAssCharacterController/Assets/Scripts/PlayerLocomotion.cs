using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{

    AnimatorManager animatorManager;
    EnvironmentScanner envScanner;

    Transform cameraObject;
    CharacterController characterController;


    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;
    [field: SerializeField] public bool IsOnLedge { get; set; }

    [SerializeField] bool isClimbingLadderUp = false;
    [SerializeField] bool isClimbingLadderDown = false;
    [SerializeField] bool isClimbingLadderIdle = false;

    private LadderHitData ladderHitData;

    [field: SerializeField] public bool IsOnLadder { get; set; }
    [field: SerializeField] public bool IsHanging { get; set; }

    [Header("Ground Check and Falling Stuff")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float gravityForce = -10;
    [field: SerializeField] public EdgeHitData LedgeHitData { get; set; }

    float currentGravity;

    [Header("Movement Speeds")]
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5f;
    public float sprintingSpeed = 7f;
    public float ladderClimbingSpeed = 0.5f;

    [Tooltip("The amount of speed which will get added/subtracted every frame to increase/decrease speed smoothly")]
    [SerializeField] float movementSpeedDampingValue = 0.5f;

    [field:SerializeField] public float RotationSpeed { get;private set; } = 15f;

    Vector3 desiredMoveDirection;
    Vector3 moveDirection;
    [SerializeField] Vector3 movementVelocity;
    Quaternion targetRotation;
    [SerializeField] float currentSpeed;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animatorManager = GetComponent<AnimatorManager>();
        envScanner = GetComponent<EnvironmentScanner>();    
        cameraObject = Camera.main.transform;

        animatorManager.OnSetIsOnLedge += (value) => IsOnLedge = value;
        animatorManager.OnResetSpeed += (resetSpeed) => currentSpeed = resetSpeed ? 0 : currentSpeed;
        animatorManager.OnSetIsHanging += (value) => IsHanging = value;
        animatorManager.OnSetPlayerControl += (value) => SetControl(value);
        animatorManager.OnSetIsOnLadder += (value) => { IsOnLadder = value; SetAllLadderClimbingFalse(); };

    }

    public void HandleAllMovement(Vector2 inputVector, bool isInteracting, bool highProfileInput)
    {
        // Prevent player movement when locked in animation interaction
        if (isInteracting) return;
        if (IsHanging) return;

        SetMovementDirection(inputVector,highProfileInput);

        HandleFallingAndLanding();

        if (IsOnLadder && !isInteracting)
        {
            HandleLadderMovement(inputVector);
            return;
        }


        HandleMovement(inputVector,highProfileInput);
        SetMovementSpeed(inputVector);


        HandleRotation(inputVector);
    }

    public void SetControl(bool hasControl)
    {
        if (!hasControl)
        {
            // set animator moveAmount to 0
            animatorManager.UpdateAnimatorValues(0, 0, isSprinting);
            targetRotation = transform.rotation;
        }

        characterController.enabled = hasControl;
    }

    private void SetMovementDirection(Vector2 inputVector,bool highProfileInput)
    {

        var moveAmount = Mathf.Clamp01(Mathf.Abs(inputVector.x) + Mathf.Abs(inputVector.y));
        
        this.isSprinting = highProfileInput&&moveAmount>0.5f;

        //movement direction in front back direction
        desiredMoveDirection = cameraObject.forward * inputVector.y;
        //movement direction in right left direction
        desiredMoveDirection = desiredMoveDirection + cameraObject.right * inputVector.x;
        //Keep the direction but reduce magnitude btwn 0 and 1
        desiredMoveDirection.Normalize();
        // Dont want the player moving vertically upwards lmao
        desiredMoveDirection.y = currentGravity;

        moveDirection = desiredMoveDirection;
    }
     
    void HandleMovement(Vector2 inputVector,bool highProfileinput)
    {
        if (isGrounded)
        {
            movementVelocity = moveDirection * currentSpeed;
        }

        //TODO: Move this into else block into free fall state
        else
        {
            // move forward while jumping, forward jumping speed is currentSpeed/2
            Vector3 forwardJumpForce = transform.forward * currentSpeed / 1.2f;
            Vector3 downwardGravityForce = Vector3.up * moveDirection.y;

            movementVelocity = forwardJumpForce + downwardGravityForce;
        }
        Vector3 moveAmountVelocity = movementVelocity;

        if (!envScanner.ObstacleCheck().forwardHitFound)
        {
            characterController.Move(movementVelocity * Time.deltaTime);
        }
        else
        {
            moveAmountVelocity.x = 0;
            moveAmountVelocity.z = 0;
        }

        if (!IsOnLadder)
        {
            moveAmountVelocity.y = 0;
        }


        var moveAmount = Mathf.Clamp(moveAmountVelocity.magnitude / currentSpeed, 0, 2);
        this.isSprinting = movementVelocity.x != 0 && highProfileinput && !envScanner.ObstacleCheck().forwardHitFound;

        //TODO: Add this in the States wherever Movement Takes Place? or just straight up in the Grounded State?
        animatorManager.UpdateAnimatorValues(0, moveAmount, isSprinting);

    }

    void HandleRotation(Vector2 inputVector)
    {
            Vector3 targetDirection = moveDirection;

            //targetDirection = cameraObject.forward*inputVector.y;
            //targetDirection = targetDirection + cameraObject.right * inputVector.x;
            //targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
            {
                targetDirection =  transform.forward;
            }

            targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed *Time.deltaTime);

            transform.rotation = playerRotation;
        
    }

    

    void HandleFallingAndLanding()
    {

        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
        if (IsOnLadder) return;

        if (isGrounded)
        {
            IsOnLedge = envScanner.EdgeCheck(desiredMoveDirection,out EdgeHitData ledgeHitData);

            currentGravity = -0.5f;
            
            // limit LedgeMovement
            if(IsOnLedge) 
            {
                LedgeHitData = ledgeHitData; 
                HandleLedgeMovement();
            }
        }
        else
        {
            currentGravity += gravityForce * Time.deltaTime;
        }
    }

    private void HandleLadderMovement(Vector2 inputVector)
    {
        Debug.Log("Ladder Movement is being called");
        ladderHitData = envScanner.LadderCheck();
               
        if (!ladderHitData.ladderHitFound) {
            if(isGrounded)
            {
                IsOnLadder = false;
                animatorManager.LeaveLadder();
                return;
            }
           return; 
        }

        //if(isGrounded && ladderhitData.ladderHitFound)
        if (ladderHitData.ladderHitFound)
        {
            // If trynna climb the ladder cross then you cannot no boss
            //if (Vector3.Dot(-ladderHitData.ladderHit.transform.forward, transform.forward) < 0.5f)
            //{
            //    Debug.Log(Vector3.Dot(-ladderHitData.ladderHit.transform.forward, transform.forward));
            //    IsOnLadder = false;
            //    animatorManager.LeaveLadder();
            //    animatorManager.SetIsClimbingLadder(false);
            //    return;
            //}
            
            animatorManager.SetIsClimbingLadder(true);

            var targetRot = Quaternion.LookRotation(-ladderHitData.ladderHit.transform.forward);

            transform.rotation = Quaternion.Slerp(transform.rotation,
                targetRot, 200f * Time.deltaTime);


            //// Set player's position in centre of the ladder pos
            //Vector3 playerOnLadderPosition = new Vector3(ladderHitData.ladderHit.transform.position.x,transform.position.y, ladderHitData.ladderHit.transform.position.z);
            //transform.position = Vector3.Lerp( transform.position,playerOnLadderPosition,0.6f);

            //SetControl(true);

            Debug.Log("Setting player's position to ladder pos");

            moveDirection = new Vector3(0f,inputVector.y,0f);
            movementVelocity = moveDirection * ladderClimbingSpeed;


            if(inputVector.y > 0 && !isClimbingLadderUp)
            {
                SetAllLadderClimbingFalse();
                animatorManager.PlayTargetAnimation("LadderClimbUpLoop");
                animatorManager.SetIsClimbingLadder(isClimbingLadderUp);
                isClimbingLadderUp = true;
            }
            if (inputVector.y < 0 && !isClimbingLadderDown)
            {
                SetAllLadderClimbingFalse();
                animatorManager.PlayTargetAnimation("LadderClimbDownLoop");
                isClimbingLadderDown = true;
            }
            // if no input given and ladderIdle
            if (inputVector.y == 0){
                SetAllLadderClimbingFalse();
                animatorManager.PlayTargetAnimation("LadderIdle");
            }
            if(inputVector.y < 0 && isGrounded) 
            {
                animatorManager.PlayTargetAnimation("LeaveLadder");
                isClimbingLadderDown = false;
                animatorManager.LeaveLadder();
                IsOnLadder = false;
            }
           
            characterController.Move(movementVelocity*Time.deltaTime);

        }

    }

    void SetAllLadderClimbingFalse()
    {
        isClimbingLadderUp = false;
        isClimbingLadderIdle = false;
        isClimbingLadderDown = false;
    }

    // limits ledge movement, prevents player from falling down from ledge
    // TODO: add a looking down animation state to fix the falling off ledge error when brute force input and shee
    void HandleLedgeMovement()
    {
        float signedLedgeNormalMoveAngle = Vector3.SignedAngle(LedgeHitData.edgeFaceHit.normal, desiredMoveDirection,Vector3.up);
        float ledgeNormalMoveAngle = Mathf.Abs(signedLedgeNormalMoveAngle);

        if (Vector3.Angle(desiredMoveDirection, transform.forward) >= 80) 
        {
            // dont move just rotate
            movementVelocity = Vector3.zero;
            return;
        }

        if (ledgeNormalMoveAngle < 60)
        {
            movementVelocity = Vector3.zero;
            moveDirection = Vector3.zero;
        }
        else if(ledgeNormalMoveAngle < 90)
        {
            // angle is btwn 60 and 90, so limit velocity to horizontal direction

            // cross product of normal and up vector gives us the left vector
            var left = Vector3.Cross(Vector3.up, LedgeHitData.edgeFaceHit.normal);
            var direction = left * Mathf.Sign(signedLedgeNormalMoveAngle);

            movementVelocity = movementVelocity.magnitude * direction;
            moveDirection = direction;
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

            if (moveAmount >= 0.1f)
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
    
    //TODO: improve this mess of animation event functions later
    public void SetSpeedToRunning()
    {
        currentSpeed = runningSpeed;
    }

    // To Show Ground Check Sphere

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded? new Color(0, 1, 0, 0.5f):Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}
