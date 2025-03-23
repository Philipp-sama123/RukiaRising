using KrazyKatGames.Enemy;
using UnityEngine;
using UnityEngine.AI;

namespace KrazyKatGames
{
    [CreateAssetMenu(menuName = "AI/States/Patrol State")]
    public class PatrolState : AIState
    {
        private Vector3 patrolWaypoint;

        protected override void OnEnterState(AICharacterManager aiCharacter)
        {
            base.OnEnterState(aiCharacter);

            GeneratePatrolWaypoint(aiCharacter);

            if (aiCharacter.aICharacterLocomotionManager != null)
            {
                aiCharacter.aICharacterLocomotionManager.StartPath(patrolWaypoint);
            }
        }

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction)
                return this;

            aiCharacter.aiCharacterCombatManager.FindATargetViaLineOfSight(aiCharacter);

            if (aiCharacter.aiCharacterCombatManager.currentTarget != null)
            {
                return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
            }

            aiCharacter.aICharacterLocomotionManager.UpdatePathMovement(aiCharacter.aICharacterLocomotionManager.patrolSpeed,
                aiCharacter.aICharacterLocomotionManager.rotationSpeed);
            aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0.5f, false);
            
            if (AICharacterLocomotionManager.IsAtPosition(
                    aiCharacter.characterController.transform.position,
                    patrolWaypoint,
                    aiCharacter.aICharacterLocomotionManager.waypointTolerance))
            {
                return SwitchState(aiCharacter, aiCharacter.idleState);
            }
            return this;
        }

        private void GeneratePatrolWaypoint(AICharacterManager aiCharacter)
        {
            Vector3 origin = aiCharacter.transform.position;
            Vector3 randomPoint = origin + new Vector3(
                Random.Range(-aiCharacter.aICharacterLocomotionManager.patrolRadius, aiCharacter.aICharacterLocomotionManager.patrolRadius),
                0,
                Random.Range(-aiCharacter.aICharacterLocomotionManager.patrolRadius, aiCharacter.aICharacterLocomotionManager.patrolRadius)
            );

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, aiCharacter.aICharacterLocomotionManager.patrolRadius, NavMesh.AllAreas))
            {
                patrolWaypoint = hit.position;
            }
            else
            {
                patrolWaypoint = origin + new Vector3(5, 0, 5); // Default fallback waypoint
                Debug.LogWarning("Could not find a valid NavMesh position. Using fallback waypoint.");
            }
        }
    }
}