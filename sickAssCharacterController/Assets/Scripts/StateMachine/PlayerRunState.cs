using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{

    public PlayerRunState(PlayerStateMachine context, PlayerStateFactory playerStateFactory) :base(context, playerStateFactory){}

    public override void CheckSwitchStates()
    {
        if (_ctx.NormalizedMoveAmount <= 0.1f)
        {
            SetSubState(_stateFactory.Idle());
        }

        else if (_ctx.NormalizedMoveAmount > 0.15f)
        {
            SetSubState(_stateFactory.Walk());
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
