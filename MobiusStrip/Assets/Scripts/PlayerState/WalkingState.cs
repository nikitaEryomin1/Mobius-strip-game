using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : PlayerState
{
    const float JUMP_ACCEL_COEF = 280.0f;
    const float JUMP_DEPTH = 0.2f;
    private const float WALK_ACCEL_COEF = 18.0f;   
    private const float RUN_COEF = 2.0f; 
    private const float GRAVITY_SMOOTH = 0.92f;
    private float horizontalInput;
    private float verticalInput;
    private bool jump = false;
    private bool isRunning = false;

    public WalkingState(FirstPersonController character, PlayerStateMachine stateMachine) : base(character, stateMachine)
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
        jump = false;
    }

    public override void HandleInput()
    {
        base.HandleInput();
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        if(Input.GetButtonDown("Jump")) jump = true;
        isRunning = Input.GetKey(KeyCode.LeftShift);

    }

    public override void MoveUpdate()
    {
        base.MoveUpdate();
        Vector3 forward = Vector3.Cross(WorldPhysics.GetLocalGravity(character.GetTransform().position), character.GetTransform().right);
        Vector3 side = Vector3.Cross(forward, WorldPhysics.GetLocalGravity(character.GetTransform().position));
        Vector3 accelerationForw = verticalInput * forward;
        Vector3 accelerationSide = horizontalInput * side;
        Vector3 acceleration = accelerationForw + accelerationSide;
        if (acceleration.sqrMagnitude > 1.0) acceleration.Normalize(); //Исключаем быстрый бег по диагонали
        float accelCoef = isRunning ? WALK_ACCEL_COEF * RUN_COEF : WALK_ACCEL_COEF;
        acceleration *= accelCoef;
         if (jump && WorldPhysics.GetDepth(character.GetTransform().position) > JUMP_DEPTH)
        {
            jump = false;       
        }       
        if (jump && WorldPhysics.GetDepth(character.GetTransform().position) < JUMP_DEPTH)
        {
            acceleration += WorldPhysics.GetLocalGravity(character.GetTransform().position) * JUMP_ACCEL_COEF;       
        }
        character.MoveBody(acceleration);
        if (WorldPhysics.IsInOuterSpace(character.GetTransform().position)) 
        {
            stateMachine.ChangeState(character.GetState(FirstPersonController.State.OUTER_FLYING));
        }
        else if (WorldPhysics.GetDepth(character.GetTransform().position) < 0.0)
        {
            stateMachine.ChangeState(character.GetState(FirstPersonController.State.INNER_FLYING));
        }
        character.RotateHead(GRAVITY_SMOOTH);
    }
}
