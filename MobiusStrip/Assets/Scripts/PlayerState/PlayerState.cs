using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
        protected FirstPersonController character;
        protected PlayerStateMachine stateMachine;

        protected PlayerState(FirstPersonController character, PlayerStateMachine stateMachine)
        {
            this.character = character;
            this.stateMachine = stateMachine;
        }

        public virtual void Enter()
        {

        }

        public virtual void HandleInput()
        {

        }

        public virtual void MoveUpdate()
        {

        }

        public virtual void Exit()
        {

        }

}
