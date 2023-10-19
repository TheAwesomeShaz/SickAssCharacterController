using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    private readonly float _movementSpeedDampingValue = 0.05f;

    public PlayerRunState(PlayerStateMachine context, PlayerStateManager playerStateFactory) :base(context, playerStateFactory){}

    public override void CheckSwitchStates()
    {
        if (_ctx.IsOnEdge || _ctx.EnvScanner.ObstacleCheck().forwardHitFound || _ctx.NormalizedMoveAmount <= 0.1f)
        {
            SwitchState(_stateManager.Idle());
        }

        else if (!_ctx.IsSprinting && _ctx.NormalizedMoveAmount > 0.15f)
        {
            SwitchState(_stateManager.Walk());
        }
        if (!_ctx.IsInteracting && _ctx.EnvScanner.ObstacleCheck().forwardHitFound && _ctx.InputManager.JumpInput)
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
        CheckSwitchStates();
        SetMovementSpeed();
    }


    private void SetMovementSpeed()
    {
        if (!_ctx.IsInteracting && !_ctx.IsOnEdge)
        {
            _ctx.CurrentSpeed = Mathf.Lerp(_ctx.CurrentSpeed, _ctx.SprintingSpeed, _movementSpeedDampingValue);
        }
    }

}
