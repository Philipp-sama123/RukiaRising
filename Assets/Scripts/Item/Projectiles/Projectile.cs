using UnityEngine;

namespace KrazyKatGames
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        public bool isActive = false; // Controls if the projectile moves
        public float speed = 5f; // Speed of the projectile
        public Collider damageCollider;

        [Header("Particle Effect Prefab")]
        public GameObject damageParticlePrefab; // Assign your particle system prefab here.

        private Rigidbody rb;

        public void Awake()
        {
            if (damageCollider == null)
                damageCollider = GetComponent<Collider>();

            if (damageCollider != null)
                damageCollider.enabled = false; // Disable initially to avoid premature collisions

            rb = GetComponent<Rigidbody>();
        }

        public void FixedUpdate()
        {
            if (isActive)
            {
                rb.linearVelocity = transform.forward * speed; // Move the projectile
            }
            else
            {
                rb.linearVelocity = Vector3.zero; // Stop the projectile
            }
        }

        public void IgnoreCollisionWith(Collider colliderToIgnore)
        {
            if (damageCollider != null)
            {
                Physics.IgnoreCollision(damageCollider, colliderToIgnore);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!damageCollider.enabled) return; // Ensure damageCollider is active

            Debug.LogWarning($"Projectile hit: {other.name} from {other.transform.root.name}");

            // Find the character manager on the other object
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            if (damageTarget != null)
            {
                Vector3 contactPoint = other.ClosestPoint(transform.position);

                PlayEffects(damageTarget, contactPoint);

                Debug.LogWarning($"Damage applied to {damageTarget.name} at {contactPoint}");
            }

            Destroy(gameObject);
        }
        private void PlayEffects(CharacterManager damageTarget, Vector3 contactPoint)
        {
            if (damageTarget.audioSource != null && damageTarget.characterCombatManager.hitSound != null)
            {
                damageTarget.audioSource.PlayOneShot(damageTarget.characterCombatManager.hitSound);
            }

            if (damageParticlePrefab != null)
            {
                Instantiate(damageParticlePrefab, contactPoint, Quaternion.identity);
            }
        }
    }
}