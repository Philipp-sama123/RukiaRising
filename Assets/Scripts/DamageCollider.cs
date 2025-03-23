using System.Collections.Generic;
using UnityEngine;

namespace KrazyKatGames
{
    public class DamageCollider : MonoBehaviour
    {
        public Collider damageCollider;
        public DamageColliderType damageColliderType;
        public ParticleSystem trailEffect;

        [Header("Particle Effect Prefab")]
        public GameObject damageParticlePrefab; // Assign your particle system prefab here.

        [Header("Contact Point")]
        protected Vector3 contactPoint;

        // Track which characters have been hit
        private HashSet<CharacterManager> hitCharacters;

        protected virtual void Awake()
        {
            if (damageCollider == null)
                damageCollider = GetComponent<Collider>();

            // Initialize the set for tracking hits
            hitCharacters = new HashSet<CharacterManager>();

            // Initially disable the collider (optional, depending on activation logic)
            damageCollider.enabled = false;
        }

        /// <summary>
        /// Activates the damage collider and resets hit tracking.
        /// Call this method when the collider should be enabled.
        /// </summary>
        public void EnableDamageCollider()
        {
            if (trailEffect != null)
                trailEffect.Play();

            damageCollider.enabled = true;
            hitCharacters.Clear(); // Clear previously hit characters
        }

        /// <summary>
        /// Deactivates the damage collider.
        /// Call this method when the collider should be disabled.
        /// </summary>
        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;

            if (trailEffect != null)
                trailEffect.Stop();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            Debug.Log(
                $" OnTriggerEnter {other.name} of '{other.transform.root.name}' has been hit by the damage collider of {damageCollider.transform.root.name}");

            // Find the character manager on the other object
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            // Exit if no character manager is found or the character was already hit
            if (damageTarget == null || hitCharacters.Contains(damageTarget))
                return;

            // Mark this character as hit
            hitCharacters.Add(damageTarget);

            Debug.Log($"{damageTarget.name} has been hit by the damage collider");

            // Find the collision contact point
            contactPoint = other.ClosestPoint(transform.position);

            // Instantiate particle effect prefab at the contact point
            if (damageParticlePrefab != null)
            {
                Instantiate(damageParticlePrefab, contactPoint, Quaternion.identity);
            }
            else
            {
                Debug.LogError("No particle effect prefab assigned to the DamageCollider.");
            }

            // Play hit sound
            if (damageTarget.audioSource != null && damageTarget.characterCombatManager.hitSound != null)
            {
                damageTarget.audioSource.PlayOneShot(damageTarget.characterCombatManager.hitSound);
            }

            Debug.Log($"Damage applied to {damageTarget.name} at {contactPoint}");

            // Additional logic for applying damage can go here
        }
    }
}