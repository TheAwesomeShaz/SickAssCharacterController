using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{

    public PlayerWalkState(PlayerStateMachine context, PlayerStateFactory playerStateFactory) : base(context, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (_ctx.NormalizedMoveAmount <= 0.1f)
        {
            SetSubState(_stateFactory.Idle());
        }
        else if (_ctx.IsSprinting && _ctx.NormalizedMoveAmount > 0.5f)
        {
            SetSubState(_stateFactory.Run());
        }
    }

    public override void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

}
