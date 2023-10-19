using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFreeFallState : PlayerBaseState
{
    public PlayerFreeFallState(PlayerStateMachine context, PlayerStateManager stateFactory) : base(context, stateFactory)
    {
        _isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        if (_ctx.IsGrounded)
        {
            SwitchState(_stateManager.Grounded());
        }
        // Should Only do this if Parkour is Possible?
        if(_ctx.EnvScanner.ObstacleCheck().forwardHitFound && _ctx.InputManager.JumpInput)
        {
            SwitchState(_stateManager.Parkour());
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
    }

    public override void UpdateState()
    {
        HandleFreeFallMovement();
        HandleGravity();
        CheckSwitchStates();
    }

    private void HandleGravity()
    {
        _ctx.CurrentGravity += _ctx.GravityForce * Time.deltaTime;
    }

    private void HandleFreeFallMovement()
    {

        // move forward while jumping, forward jumping speed is currentSpeed/2

        Vector3 forwardJumpForce = _ctx.transform.forward * _ctx.CurrentSpeed / 1.2f;
        Vector3 downwardGravityForce = Vector3.up * _ctx.MoveDirection.y;

        //_ctx.MovementVelocity = forwardJumpForce + downwardGravityForce;
        //_ctx.CharacterController.Move(_ctx.MovementVelocity * Time.deltaTime);

        _ctx.MovementVelocity = (forwardJumpForce *_ctx.NormalizedMoveAmount) + downwardGravityForce;

        _ctx.IsSprinting = _ctx.MovementVelocity.x != 0 && _ctx.InputManager.HighProfileInput && !_ctx.EnvScanner.ObstacleCheck().forwardHitFound;

        if (!_ctx.EnvScanner.ObstacleCheck().forwardHitFound)
        {
            _ctx.CharacterController.Move(_ctx.MovementVelocity * Time.deltaTime);
            _ctx.AnimatorManager.UpdateAnimatorValues(0, _ctx.NormalizedMoveAmount, _ctx.IsSprinting);
        }
    }
}
