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

    public PlayerWalkState(PlayerStateMachine context, PlayerStateManager playerStateFactory) : base(context, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (_ctx.IsOnEdge || _ctx.EnvScanner.ObstacleCheck().forwardHitFound || _ctx.NormalizedMoveAmount <= 0.1f)
        {
            SwitchState(_stateManager.Idle());
        }
        else if (_ctx.IsSprinting && _ctx.NormalizedMoveAmount > 0.5f)
        {
            SwitchState(_stateManager.Run());
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
        if (!_ctx.IsInteracting && !_ctx.IsOnEdge) { 
            _ctx.CurrentSpeed = Mathf.Lerp(_ctx.CurrentSpeed,_ctx.RunningSpeed,_movementSpeedDampingValue); 
        }
    }

}
