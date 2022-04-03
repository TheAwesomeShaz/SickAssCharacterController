//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//[RequireComponent(typeof(Rigidbody))]
//public class PlayerMovement : MonoBehaviour
//{
//    [SerializeField] float walkSpeed;
//    [SerializeField] float jogSpeed;
//    //[SerializeField] float runSpeed;


//    private float turnSmoothVelocity = 0.5f;
//    private float turnSmoothTime = 0.2f;

//    float currentSpeed;
    

//    Animator anim;
//    PlayerControls input;
//    Rigidbody rb;
//    Transform cam;

//    int isWalkingHash;
//    int isRunningHash;


//    Vector2 cameraVector;
//    Vector2 currentMovement;
//    Vector3 moveDir;
    
//    bool movementPressed;
//    bool isHighProfile;

//    private void Awake()
//    {
//        input = new PlayerControls();

//        input.CharacterControls.Movement.performed += ctx =>
//        {
//            currentMovement = ctx.ReadValue<Vector2>();
//            movementPressed = currentMovement.x!=0 || currentMovement.y!= 0;
//            print("Value of joystick right now is: " + currentMovement);
//            print("Movement Pressed is: " + movementPressed);

//        };
        
//        input.CharacterControls.HighProfileModifier.performed += ctx =>
//        {
//            isHighProfile = ctx.ReadValueAsButton();
//        };
//        input.CharacterControls.Camera.performed += ctx =>
//        {
//            cameraVector = ctx.ReadValue<Vector2>();
//        };
//    }


//    private void OnEnable()
//    {
//        input.CharacterControls.Enable();
//    }

//    private void OnDisable()
//    {
//        input.CharacterControls.Disable();
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        anim = GetComponent<Animator>();
//        rb = GetComponent<Rigidbody>();
//        cam = Camera.main.transform;
//        isWalkingHash = Animator.StringToHash("isWalking");
//        isRunningHash = Animator.StringToHash("isRunning");
//    }

//    private void Update()
//    {
//        Animate();
//        //HandleRotation();
//        SetPlayerDirection();
//    }

//    private void FixedUpdate()
//    {
//        Move();
//    }

//    private void SetPlayerDirection()
//    {
//        Vector3 direction = new Vector3(currentMovement.x, 0f, currentMovement.y).normalized;
//        if (direction.magnitude >= 0.1f)
//        {
//            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

//            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,
//            targetAngle, ref turnSmoothVelocity, turnSmoothTime);

//            transform.rotation = Quaternion.Euler(0f, angle, 0f);

//            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

//        }
//    }

//    private void Move()
//    {
//        if (movementPressed)
//        {
//        currentSpeed = isHighProfile ? jogSpeed : walkSpeed; 
//        rb.MovePosition(transform.position + Time.deltaTime * currentSpeed *
//                            moveDir.normalized);
//        }
//    }

//    private void HandleRotation()
//    {
//        Vector3 currentPosition = transform.position;

//        //the change in positon that our character should point to
//        Vector3 newPosition = new Vector3(currentMovement.x, 0F, currentMovement.y);

//        Vector3 positionToLookAt = currentPosition + newPosition;

//        moveDir = new Vector3(currentMovement.x, 0f, currentMovement.y);
        
//        transform.LookAt(positionToLookAt);
//    }

//    public void Animate()
//    {
//        bool isWalking = anim.GetBool(isWalkingHash);
//        bool isRunning = anim.GetBool(isRunningHash);

//        if (movementPressed && !isWalking)
//        {
//            anim.SetBool(isWalkingHash, true);
//        }
//        if (!movementPressed && isWalking)
//        {
//            anim.SetBool(isWalkingHash, false);
//        }
//        if ((movementPressed && isHighProfile)&& !isRunning)
//        {
//            anim.SetBool(isRunningHash, true);
//        }
//        if ((!movementPressed || !isHighProfile) && isRunning)
//        {
//            anim.SetBool(isRunningHash, false);
//        }
//    }
//}
