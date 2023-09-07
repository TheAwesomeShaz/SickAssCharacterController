internal class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine context, PlayerStateFactory stateFactory) : base(context, stateFactory)
    {
    }

    public override void CheckSwitchStates()
    {
        if (_ctx.CharacterController.isGrounded)
        {
            SwitchState(_stateFactory.Grounded());
        }
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
        // Falling down after reaching a jump height or something
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }

    private void HandleJump()
    {
        // Jumping Logic goes here
        // use _context.stuff to access stuff from PlayerStateMachine
        UnityEngine.Debug.Log("Player Jumps moving forward and animations Plays (depending on what condition lmao) use ur imagination");
    }

    private void HandleGravity()
    {
        if (_ctx.IsGrounded)
        {

        }
    }
}