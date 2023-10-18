using System;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;

public class PlayerStateManager
{
    private PlayerStateMachine _context;
    Dictionary<Type, PlayerBaseState> _states = new();

    public PlayerStateManager(PlayerStateMachine context)
    {
        _context = context;
        _states[typeof(PlayerGroundedState)] = new PlayerGroundedState(_context, this);
        _states[typeof(PlayerGroundedIdleState)] = new PlayerGroundedIdleState(_context, this);
        _states[typeof(PlayerWalkState)] = new PlayerWalkState(_context, this);
        _states[typeof(PlayerRunState)] = new PlayerRunState(_context, this);
        _states[typeof(PlayerFreeFallState)] = new PlayerFreeFallState(_context, this);
        _states[typeof(PlayerParkourState)] = new PlayerParkourState(_context, this);
        _states[typeof(PlayerJumpFromEdgeState)] = new PlayerJumpFromEdgeState(_context, this);

    }
    public PlayerBaseState Grounded()
    {
        return new PlayerGroundedState(_context, this);
    }

    public PlayerBaseState Idle()
    {
        return new PlayerGroundedIdleState(_context, this);
    }

    public PlayerBaseState Walk()
    {
        return new PlayerWalkState(_context, this);
    }

    public PlayerBaseState Run()
    {
        return new PlayerRunState(_context, this);
    }

    public PlayerBaseState Parkour()
    {
        return new PlayerParkourState(_context, this);
    }

    internal PlayerBaseState JumpFromEdge()
    {
        return new PlayerJumpFromEdgeState(_context, this);
    }

    public PlayerBaseState FreeFall()
    {
        return new PlayerFreeFallState(_context, this);
    }
}
