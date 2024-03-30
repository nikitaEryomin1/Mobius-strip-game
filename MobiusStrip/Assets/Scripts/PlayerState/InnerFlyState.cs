using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerFlyState : PlayerState
{

    public InnerFlyState(FirstPersonController character, PlayerStateMachine stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void HandleInput()
    {
    }

    public override void MoveUpdate()
    {
        base.MoveUpdate();
        character.MoveBody(Vector3.zero);
        if (WorldPhysics.GetDepth(character.GetTransform().position) >= 0.0f && !WorldPhysics.IsInOuterSpace(character.GetTransform().position))
        {
            stateMachine.ChangeState(character.GetState(FirstPersonController.State.WALKING));
        }       
        if (WorldPhysics.IsInOuterSpace(character.GetTransform().position))
        {
            stateMachine.ChangeState(character.GetState(FirstPersonController.State.OUTER_FLYING));    
        }
        character.RotateHead();
    }
}
