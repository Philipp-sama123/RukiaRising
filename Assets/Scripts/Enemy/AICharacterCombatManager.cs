using UnityEngine;

namespace KrazyKatGames.Enemy
{
    public class AICharacterCombatManager : CharacterCombatManager
    {
        public CharacterManager currentTarget;

        [Header("Detection Settings")]
        public float detectionRadius = 20f; // How far the AI can see
        public float fieldOfViewAngle = 90f; // Field of view in degrees
        public LayerMask targetLayer; // Layer for valid targets
        public LayerMask obstacleLayer; // Layer for obstacles blocking line of sight
        public Transform eyePosition; // Position to start detection

        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();
        }
        protected void OnDisable()
        {
            if (this.gameObject == null || this == null)
            {
                Debug.LogWarning($"{this.name} is being destroyed.");
                return;
            }
            Debug.LogWarning($"{this.name} (AICharacterCombatManager) has been disabled.");
        }

        public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
        {
            if (eyePosition == null)
            {
                Debug.LogError("Eye position not assigned.");
                return;
            }

            Collider[] potentialTargets = Physics.OverlapSphere(
                eyePosition.position,
                detectionRadius,
                targetLayer
            );

            foreach (Collider target in potentialTargets)
            {
                // Skip if the target is the AI itself or its child
                if (target.transform.root == aiCharacter.transform.root)
                    continue;

                CharacterManager targetCharacter = target.GetComponentInParent<CharacterManager>();

                if (targetCharacter != null)
                {
                    Vector3 directionToTarget = (targetCharacter.transform.position - eyePosition.position).normalized;
                    float angleToTarget = Vector3.Angle(eyePosition.forward, directionToTarget);

                    // Add additional dot product check to ensure target is in front
                    float dotProduct = Vector3.Dot(eyePosition.forward, directionToTarget);

                    if (dotProduct > 0 && angleToTarget < fieldOfViewAngle / 2)
                    {
                        if (!Physics.Linecast(eyePosition.position, targetCharacter.transform.position, obstacleLayer))
                        {
                            currentTarget = targetCharacter;
                            Debug.Log($"Target: {targetCharacter.name}, Angle: {angleToTarget}, Direction: {directionToTarget}");
                            Debug.Log("Target found: " + currentTarget.name);
                            return;
                        }
                    }
                }
            }

            currentTarget = null;
        }

        private void OnDrawGizmosSelected()
        {
            if (eyePosition == null) return;

            // Draw detection radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(eyePosition.position, detectionRadius);

            // Draw field of view lines
            Gizmos.color = Color.cyan;
            Quaternion leftRayRotation = Quaternion.Euler(0, -fieldOfViewAngle / 2, 0);
            Quaternion rightRayRotation = Quaternion.Euler(0, fieldOfViewAngle / 2, 0);

            Vector3 leftRayDirection = leftRayRotation * eyePosition.forward * detectionRadius;
            Vector3 rightRayDirection = rightRayRotation * eyePosition.forward * detectionRadius;

            Gizmos.DrawRay(eyePosition.position, leftRayDirection);
            Gizmos.DrawRay(eyePosition.position, rightRayDirection);

            // Draw a line to the current target if there is one
            if (currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(eyePosition.position, currentTarget.transform.position);
            }
        }
    }
}