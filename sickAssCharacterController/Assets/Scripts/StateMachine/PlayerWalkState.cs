using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This is Actually supposed to be Jogging but oh well lmao x3
/// </summary>
public class PlayerWalkState : PlayerBaseState
{

    private readonly float _movementSpeedDampingValue = 0.7f;

    public PlayerWalkState(PlayerStateMachine context, PlayerStateFactory playerStateFactory) : base(context, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (_ctx.NormalizedMoveAmount <= 0.01f)
        {
            SwitchState(_stateFactory.Idle());
        }
        else if (_ctx.IsSprinting && _ctx.NormalizedMoveAmount > 0.5f)
        {
            SwitchState(_stateFactory.Run());
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
        _ctx.CurrentSpeed = Mathf.Lerp(_ctx.CurrentSpeed,_ctx.RunningSpeed,_movementSpeedDampingValue);
        //_ctx.NormalizedMoveAmount = Mathf.Clamp(_ctx.MovementVelocity.magnitude / _ctx.CurrentSpeed, 0, 2);

    }

}
