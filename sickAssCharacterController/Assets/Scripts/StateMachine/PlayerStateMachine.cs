using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector3 _desiredMoveDirection;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isJumping;
    [field: SerializeField] public bool IsOnLedge { get; set; }
    [field: SerializeField] public bool IsOnLadder { get; set; }
    [field: SerializeField] public bool IsHanging { get; set; }

    // TODO: maybe turn these into properties too?
    // Ground Check Stuff..
    [field: Header("Ground Check and Falling Stuff")]
    [field: SerializeField] public float GroundCheckRadius { get; set; } = 0.2f;
    [field: SerializeField] public Vector3 GroundCheckOffset { get; set; }
    [field: SerializeField] public LayerMask GroundLayer { get; set; }
    [field: SerializeField] public float GravityForce { get; set; } = -10;
    [field: SerializeField] public LedgeHitData LedgeHitData { get; set; }


    // Movement Speeds
    [field: Header("Movement Speeds")]
    [field: SerializeField] public float WalkingSpeed { get; private set; } = 1.5f;
    [field: SerializeField] public float RunningSpeed { get; private set; } = 5f;
    [field: SerializeField] public float SprintingSpeed { get; private set; } = 7f;

    [SerializeField] private float _ladderClimbingSpeed = 0.5f;
    [field: SerializeField] public float CurrentGravity { get; private set; }
    [field: SerializeField] public float RotationSpeed { get; private set; } = 15f;
    [field: SerializeField] public bool IsGrounded { get; private set; }
    [field: SerializeField] public float NormalizedMoveAmount { get; private set; }

    [field: SerializeField] public float CurrentSpeed { get; set; }

    // WTF is going On??
    [Tooltip("The amount of speed which will get added/subtracted every frame to increase/decrease speed smoothly")]
    [SerializeField] private float _movementSpeedDampingValue = 0.5f;

    // Cached Stuff for later Use
    private Quaternion _targetRotation;
    [field: SerializeField] public Vector3 MoveDirection { get; set; }
    public Vector3 DesiredMoveDirection { get; set; }
    public Vector3 MovementVelocity { get; set; }

    // State variables
    public PlayerBaseState _currentState;
    PlayerStateFactory _states;

    private void Awake()
    {
        // TODO: Change it into Cinemachine Controller idk summ gon change ig
        CameraObject = Camera.main.transform;

        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    private void Update()
    {
        InputManager.HandleAllInputs();
        _currentState.UpdateStates();
        HandleGroundCheck();
        SetMovementDirection(InputManager.MovementInput,InputManager.HighProfileInput);
        HandleRotation(InputManager.MovementInput);

        // TODO: should this be here or in the Grounded state?

        //AnimatorManager.UpdateAnimatorValues(0, NormalizedMoveAmount, InputManager.HighProfileInput);
    }

    private void LateUpdate()
    {
        CameraManager.HandleAllCameraMovement();
    }

    private void HandleGroundCheck()
    {
        IsGrounded = Physics.CheckSphere(transform.TransformPoint(GroundCheckOffset), GroundCheckRadius, GroundLayer);
        if (IsGrounded)
        {
            IsOnLedge = EnvScanner.EdgeLedgeCheck(DesiredMoveDirection, out LedgeHitData ledgeHitData);

            CurrentGravity = -0.5f;

            // TODO: Take this if block into another State
            // limit LedgeMovement
            //if (IsOnLedge)
            //{
            //    LedgeHitData = ledgeHitData;
            //    //HandleLedgeMovement();
            //}
        }
        else
        {
            CurrentGravity += GravityForce * Time.deltaTime;
        }

    }

    private void SetMovementDirection(Vector2 inputVector, bool highProfileInput)
    {
        NormalizedMoveAmount = Mathf.Clamp01(Mathf.Abs(inputVector.x) + Mathf.Abs(inputVector.y));

        IsSprinting = highProfileInput && NormalizedMoveAmount > 0.5f;
        //movement direction in front back direction
        _desiredMoveDirection = CameraObject.forward * inputVector.y;
        //movement direction in right left direction
        _desiredMoveDirection += CameraObject.right * inputVector.x;
        //Keep the direction but reduce magnitude btwn 0 and 1
        _desiredMoveDirection.Normalize();
        // Dont want the player moving vertically upwards lmao
        _desiredMoveDirection.y = CurrentGravity;

        MoveDirection = _desiredMoveDirection;
    }
    
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

    private void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded ? new Color(0, 1, 0, 0.5f) : Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(GroundCheckOffset), GroundCheckRadius);
    }
}
