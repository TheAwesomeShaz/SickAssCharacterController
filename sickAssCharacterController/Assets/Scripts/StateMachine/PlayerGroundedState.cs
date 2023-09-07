using UnityEngine.EventSystems;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    bool isOnLedge;
    public PlayerGroundedState(PlayerStateMachine context, PlayerStateFactory playerStateFactory):base (context,playerStateFactory) {}

    public override void CheckSwitchStates()
    {
        // if player is grounded and presses jump then:
        // if player is infront of a climbable object then switch to parkour super state (which again has many sub states)
        // if player taps and releases jump btn while running then jump state then fall then if something in front it can switch to grab 
        //   which will be a state under the parkour super state ofc

        if (_ctx.InputManager.jumpInput)
        {
            SwitchState(_stateFactory.Jump());
        }
    }

    public override void EnterState()
    {

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
        CheckSwitchStates();
        LimitEdgeLedgeMovement();
    }

    private void LimitEdgeLedgeMovement()
    {
        isOnLedge = _ctx.EnvScanner.EdgeLedgeCheck(_ctx.DesiredMoveDirection, out LedgeHitData ledgeHitData);
        if (isOnLedge)
        {
            HandleEdgeLedgeMovement();
        }
    }

    // limits ledge movement, prevents player from falling down from ledge
    // TODO: add a looking down animation state to fix the falling off ledge error when brute force input and shee
    private void HandleEdgeLedgeMovement()
    {
        float signedLedgeNormalMoveAngle = Vector3.SignedAngle(_ctx.LedgeHitData.ledgeFaceHit.normal, _ctx.DesiredMoveDirection, Vector3.up);
        float ledgeNormalMoveAngle = Mathf.Abs(signedLedgeNormalMoveAngle);

        if (Vector3.Angle(_ctx.DesiredMoveDirection, _ctx.transform.forward) >= 80)
        {
            // dont move just rotate
            _ctx.MovementVelocity = Vector3.zero;
            return;
        }

        if (ledgeNormalMoveAngle < 60)
        {
            _ctx.MovementVelocity = Vector3.zero;
            _ctx.MoveDirection = Vector3.zero;
        }
        else if (ledgeNormalMoveAngle < 90)
        {
            // angle is btwn 60 and 90, so limit velocity to horizontal direction
            // cross product of normal and up vector gives us the left vector
            var left = Vector3.Cross(Vector3.up, _ctx.LedgeHitData.ledgeFaceHit.normal);
            var direction = left * Mathf.Sign(signedLedgeNormalMoveAngle);
            _ctx.MovementVelocity = _ctx.MovementVelocity.magnitude * direction;
            _ctx.MoveDirection = direction;
        }
    }
}
