using UnityEngine;

namespace KrazyKatGames
{
    public class AIState : ScriptableObject
    {
        protected virtual void OnEnterState(AICharacterManager aiCharacter)
        {
            Debug.Log($"{aiCharacter.name} entered state: {this.name}");
        }

        protected virtual void OnExitState(AICharacterManager aiCharacter)
        {
            Debug.Log($"{aiCharacter.name} exited state: {this.name}");
        }

        public virtual AIState Tick(AICharacterManager aiCharacter)
        {
            return this;
        }

        protected virtual AIState SwitchState(AICharacterManager aiCharacter, AIState newState)
        {
            Debug.Log($"{aiCharacter.name} switching from {this.name} to {newState.name}");
            
            // Exit the current state
            OnExitState(aiCharacter);

            // Enter the new state
            newState.OnEnterState(aiCharacter);

            return newState;
        }

        protected virtual void ResetStateFlags(AICharacterManager aiCharacter)
        {
        }
    }
}