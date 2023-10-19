using UnityEngine.EventSystems;
using UnityEngine;
using System;
using UnityEngine.Scripting;

public class PlayerGroundedState : PlayerBaseState
{
    private EdgeHitData edgeHitData;
    private Vector3 _desiredMoveDirection;

    public PlayerGroundedState(PlayerStateMachine context, PlayerStateManager playerStateFactory)
    : base (context,playerStateFactory) 
    {
        _isRootState = true;
        InitializeSubState();    
    }

    public override void CheckSwitchStates()
    {
        // TODO: Add the Climb Logic here?

        // if player is grounded and presses jump then:
        // if player is infront of a climbable object then switch to parkour super state (which again has many sub states)
        // if player taps and releases jump btn while running then jump state then fall then if something in front it can switch to grab 
        // which will be a state under the parkour super state ofc

        var obstacleHitData = _ctx.EnvScanner.ObstacleCheck();


        if (obstacleHitData.forwardHitFound && _ctx.InputManager.JumpInput)
        {
            SwitchState(_stateManager.Parkour());
        }

        if (!obstacleHitData.forwardHitFound && _ctx.IsOnEdge)
        {
            bool shouldJump = true;
            if (edgeHitData.height > _ctx.AutoJumpHeightLimit && !_ctx.InputManager.JumpInput)
                shouldJump = false;

            // if large angle then dont jump down
            if (shouldJump && edgeHitData.angle <= 50f)
            {
                SwitchState (_stateManager.JumpFromEdge());
            }

        }

        if (!_ctx.IsGrounded)
        {
            //SwitchState(_stateFactory.FreeFall());
        }
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        if (_ctx.IsOnEdge || _ctx.NormalizedMoveAmount <= 0.1f)
        {
            SetSubState(_stateManager.Idle());
        }

        else if (_ctx.NormalizedMoveAmount > 0.15f)
        {
            SetSubState(_stateManager.Walk());
        }

        else if(_ctx.IsSprinting && _ctx.NormalizedMoveAmount > 0.5f)
        {
            SetSubState(_stateManager.Run());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        if (!_ctx.IsInteracting)
        {
            HandleEdgeMovement();
            HandleGroundedMovement();
            HandleGravity();
        }
        // limit EdgeMovement
    }

    private void HandleGravity()
    {
        _ctx.CurrentGravity = -0.5f;
    }

    private void HandleGroundedMovement()
    {
        //Debug.Log("My substate is" + _currentSubState);

        _ctx.MovementVelocity = _ctx.MoveDirection * (_ctx.CurrentSpeed*_ctx.NormalizedMoveAmount);
        _ctx.IsSprinting = _ctx.MovementVelocity.x != 0 && _ctx.InputManager.HighProfileInput && !_ctx.EnvScanner.ObstacleCheck().forwardHitFound;

        if (!_ctx.EnvScanner.ObstacleCheck().forwardHitFound && !_ctx.IsOnEdge)
        {
            _ctx.CharacterController.Move(_ctx.MovementVelocity * Time.deltaTime);
            _ctx.AnimatorManager.UpdateAnimatorValues(0, _ctx.NormalizedMoveAmount, _ctx.IsSprinting);
        }
    }

    //TODO: The below stuff should go in a SubState?
    private void HandleEdgeMovement()
    {
        _ctx.IsOnEdge = _ctx.EnvScanner.EdgeCheck(_ctx.DesiredMoveDirection, out edgeHitData);
        if (_ctx.IsOnEdge)
        {
            LimitEdgeMovement();
        }
    }

    // limits ledge movement, prevents player from falling down from ledge
    // TODO: add a looking down animation state to fix the falling off ledge error when brute force input and shee
    private void LimitEdgeMovement()
    {
        float signedEdgeNormalMoveAngle = Vector3.SignedAngle(_ctx.EdgeHitData.edgeFaceHit.normal, _ctx.DesiredMoveDirection, Vector3.up);
        float edgeNormalMoveAngle = Mathf.Abs(signedEdgeNormalMoveAngle);

        if (Vector3.Angle(_ctx.DesiredMoveDirection, _ctx.transform.forward) >= 80)
        {
            // dont move just rotate
            _ctx.MovementVelocity = Vector3.zero;
            return;
        }

        if (edgeNormalMoveAngle < 60)
        {
            _ctx.MovementVelocity = Vector3.zero;
            _ctx.MoveDirection = Vector3.zero;
        }
        else if (edgeNormalMoveAngle < 90)
        {
            // angle is btwn 60 and 90, so limit velocity to horizontal direction
            // cross product of normal and up vector gives us the left vector
            var left = Vector3.Cross(Vector3.up, _ctx.EdgeHitData.edgeFaceHit.normal);
            var direction = left * Mathf.Sign(signedEdgeNormalMoveAngle);
            _ctx.MovementVelocity = _ctx.MovementVelocity.magnitude * direction;
            _ctx.MoveDirection = direction;
        }
    }
}
