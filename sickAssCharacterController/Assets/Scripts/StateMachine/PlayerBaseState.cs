public abstract class PlayerBaseState
{
    protected PlayerStateMachine _ctx;
    protected PlayerStateFactory _stateFactory;
    private PlayerBaseState _currentSubState;
    private PlayerBaseState _currentSuperState;

    public PlayerBaseState(PlayerStateMachine context, PlayerStateFactory stateFactory)
    {
        _ctx = context;
        _stateFactory = stateFactory;
    }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    private void UpdateStates()
    {

    }
    protected void SwitchState(PlayerBaseState newState)
    {
        ExitState();
        newState.EnterState();
        _ctx._currentState = newState;
    }
    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        // assigning this state as the parent of it's new substate
        newSubState.SetSuperState(this);
    }
}
