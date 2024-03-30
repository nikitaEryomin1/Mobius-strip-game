using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
        private PlayerState currentState;

        public void Initialize(PlayerState startingState)
        {
            currentState = startingState;
            startingState.Enter();
        }

        public void ChangeState(PlayerState newState)
        {
            currentState.Exit();

            currentState = newState;
            newState.Enter();
        }
        public PlayerState GetState()
        {
            return currentState;
        }
}
