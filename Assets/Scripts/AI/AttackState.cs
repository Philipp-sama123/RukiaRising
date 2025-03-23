using UnityEngine;

namespace KrazyKatGames
{
    [CreateAssetMenu(menuName = "AI/States/Attack State")]
    public class AttackState : AIState
    {
        protected override void OnEnterState(AICharacterManager aiCharacter)
        {
            base.OnEnterState(aiCharacter);
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("LightAttack_01",true);
        }
        // the order of operations is here very important
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction)
                return this;
            
            if (aiCharacter.aiCharacterCombatManager.currentTarget != null)
            {
                return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
            }
            else
            {
                return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
            }
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);
        }
    }
}