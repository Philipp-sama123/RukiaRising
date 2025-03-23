using UnityEngine;
using UnityEngine.AI;

namespace KrazyKatGames
{
    [CreateAssetMenu(menuName = "AI/States/Pursue Target")]
    public class PursueTargetState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction)
                return this;

            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
            {
                return SwitchState(aiCharacter, aiCharacter.idleState);
            }
 
            Vector3 targetPosition = aiCharacter.aiCharacterCombatManager.currentTarget.transform.position;

            aiCharacter.aICharacterLocomotionManager.MoveToAndRotate(
                targetPosition,
                aiCharacter.aICharacterLocomotionManager.pursueSpeed,
                aiCharacter.aICharacterLocomotionManager.rotationSpeed
            );

            // Check if the AI is close enough to the target to stop
            if (Vector3.Distance(aiCharacter.transform.position, targetPosition) <= aiCharacter.stoppingDistance)
            {
                // Perform attack or switch to combat state (if implemented)
                aiCharacter.isPerformingAction = true;
                Debug.Log("Reached target. Preparing for combat...");
                return SwitchState(aiCharacter, aiCharacter.attackState); // Switch to a combat state
            }

            aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0.5f, false);
            return this;
        }
    }
}