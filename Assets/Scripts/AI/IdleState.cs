using UnityEngine;

namespace KrazyKatGames
{
    [CreateAssetMenu(menuName = "AI/States/Idle")]
    public class IdleState : AIState
    {
        private float timeInIdleState = 0f; // Tracks time spent in the Idle state
        public float idleDurationBeforePatrol = 10f; // Time before switching to PatrolState
        
        public override AIState Tick(AICharacterManager aiCharacter)
        {

            aiCharacter.aiCharacterCombatManager.FindATargetViaLineOfSight(aiCharacter);

            if (aiCharacter.aiCharacterCombatManager.currentTarget != null)
            {
                return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
            }

            timeInIdleState += Time.deltaTime;

            if (timeInIdleState >= idleDurationBeforePatrol)
            {
                Debug.Log("Idle duration exceeded. Transitioning to Patrol State.");
                return SwitchState(aiCharacter, aiCharacter.patrolState);
            }
            
            aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0, false);

            return this;
        }
        protected override void OnExitState(AICharacterManager aiCharacter)
        {
            base.OnExitState(aiCharacter);
            timeInIdleState = 0f;
        }
    }
}