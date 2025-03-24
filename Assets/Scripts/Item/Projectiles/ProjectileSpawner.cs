using System;
using UnityEngine;

namespace KrazyKatGames
{
    public class ProjectileSpawner : MonoBehaviour
    {
        private PlayerCombatManager _playerCombatManager;

        public Projectile projectilePrefab;
        private Projectile _currentProjectile;
        private void Awake()
        {
            _playerCombatManager = GetComponentInParent<PlayerCombatManager>();
        }
        public void SpawnProjectile()
        {
            _currentProjectile = Instantiate(projectilePrefab, transform.position, transform.rotation);

            // If lockedTarget exists, rotate projectile to face the target.
            if (_playerCombatManager.lockOnTarget != null)
            {
                Vector3 directionToTarget = (_playerCombatManager.lockOnTarget.lockOnTransform.transform.position - transform.position).normalized;
                _currentProjectile.transform.forward = directionToTarget;
            }
            else
            {
                // If no target is locked, use the player's forward direction.
                _currentProjectile.transform.forward = _playerCombatManager.transform.forward;
            }

            IgnoreCollisionBetweenArrowAndPlayer();

            _currentProjectile.transform.parent = null; // Ensure the projectile isn't parented to the spawner

            // Enable the projectile's collider and activate it
            if (_currentProjectile.damageCollider != null)
            {
                _currentProjectile.damageCollider.enabled = true;
            }

            _currentProjectile.isActive = true;
        }

        private void IgnoreCollisionBetweenArrowAndPlayer()
        {
            foreach (DamageCollider damageCollider in _playerCombatManager.DamageColliders)
            {
                if (damageCollider != null && damageCollider.damageCollider != null)
                {
                    _currentProjectile.IgnoreCollisionWith(damageCollider.damageCollider);
                }
            }
            foreach (Collider playerCollider in _playerCombatManager.PlayerColliders)
            {
                if (playerCollider != null)
                {
                    _currentProjectile.IgnoreCollisionWith(playerCollider);
                }
            }
        }
    }
}