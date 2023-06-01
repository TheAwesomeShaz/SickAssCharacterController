using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [SerializeField] List<ParkourAction> parkourActions;
    [SerializeField] ParkourAction jumpDownAction;

    EnvironmentScanner environmentScanner;
    [SerializeField] float rotateTowardsObstacleSpeed = 500f;
    bool isInteracting;
    bool isOnLedge;
    ObstacleHitData hitData;

    public Animator animator;

    readonly int horizontal = Animator.StringToHash("Horizontal");
    readonly int vertical = Animator.StringToHash("Vertical");
    public readonly int IsGrounded = Animator.StringToHash("IsGrounded");

    public event Action<bool> OnSetInteracting;
    public event Action<bool> OnSetIsOnLedge;

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

    public void HandleAllParkour(bool jumpInput, bool isInteracting,bool isOnLedge,LedgeHitData ledgeHitData)
    {
        this.isOnLedge = isOnLedge;
        this.isInteracting = isInteracting;

        hitData = environmentScanner.ObstacleCheck();

        HandleObstacleCheck(jumpInput);
        HandleLedgeCheck(ledgeHitData);
    }

    void HandleLedgeCheck(LedgeHitData ledgeHitData)
    {
        if (isOnLedge && !isInteracting && !hitData.forwardHitFound)
        {
            // if large angle then dont jump down
            if (ledgeHitData.angle <= 50f)
            {
                Debug.Log("JumpDOWN ANIMATION!!!!");
                
                isOnLedge = false;
                OnSetIsOnLedge?.Invoke(isOnLedge);

                StartCoroutine(DoParkourAction(jumpDownAction));
            }                
        }
    }

    void HandleObstacleCheck(bool jumpInput)
    {

        if (jumpInput && !isInteracting)
        {
            if (hitData.forwardHitFound)
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
    }

    IEnumerator DoParkourAction(ParkourAction action)
    {
        isInteracting = true;
        OnSetInteracting?.Invoke(isInteracting);

        PlayTargetAnimation(action.AnimName);

        animator.SetBool("MirrorAction", action.Mirror);

        // we return null to wait for this frame to end before fetching the animator State
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);


        if (!animState.IsName(action.AnimName))
        {
            Debug.LogError("Parkour Animation Name is spelled wrong \n current animation name is "+animState.ToString()+" " +
                "given name is "+action.AnimName);
        }

        // wait for the current parkour animation to complete
        // The loop below means that while "the action is being performed" do this 
        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            
            if(action.RotateToObstacle) 
                transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                    action.TargetRotation, rotateTowardsObstacleSpeed* Time.deltaTime);

            if (action.IstargetMatchingEnabled)
                MatchTarget(action);

            if(animator.IsInTransition(0) && timer >= 0.5f)
                break;

            // return null makes it do nothing i.e it makes it wait till end of while loop?
            yield return null;
        }

        yield return new WaitForSeconds(action.PostActionDelay);

        isInteracting = false;
        OnSetInteracting?.Invoke(isInteracting);
    }

    void MatchTarget(ParkourAction action)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(action.MatchPos,transform.rotation,
            action.MatchBodyPart,new MatchTargetWeightMask(action.MatchPosWeight,0),
            action.MatchStartTime,action.MatchTargetTime);
    }

}
