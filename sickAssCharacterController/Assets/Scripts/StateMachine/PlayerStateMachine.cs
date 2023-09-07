using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    // References from places (try assigning them in inspector to reduce getcomponent cost)
    [SerializeField] private AnimatorManager AnimatorManager { get; set; }
    [SerializeField] public EnvironmentScanner EnvScanner { get; set; }
    [SerializeField] private Transform CameraObject { get; set; }
    [field: SerializeField] public CharacterController CharacterController { get; set; }

    [field: SerializeField] public InputManager InputManager { get; private set; }
    

    // Movement Booleans (They kinda BAD tho)
    [Header("Movement Flags")]
    [SerializeField] private bool _isSprinting;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isJumping;
    [field: SerializeField] public bool IsOnLedge { get; set; }
    [field: SerializeField] public bool IsOnLadder { get; set; }
    [field: SerializeField] public bool IsHanging { get; set; }

    // Ground Check Stuff.. TODO: maybe turn these into properties too?
    [Header("Ground Check and Falling Stuff")]
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private Vector3 _groundCheckOffset;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float GravityForce { get; set; } = -10;
    [field: SerializeField] public LedgeHitData LedgeHitData { get; set; }


    // Movement Speeds
    [Header("Movement Speeds")]
    [SerializeField] private float _walkingSpeed = 1.5f;
    [SerializeField] private float _runningSpeed = 5f;
    [SerializeField] private float _sprintingSpeed = 7f;
    [SerializeField] private float _ladderClimbingSpeed = 0.5f;
    [field: SerializeField] public float CurrentGravity { get; private set; }
    [field: SerializeField] public float _RotationSpeed { get; private set; } = 15f;
    [field: SerializeField] public bool IsGrounded { get; private set; }

    [SerializeField] private float _currentSpeed;

    // WTF is going On??
    [Tooltip("The amount of speed which will get added/subtracted every frame to increase/decrease speed smoothly")]
    [SerializeField] private float _movementSpeedDampingValue = 0.5f;

    // Cached Stuff for later Use
    private Quaternion _targetRotation;
    public Vector3 MoveDirection { get; set; }
    public Vector3 DesiredMoveDirection { get; set; }
    public Vector3 MovementVelocity { get; set; }

    // State variables
    public PlayerBaseState _currentState;
   

    PlayerStateFactory _states;

    private void Awake()
    {
        //TODO: remove all this GetComponent Shet and Assign the stuff in inspector to prevent all this unnecessary mess

        CameraObject = Camera.main.transform;

        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

    }

    private void Update()
    {
        _currentState.UpdateState(); 
        HandleGroundCheck();
    }
    private void HandleGroundCheck()
    {
        IsGrounded = Physics.CheckSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius, _groundLayer);
    }

}
