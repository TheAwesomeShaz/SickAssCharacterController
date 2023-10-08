using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine context, PlayerStateFactory playerStateFactory) : base(context, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if(_ctx.IsSprinting && _ctx.NormalizedMoveAmount > 0.5f)
        {
            SwitchState(_stateFactory.Run());
        }
        else if(_ctx.NormalizedMoveAmount > 0.1f)
        {
            SwitchState(_stateFactory.Walk());
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
    }

}
