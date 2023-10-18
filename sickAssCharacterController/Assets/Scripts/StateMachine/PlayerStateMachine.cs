using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStateMachine : MonoBehaviour
{
    // References from places (try assigning them in inspector to reduce getcomponent cost)
    [field: SerializeField] public AnimatorManager AnimatorManager { get; set; }
    [field: SerializeField] public CameraManager CameraManager { get; set; }
    [field: SerializeField] public EnvironmentScanner EnvScanner { get; set; }
    [field: SerializeField] public CharacterController CharacterController { get; set; }
    [field: SerializeField] public InputManager InputManager { get; private set; }
    [field: SerializeField] public Transform CameraObject { get; private set; }
    

    //TODO: As refactor progresses try reducing the booleans as much as possible
    // Movement Booleans (They kinda BAD tho)
    [field: Header("Movement Flags")]
    [field: SerializeField] public bool IsSprinting { get; set; }

    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isJumping;
    [field: SerializeField] public bool IsOnEdge { get; set; }
    [field: SerializeField] public bool IsInteracting { get; set; }
    [field: SerializeField] public bool IsOnLadder { get; set; }
    [field: SerializeField] public bool IsHanging { get; set; }
    [field: SerializeField] public bool IsGrounded { get; private set; }

    // TODO: maybe turn these into properties too?
    // Ground Check Stuff..
    [field: Header("Ground Check and Falling Stuff")]
    [field: SerializeField] public float GroundCheckRadius { get; set; } = 0.2f;
    [field: SerializeField] public Vector3 GroundCheckOffset { get; set; }
    [field: SerializeField] public LayerMask GroundLayer { get; set; }
    [field: SerializeField] public float GravityForce { get; set; } = -10;
    [field: SerializeField] public EdgeHitData EdgeHitData { get; set; }
    [field: SerializeField] public float AutoJumpHeightLimit { get; private set; } = 2f;



    // Movement Speeds
    [field: Header("Movement Speeds")]
    [field: SerializeField] public float WalkingSpeed { get; private set; } = 1.5f;
    [field: SerializeField] public float RunningSpeed { get; private set; } = 5f;
    [field: SerializeField] public float SprintingSpeed { get; private set; } = 7f;

    [SerializeField] private float _ladderClimbingSpeed = 0.5f;
    [field: SerializeField] public float CurrentGravity { get; set; }
    [field: SerializeField] public float RotationSpeed { get; private set; } = 15f;
    [field: SerializeField] public float NormalizedMoveAmount { get; set; }

    [field: SerializeField] public float CurrentSpeed { get; set; }

    // WTF is going On??
    [Tooltip("The amount of speed which will get added/subtracted every frame to increase/decrease speed smoothly")]
    [SerializeField] private float _movementSpeedDampingValue = 0.5f;

    // Cached Stuff for later Use
    private Quaternion _targetRotation;
    [field: SerializeField] public Vector3 MoveDirection { get; set; }
    public Vector3 DesiredMoveDirection { get; set; }
    [field:SerializeField] public Vector3 MovementVelocity { get; set; }

    // State variables
    public PlayerBaseState _currentState;
    PlayerStateManager _states;

    private void Awake()
    {
        // TODO: Change it into Cinemachine Controller idk summ gon change ig
        CameraObject = Camera.main.transform;

        // setup state
        _states = new PlayerStateManager(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();


    }

    private void Update()
    {
        InputManager.HandleAllInputs();
        _currentState.UpdateStates();
        HandleGroundCheck();
        SetMovementDirection(InputManager.MovementInput,InputManager.HighProfileInput);
        if (!IsInteracting)
        {
            HandleRotation(InputManager.MovementInput);
        }
        Debug.Log("Current State is " + _currentState);

        // TODO: should this be here or in the Grounded state?
        //AnimatorManager.UpdateAnimatorValues(0, NormalizedMoveAmount, InputManager.);
    }

    private void LateUpdate()
    {
        CameraManager.HandleAllCameraMovement();
    }

    private void HandleGroundCheck()
    {
        IsGrounded = Physics.CheckSphere(transform.TransformPoint(GroundCheckOffset), GroundCheckRadius, GroundLayer);
        //if (IsGrounded)
        //{
            // TODO: this stuff goes in the grounded state
            //IsOnLedge = EnvScanner.EdgeCheck(DesiredMoveDirection, out EdgeHitData ledgeHitData);

            //CurrentGravity = -0.5f;

            //// TODO: Take this if block into another State?
            // limit EdgeMovement
            //if (IsOnLedge)
            //{
            //    LedgeHitData = ledgeHitData;
            //    //LimitEdgeMovement();
            //}
        //}
        // TODO: move this into the FreeFall State
        //if(!IsGrounded)
        //{
        //    CurrentGravity += GravityForce * Time.deltaTime;
        //}
    }

    private void SetMovementDirection(Vector2 inputVector, bool highProfileInput)
    {
        NormalizedMoveAmount = Mathf.Clamp01(Mathf.Abs(inputVector.x) + Mathf.Abs(inputVector.y));

        IsSprinting = highProfileInput && NormalizedMoveAmount > 0.5f;
        //movement direction in front back direction
        DesiredMoveDirection = CameraObject.forward * inputVector.y;
        //movement direction in right left direction
        DesiredMoveDirection += CameraObject.right * inputVector.x;
        //Keep the direction but reduce magnitude btwn 0 and 1
        DesiredMoveDirection.Normalize();
        // Dont want the player moving vertically upwards lmao
        DesiredMoveDirection = new Vector3(DesiredMoveDirection.x,CurrentGravity,DesiredMoveDirection.z);

        MoveDirection = DesiredMoveDirection;
    }

    //void LimitEdgeMovement()
    //{
    //    float signedLedgeNormalMoveAngle = Vector3.SignedAngle(LedgeHitData.ledgeFaceHit.normal, DesiredMoveDirection, Vector3.up);
    //    float ledgeNormalMoveAngle = Mathf.Abs(signedLedgeNormalMoveAngle);

    //    if (Vector3.Angle(DesiredMoveDirection, transform.forward) >= 80)
    //    {
    //        // dont move just rotate
    //        MovementVelocity = Vector3.zero;
    //        return;
    //    }

    //    if (ledgeNormalMoveAngle < 60)
    //    {
    //        MovementVelocity = Vector3.zero;
    //        MoveDirection = Vector3.zero;
    //    }
    //    else if (ledgeNormalMoveAngle < 90)
    //    {
    //        // angle is btwn 60 and 90, so limit velocity to horizontal direction

    //        // cross product of normal and up vector gives us the left vector
    //        var left = Vector3.Cross(Vector3.up, LedgeHitData.ledgeFaceHit.normal);
    //        var direction = left * Mathf.Sign(signedLedgeNormalMoveAngle);

    //        MovementVelocity = MovementVelocity.magnitude * direction;
    //        MoveDirection = direction;
    //    }
    //}

    private void HandleRotation(Vector2 inputVector)
    {
        Vector3 targetDirection = MoveDirection;

        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        var targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    /// <summary>
    /// Sets Character Controller enabled, isInteracting, and rotates to obstacle Accordingly 
    /// </summary>
    /// <param name="hasControl">enables character controller, sets isInteracting to false, prevents rotate to obstacle</param>
    public void SetControl(bool hasControl)
    {
        if (!hasControl)
        {
            // set animator moveAmount to 0
            AnimatorManager.UpdateAnimatorValues(0, 0, IsSprinting);
            _targetRotation = transform.rotation;
        }
        CharacterController.enabled = hasControl;
        IsInteracting = !hasControl;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded ? new Color(0, 1, 0, 0.5f) : Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(GroundCheckOffset), GroundCheckRadius);
    }
}
