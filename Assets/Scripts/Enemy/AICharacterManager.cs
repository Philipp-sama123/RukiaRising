using System;
using KrazyKatGames.Enemy;
using UnityEngine;
using UnityEngine.AI;

namespace KrazyKatGames
{
    public class AICharacterManager : CharacterManager
    {
        public AICharacterCombatManager aiCharacterCombatManager;
        public AICharacterLocomotionManager aICharacterLocomotionManager;

        [Header("AI Settings")]
        public AIState currentAIState;
        public IdleState idleState;
        public CombatStanceState combatStanceState;
        public PursueTargetState pursueTargetState;
        public PatrolState patrolState;
        public AttackState attackState;
        public float stoppingDistance =1;

        protected override void Awake()
        {
            base.Awake();

            aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
            aICharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();
        }

        protected override void Start()
        {
            base.Start();

            currentAIState = idleState;
        }
        private void FixedUpdate()
        {
            ProcessStateMachine();
        }
        private void ProcessStateMachine()
        {
            AIState nextState = currentAIState?.Tick(this);

            if (nextState != null)
            {
                currentAIState = nextState;
            }
            // the position/rotation should be reset after the state machine has processed it
            // navMeshAgent.transform.localPosition = Vector3.zero;
            // navMeshAgent.transform.localRotation = Quaternion.identity;
        }
    }
}