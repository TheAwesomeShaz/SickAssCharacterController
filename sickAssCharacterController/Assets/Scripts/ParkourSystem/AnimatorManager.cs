using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    // Component References
    public Animator animator;
    EnvironmentScanner environmentScanner;

    [Header("Parkour Stuff")]
    [SerializeField] List<ParkourAction> parkourActions;
    [SerializeField] ParkourAction jumpDownAction;
    [SerializeField] ParkourAction jumpOffAction;
    [SerializeField] float autoJumpHeightLimit = 2f;
    [SerializeField] float rotateTowardsObstacleSpeed = 500f;

    // Readonly Stuff
    readonly int horizontal = Animator.StringToHash("Horizontal");
    readonly int vertical = Animator.StringToHash("Vertical");
    public readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    // Events
    public event Action<bool> OnSetInteracting;
    public event Action<bool> OnSetIsOnLedge;
    public event Action<bool> OnResetSpeed;
    public event Action<bool> OnSetIsHanging;

    // Booleans
    public bool IsHanging { get; set; }
    bool isInteracting;
    bool isOnLedge;

    // Other Stuff
    ClimbPoint currentClimbPoint;
    ObstacleHitData hitData;


    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animator = GetComponent<Animator>();
    }

    #region Animator Stuff
    public void SetAnimatorBool(int hash, bool value)
    {
        animator.SetBool(hash, value);
    }

    public void PlayTargetAnimation(string targetAnimationHash)
    {
        animator.CrossFade(targetAnimationHash, 0.2f);
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        // Animation Snapping
        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal
        if (horizontalMovement > 0f && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1f;
        }
        else if (horizontalMovement < 0f && horizontalMovement > -0.55f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            snappedHorizontal = -1f;
        }
        else
        {
            snappedHorizontal = 0f;
        }
        #endregion
        #region Snapped Vertical
        if (verticalMovement > 0f && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVertical = 1f;
        }
        else if (verticalMovement < 0f && verticalMovement > -0.55f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            snappedVertical = -1f;
        }
        else
        {
            snappedVertical = 0f;
        }
        #endregion

        if (isSprinting)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 2f;
        }

        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }
    #endregion

    #region Parkour Stuff
    public void HandleAllParkour(bool jumpInput, bool isInteracting,bool isOnLedge,LedgeHitData ledgeHitData, bool sprintingInput)
    {
        this.isOnLedge = isOnLedge;
        this.isInteracting = isInteracting;

        hitData = environmentScanner.ObstacleCheck();

        HandleObstacleCheck(jumpInput);
        HandleLedgeCheck(jumpInput,ledgeHitData,sprintingInput);
    }

    void HandleLedgeCheck(bool jumpInput,LedgeHitData ledgeHitData,bool sprintingInput)
    {

        if (isOnLedge && !isInteracting && !hitData.forwardHitFound)
        {
            bool shouldJump = true;
            if (ledgeHitData.height > autoJumpHeightLimit && !jumpInput) 
                shouldJump = false;

            // if large angle then dont jump down
            if (shouldJump && ledgeHitData.angle <= 50f)
            {
                //Debug.Log("JumpDOWN ANIMATION!!!!");
                
                isOnLedge = false;
                OnSetIsOnLedge?.Invoke(isOnLedge);

                var jumptype = sprintingInput ? jumpOffAction : jumpDownAction;

                StartCoroutine(DoParkourAction(jumptype));
            }
        }

    }

    void HandleObstacleCheck(bool jumpInput)
    {
        if (jumpInput && !isInteracting && hitData.forwardHitFound)
        {
            foreach (var action in parkourActions)
            {
                if (action.CheckIfPossible(hitData,transform))
                {
                    StartCoroutine(DoParkourAction(action));
                    break;
                }
            }
        }
    }

    IEnumerator DoParkourAction(ParkourAction action)
    {
        isOnLedge = false;

        isInteracting = true;
        OnSetInteracting?.Invoke(isInteracting);

        var matchTargetParams = new MatchTargetParams
        {
            pos = action.MatchPos,
            bodyPart = action.MatchBodyPart,
            posWeight = action.MatchPosWeight,
            startTime = action.MatchStartTime,
            targetTime = action.MatchTargetTime,
        };

        yield return DoAction(action.AnimName, matchTargetParams, action.TargetRotation,
            action.ResetMovementSpeed,action.RotateToObstacle, action.PostActionDelay, action.Mirror);

        isInteracting = false;
        OnSetInteracting?.Invoke(isInteracting);
    }
    #endregion

    public IEnumerator DoAction(string animName,MatchTargetParams matchTargetParams, Quaternion targetRotation,
        bool resetMovementSpeed = false,bool rotate = false, float postActionDelay=0f, bool mirror = false)
    {
        //Debug.Log("Reset Movement Speed "+resetMovementSpeed);

        animator.applyRootMotion = true;
        OnSetIsHanging?.Invoke(IsHanging);

        PlayTargetAnimation(animName);
        animator.SetBool("MirrorAction", mirror);
        if (resetMovementSpeed) OnResetSpeed?.Invoke(resetMovementSpeed);

        // we return null to wait for this frame to end before fetching the animator State
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);


        if (!animState.IsName(animName))
        {
            Debug.LogError("Parkour Animation Name is spelled wrong \n current animation name is " + animState.ToString() + " " +
                "given name is " + animName);
        }

        // wait for the current parkour animation to complete
        // The loop below means that while "the action is being performed" do this 
        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;

            if (rotate)
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    targetRotation, rotateTowardsObstacleSpeed * Time.deltaTime);

            if (matchTargetParams != null)
                MatchTarget(matchTargetParams);

            if (animator.IsInTransition(0) && timer >= 0.5f)
                break;

            // return null makes it do nothing i.e it makes it wait till end of while loop?
            yield return null;
        }

        yield return new WaitForSeconds(postActionDelay);

      
    }


    public void HandleAllClimbing(Vector2 inputDir, bool jumpInput, bool isHanging,bool isInteracting)
    {
        IsHanging = isHanging;
        var inputDirection = inputDir;

        if (!IsHanging && jumpInput && !isInteracting) 
        { 
            if (environmentScanner.ClimbLedgeCheck(transform.forward, out RaycastHit climbLedgeHit))
            {
                currentClimbPoint = climbLedgeHit.transform.GetComponent<ClimbPoint>();

                // TODO: organize this mess using scriptable objects or creating data class for climbing animations
                StartCoroutine(JumpToLedge("IdleToHanging", climbLedgeHit.transform, 0.435f, 0.85f));
            }
        }

        else
        {
            // ledge to ledge jump
            // find the neighbouring ledge in the direction of input
            
            Neighbour neighbour = currentClimbPoint.GetNeighbourInDirection(inputDirection);
            if(neighbour == null) return; 

            if(neighbour.connectionType == ConnectionType.Jump && jumpInput)
            {
                currentClimbPoint = neighbour.point;

                Debug.Log(currentClimbPoint);

                //TODO: make data class for climbing animations do not hard code values like this idiot here
                if (neighbour.direction.y == 1)
                    StartCoroutine(JumpToLedge("HangHopUp", currentClimbPoint.transform, 0.35f, 0.89f));
                if (neighbour.direction.y == -1)
                    StartCoroutine(JumpToLedge("HangHopDown", currentClimbPoint.transform, 0.43f, 0.72f));
            }

        }

    }

    IEnumerator JumpToLedge(string anim, Transform ledge, float matchStartTime, float matchTargetTime)
    {
        var matchParams = new MatchTargetParams()
        {
            pos = GetHandPosition(ledge),
            bodyPart = AvatarTarget.RightHand,
            startTime = matchStartTime,
            targetTime = matchTargetTime,
            posWeight = Vector3.one,
        };

        var targetRot = Quaternion.LookRotation(-ledge.forward);

        // setting hanging to true before action cuz we need to disable rootMotion in the player controller
        IsHanging = true;
        OnSetIsHanging?.Invoke(IsHanging);

        // This will cause execution to wait until the DoAction Coroutine is complete
        yield return DoAction(anim, matchParams, targetRot, true, true);


    }

    Vector3 GetHandPosition(Transform ledge)
    {
        // we are hardCoding X,Y and Z offset here 
        //TODO: later add the offset in the "climb anim" scriptable object when we create one

        return ledge.position + ledge.forward * -0.05f + Vector3.up * 0.09f - ledge.right * 0.35f;
    }

    void MatchTarget(MatchTargetParams mtp)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(mtp.pos,transform.rotation,
            mtp.bodyPart,new MatchTargetWeightMask(mtp.posWeight,0),
            mtp.startTime,mtp.targetTime);
    }

}

public class MatchTargetParams
{
    public Vector3 pos;
    public AvatarTarget bodyPart;
    public Vector3 posWeight;
    public float startTime;
    public float targetTime;
}

