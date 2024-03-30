using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterSpaceState : PlayerState
{
    private float horizontalInput;
    private float verticalInput;

    public OuterSpaceState(FirstPersonController character, PlayerStateMachine stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        horizontalInput = verticalInput = 0.0f;
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void HandleInput()
    {
        base.HandleInput();
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
    }

    public override void MoveUpdate()
    {
        base.MoveUpdate();
        character.MoveBody(Vector3.zero);
        if (!WorldPhysics.IsInOuterSpace(character.GetTransform().position)) 
        {
            if (WorldPhysics.GetDepth(character.GetTransform().position) < 0.0f) stateMachine.ChangeState(character.GetState(FirstPersonController.State.WALKING));
            else stateMachine.ChangeState(character.GetState(FirstPersonController.State.INNER_FLYING));
        }
        character.RotateHead();
    }
}
