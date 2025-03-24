using UnityEngine;

namespace KrazyKatGames
{
    [CreateAssetMenu(menuName = "AI/States/Attack State")]
    public class AttackState : AIState
    {
        public float attackCooldownDuration = 2.5f;
        private float attackCooldownTimer = 0f;
        private bool hasAttacked = false;

        protected override void OnEnterState(AICharacterManager aiCharacter)
        {
            base.OnEnterState(aiCharacter);

            // If not on cooldown, trigger the attack.
            if (attackCooldownTimer <= 0f)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("LightAttack_01", true);
                attackCooldownTimer = attackCooldownDuration;
                hasAttacked = true;
            }
        }

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // If still performing the attack animation, wait.
            if (aiCharacter.isPerformingAction)
            {
                return this;
            }

            if (attackCooldownTimer > 0f)
            {
                attackCooldownTimer -= Time.deltaTime;
                aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0, false);
                return this;
            }

            // Once the cooldown expires and we've attacked once, switch state.
            if (hasAttacked)
            {
                return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
            }

            // (Optional) If for any reason no attack occurred, trigger an attack.
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("LightAttack_01", true);
            attackCooldownTimer = attackCooldownDuration;
            hasAttacked = true;
            return this;
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);
            // Reset attack flag so that the state can be re-entered properly
            hasAttacked = false;
            attackCooldownTimer = 0f;
        }
    }
}
