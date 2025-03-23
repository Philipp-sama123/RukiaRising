using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace KrazyKatGames
{
    // This is for Combos (!) 
    [CreateAssetMenu(menuName = "AI/States/Combat Stance State")]
    public class CombatStanceState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            return this;
        }


        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);
        }
    }
}