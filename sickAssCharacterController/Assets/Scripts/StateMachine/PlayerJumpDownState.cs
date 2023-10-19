using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpFromEdgeState : PlayerBaseState
{
    private Action OnCoroutineFinished;
    public PlayerJumpFromEdgeState(PlayerStateMachine context, PlayerStateManager stateFactory) : base(context, stateFactory)
    {
        _isRootState = true;    
    }

    public override void CheckSwitchStates()
    {
        if (!_ctx.IsInteracting)
        {
            if (_ctx.IsGrounded)
            {
                SwitchState(_stateManager.Grounded());
            }
            else
            {
                SwitchState(_stateManager.FreeFall());
            }
        }

    }

    public override void EnterState()
    {
        _ctx.SetControl(false);
        _ctx.IsOnEdge = false;
        OnCoroutineFinished += () => _ctx.SetControl(true);
        HandleJumpFromEdge();
    }

    private void HandleJumpFromEdge()
    {
        _ctx.CurrentSpeed = Mathf.Lerp(_ctx.CurrentSpeed, _ctx.ForwardJumpSpeed + (_ctx.ForwardJumpSpeed * (_ctx.InputManager.HighProfileInput?1:0)), 1f);
        var jumptype = _ctx.InputManager.HighProfileInput ?_ctx.AnimatorManager.JumpOffAction : _ctx.AnimatorManager.JumpDownAction;
        _ctx.StartCoroutine(_ctx.AnimatorManager.DoParkourAction(jumptype, OnCoroutineFinished));
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        _ctx.IsOnEdge = false;
        CheckSwitchStates();
    }

   
}
