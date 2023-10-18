using System;
using System.Collections;
using UnityEngine;

internal class PlayerParkourState : PlayerBaseState
{
    private bool isPerformingParkourAction;
    public Action OnCoroutineFinished;
    public PlayerParkourState(PlayerStateMachine context, PlayerStateManager stateFactory) : base(context, stateFactory) 
    {
        _isRootState = true;
        // Call initialize subStates if Required here
    }

    public override void CheckSwitchStates()
    {

        //TODO: switch back to Walk or Run Or Idle when isInteracting is False?
        if (!_ctx.IsInteracting)
        {
            SwitchState(_stateManager.Grounded());
        }

    }

    public override void EnterState()
    {
        _ctx.SetControl(false) ;
        _ctx.IsOnEdge = false;
        OnCoroutineFinished += () => _ctx.SetControl(true);
        HandleSimpleClimbParkour(_ctx.EnvScanner.ObstacleCheck());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    private void HandleSimpleClimbParkour(ObstacleHitData obstacleHitData)
    {
        // TODO: change the if Condition (Upgrades people Upgrades)

        foreach (var action in _ctx.AnimatorManager.ParkourActions)
        {
            if (action.CheckIfPossible(obstacleHitData, _ctx.transform))
            {
                _ctx.IsInteracting = true;
                _ctx.StartCoroutine(_ctx.AnimatorManager.DoParkourAction(action,OnCoroutineFinished));
                break;
            }
        }
        
    }

    //TODO: these are going  back to the animation manager, but setting IsOnLedge and IsInteracting will be done in the State itself i.e here

    //private IEnumerator DoParkourAction(ParkourAction action)
    //{
    //    //TODO: Change Booleans in the State itself not here, so wont need to Invoke Events
    //    _ctx.IsOnLedge = false;
    //    _ctx.IsInteracting = true;
    //    _ctx.SetControl(!_ctx.IsInteracting);

    //    var matchTargetParams = new MatchTargetParams
    //    {
    //        pos = action.MatchPos,
    //        bodyPart = action.MatchBodyPart,
    //        posWeight = action.MatchPosWeight,
    //        startTime = action.MatchStartTime,
    //        targetTime = action.MatchTargetTime,
    //    };

    //    yield return DoAction(action.AnimName, matchTargetParams, action.TargetRotation,
    //        action.ResetMovementSpeed, action.RotateToObstacle, action.PostActionDelay, action.Mirror);


    //    _ctx.IsInteracting = false;
    //    _ctx.SetControl(!_ctx.IsInteracting);
    //}

    //// Can use this thing for parkour as well as climb later? maybe make a separate state and refactor somehow?
    //IEnumerator DoAction(string animName, MatchTargetParams matchTargetParams, Quaternion targetRotation,
    //   bool resetMovementSpeed = false, bool rotate = false, float postActionDelay = 0f, bool mirror = false)
    //{
    //    //Debug.Log("Reset Movement Speed " + resetMovementSpeed);

    //    _ctx.AnimatorManager.SetRootMotion(true);
    //    //OnSetIsHanging?.Invoke(IsHanging);

    //    _ctx.AnimatorManager.PlayTargetAnimation(animName);
    //    _ctx.AnimatorManager.Animator.SetBool("MirrorAction", mirror);
    //    if (resetMovementSpeed) _ctx.CurrentSpeed = 0;

    //    // we return null to wait for this frame to end before fetching the animator State
    //    yield return null;

    //    var animState = _ctx.AnimatorManager.Animator.GetNextAnimatorStateInfo(0);

    //    if (!animState.IsName(animName))
    //    {
    //        Debug.LogError("Parkour Animation Name is spelled wrong \n current animation name is " + animState.GetHashCode() + " " +
    //            "given name is " + animName);
    //    }

    //    // wait for the current parkour animation to complete
    //    // The loop below means that while "the action is being performed" do this 
    //    float timer = 0f;
    //    while (timer <= animState.length)
    //    {
    //        timer += Time.deltaTime;

    //        if (rotate)
    //            _ctx.transform.rotation = Quaternion.RotateTowards(_ctx.transform.rotation,
    //                targetRotation, _ctx.AnimatorManager.RotateTowardsObstacleSpeed * Time.deltaTime);

    //        if (matchTargetParams != null)
    //            _ctx.AnimatorManager.MatchTarget(matchTargetParams);

    //        if (_ctx.AnimatorManager.Animator.IsInTransition(0) && timer >= 0.5f)
    //            break;

    //        // return null makes it do nothing i.e it makes it wait till end of while loop?
    //        yield return null;

    //    }
    //}
}
