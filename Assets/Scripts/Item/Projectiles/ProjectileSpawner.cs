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