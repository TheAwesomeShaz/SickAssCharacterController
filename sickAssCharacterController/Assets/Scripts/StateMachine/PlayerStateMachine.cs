using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    // References from places (try assigning them in inspector to reduce getcomponent cost)
    [field: SerializeField] public AnimatorManager AnimatorManager { get; set; }
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
    [Header("Ground Check and Falling Stuff")]
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private Vector3 _groundCheckOffset;
    [SerializeField] private LayerMask _groundLayer;
    [field: SerializeField] public float GravityForce { get; set; } = -10;
    [field: SerializeField] public LedgeHitData LedgeHitData { get; set; }


    // Movement Speeds
    [Header("Movement Speeds")]
    [SerializeField] private float _walkingSpeed = 1.5f;
    [SerializeField] private float _runningSpeed = 5f;
    [SerializeField] private float _sprintingSpeed = 7f;
    [SerializeField] private float _ladderClimbingSpeed = 0.5f;
    [field: SerializeField] public float CurrentGravity { get; private set; }
    [field: SerializeField] public float RotationSpeed { get; private set; } = 15f;
    [field: SerializeField] public bool IsGrounded { get; private set; }
    [field: SerializeField] public float NormalizedMoveAmount { get; private set; }

    [SerializeField] private float _currentSpeed;

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
        _currentState.UpdateState();
        SetMovementDirection(InputManager.MovementInput,InputManager.HighProfileInput);
        HandleGroundCheck();
        Debug.Log(_currentState);
    }

    private void HandleGroundCheck()
    {
        IsGrounded = Physics.CheckSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius, _groundLayer);
    }

    private void SetMovementDirection(Vector2 inputVector, bool highProfileInput)
    {
        NormalizedMoveAmount = Mathf.Clamp01(Mathf.Abs(inputVector.x) + Mathf.Abs(inputVector.y));

        IsSprinting = highProfileInput && NormalizedMoveAmount > 0.5f;
        //movement direction in front back direction
        _desiredMoveDirection = CameraObject.forward * inputVector.y;
        //movement direction in right left direction
        _desiredMoveDirection = _desiredMoveDirection + CameraObject.right * inputVector.x;
        //Keep the direction but reduce magnitude btwn 0 and 1
        _desiredMoveDirection.Normalize();
        // Dont want the player moving vertically upwards lmao
        _desiredMoveDirection.y = CurrentGravity;

        MoveDirection = _desiredMoveDirection;
    }
}
