using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerGroundedIdleState : PlayerBaseState
{
    public PlayerGroundedIdleState(PlayerStateMachine context, PlayerStateManager playerStateFactory) : base(context, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if(_ctx.IsSprinting && _ctx.NormalizedMoveAmount > 0.5f)
        {
            SwitchState(_stateManager.Run());
        }
        else if(_ctx.NormalizedMoveAmount > 0.1f)
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
        _ctx.CurrentSpeed = 0f;
        _ctx.AnimatorManager.UpdateAnimatorValues(0, 0, _ctx.IsSprinting);
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
}
