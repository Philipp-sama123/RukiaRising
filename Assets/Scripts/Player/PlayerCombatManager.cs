using UnityEngine;

namespace KrazyKatGames
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        [SerializeField] private float lockOnRadius = 10f; // Radius to search for enemies

        public AICharacterManager lockOnTarget;

        private PlayerManager _player;
        private ProjectileSpawner _projectileSpawner;

        private int comboCount = 0;
        private const int MAX_COMBO = 3;

        /**
         * Animator direct set
         */
        public bool hasBow = false;
        public bool isAiming = false;
        public bool isArmed = false;


        protected override void Awake()
        {
            base.Awake();

            _player = GetComponent<PlayerManager>();
            _projectileSpawner = GetComponentInChildren<ProjectileSpawner>();
        }

        protected override void Start()
        {
            base.Start();
        }
        protected internal override void AttemptToPerformAttack()
        {
            if (_player.isDead)
                return;

            if (!_player.isGrounded)
            {
                HandleAirAttack();
                return;
            }

            if (hasBow && isAiming)
            {
                _player.playerAnimatorManager.PlayTargetActionAnimation("Bow_Shoot", true, true, true, false, .1f);
                isAiming = false;
                _projectileSpawner.SpawnProjectile();
                return;
            }
            if (hasBow)
            {
                // set is aiming to true 
                _player.playerAnimatorManager.PlayTargetActionAnimation("Bow_AimStart", true, true, true, false, .1f);
                return;
            }
            // GROUND ATTACKS

            HandleGroundAttack();
        }
        private void HandleAirAttack()
        {
            // AIR ATTACKS
            if (_player.canDoCombo && comboCount > 0)
            {
                PerformComboAttack(true);
            }
            else
            {
                StartNewAttackCombo(true);
            }
        }
        private void HandleGroundAttack()
        {
            if (_player.canDoCombo && comboCount > 0)
            {
                PerformComboAttack(false);
            }
            else
            {
                StartNewAttackCombo(false);
            }
        }
        private string GetGroundAttackAnimationName()
        {
            MeleeWeaponItem meleeWeapon = ((MeleeWeaponItem)_player.playerEquipmentManager.currentEquippedItem);

            if (meleeWeapon != null)
            {
                return meleeWeapon.attackCombos[comboCount - 1];
            }
            return $"Attack_0{comboCount}";
        }
        private string GetAirAttackAnimationName()
        {
            MeleeWeaponItem meleeWeapon = ((MeleeWeaponItem)_player.playerEquipmentManager.currentEquippedItem);

            if (meleeWeapon != null)
            {
                return meleeWeapon.airAttackCombos[comboCount - 1];
            }
            return $"Air_Attack_0{comboCount}";
        }
        private void StartNewAttackCombo(bool isAirAttack)
        {
            comboCount = 1;
            if (isAirAttack)
            {
                _player.playerLocomotionManager.yVelocity.y =
                    Mathf.Sqrt(_player.playerLocomotionManager.jumpHeight * -2 * _player.playerLocomotionManager.gravityForce);
            }

            string animationName = isAirAttack ? GetAirAttackAnimationName() : GetGroundAttackAnimationName();
            _player.playerAnimatorManager.PlayTargetActionAnimation(animationName, true, true, false, false, .1f);
            _player.audioSource.PlayOneShot(attackSound);
        }


        private void PerformComboAttack(bool isAirAttack)
        {
            if (comboCount < MAX_COMBO)
            {
                comboCount++;

                if (isAirAttack)
                {
                    _player.playerLocomotionManager.yVelocity.y =
                        Mathf.Sqrt(_player.playerLocomotionManager.jumpHeight * -2 * _player.playerLocomotionManager.gravityForce);
                }

                string animationName = isAirAttack ? GetAirAttackAnimationName() : GetGroundAttackAnimationName();
                _player.playerAnimatorManager.PlayTargetActionAnimation(animationName, true, true, false, false, 0.1f);
                _player.audioSource.PlayOneShot(attackSound);
            }
            else
            {
                comboCount = 0;
            }
        }

        public void FindAndLockOnClosestEnemy()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, lockOnRadius);
            AICharacterManager closestAICharacter = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider collider in hitColliders)
            {
                AICharacterManager aiCharacter = collider.GetComponent<AICharacterManager>();
                if (aiCharacter != null) // Ensure the collider has an EnemyManager
                {
                    float distance = Vector3.Distance(transform.position, aiCharacter.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestAICharacter = aiCharacter;
                    }
                }
            }

            if (closestAICharacter != null)
            {
                lockOnTarget = closestAICharacter;
                _player.isStrafing = true;
                Debug.Log($"Locked onto: {closestAICharacter.name}");
            }
            else
            {
                Debug.Log("No enemies found in range. Now Strafing!");
                _player.isStrafing = true;
            }
        }
        public void ClearLockOnTarget()
        {
            lockOnTarget = null;
            _player.isStrafing = false;
        }
    }
}