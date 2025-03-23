using System;
using UnityEngine;

namespace KrazyKatGames
{
    public class CharacterCombatManager : MonoBehaviour
    {
        private CharacterManager _character;
        public DamageCollider[] DamageColliders { get; protected set; }
        public Collider[] PlayerColliders { get; protected set; }
        protected DamageCollider leftHandDamageCollider;
        protected DamageCollider rightHandDamageCollider;
        protected DamageCollider leftFootDamageCollider;
        protected DamageCollider rightFootDamageCollider;

        [Header("Sound Effects")]
        public AudioClip hitSound;
        public AudioClip attackSound;

        protected virtual void Awake()
        {
            _character = GetComponent<CharacterManager>();
        }
        protected virtual void Start()
        {
            DamageColliders = GetComponentsInChildren<DamageCollider>();
            PlayerColliders = _character.GetComponentsInChildren<Collider>();

            foreach (DamageCollider damageCollider in DamageColliders)
            {
                foreach (Collider playerCollider in PlayerColliders)
                {
                    Physics.IgnoreCollision(damageCollider.damageCollider, playerCollider);
                }
                // Explicitly ignore CharacterController collider
                Physics.IgnoreCollision(damageCollider.damageCollider, _character.characterController);
            }
            foreach (DamageCollider damageCollider in DamageColliders)
            {
                switch (damageCollider.damageColliderType)
                {
                    case DamageColliderType.LeftFoot:
                        leftFootDamageCollider = damageCollider;
                        break;
                    case DamageColliderType.RightFoot:
                        rightFootDamageCollider = damageCollider;
                        break;
                    case DamageColliderType.LeftHand:
                        leftHandDamageCollider = damageCollider;
                        break;
                    case DamageColliderType.RightHand:
                        rightHandDamageCollider = damageCollider;
                        break;
                    case DamageColliderType.MeleeWeapon:
                        //TODO!
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        protected virtual void Update()
        {
        }
        protected internal virtual void AttemptToPerformAttack()
        {
            if (_character.isPerformingAction) return;
            if (!_character.isGrounded) return;
            if (_character.isDead) return;
            _character.audioSource.PlayOneShot(attackSound);
            _character.characterAnimatorManager.PlayTargetActionAnimation("Attack_01", true);
            //   character.characterAnimatorManager.PlayTargetActionAnimation();
        }

        public void DisableAllDamageColliders()
        {
            foreach (DamageCollider damageCollider in DamageColliders)
            {
                damageCollider.DisableDamageCollider();
            }
        }

        /**
         * Call these methods from animation events
         */
        #region Animation Events
        public void EnableCanDoCombo()
        {
            _character.canDoCombo = true;
        }
        public void DisableCanDoCombo()
        {
            _character.canDoCombo = false;
        }
        public void EnableLeftHandDamageCollider()
        {
            leftHandDamageCollider.EnableDamageCollider();
        }
        public void EnableRightHandDamageCollider()
        {
            rightHandDamageCollider.EnableDamageCollider();
        }
        public void EnableLeftFootDamageCollider()
        {
            leftFootDamageCollider.EnableDamageCollider();
        }
        public void EnableRightFootDamageCollider()
        {
            rightFootDamageCollider.EnableDamageCollider();
        }
        #endregion
    }
}