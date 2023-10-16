using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    private readonly float _movementSpeedDampingValue = 0.7f;

    public PlayerRunState(PlayerStateMachine context, PlayerStateFactory playerStateFactory) :base(context, playerStateFactory){}

    public override void CheckSwitchStates()
    {
        if (_ctx.NormalizedMoveAmount <= 0.1f)
        {
            SwitchState(_stateFactory.Idle());
        }

        else if (!_ctx.IsSprinting && _ctx.NormalizedMoveAmount > 0.15f)
        {
            SwitchState(_stateFactory.Walk());
        }
    }

    public override void EnterState()
    {
        _ctx.AnimatorManager.UpdateAnimatorValues(0, _ctx.NormalizedMoveAmount, _ctx.InputManager.HighProfileInput);
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
        _ctx.CurrentSpeed = Mathf.Lerp(_ctx.CurrentSpeed, _ctx.SprintingSpeed, _movementSpeedDampingValue);

        //_ctx.NormalizedMoveAmount = Mathf.Clamp(_ctx.MovementVelocity.magnitude / _ctx.CurrentSpeed, 0, 2);
    }

}
