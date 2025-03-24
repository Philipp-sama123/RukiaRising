using System;
using UnityEngine;

namespace KrazyKatGames
{
    public class CharacterManager : MonoBehaviour
    {
        public Animator animator;
        public AudioSource audioSource;
        public CharacterController characterController;
        public CharacterCombatManager characterCombatManager;
        public CharacterAnimatorManager characterAnimatorManager;
        public CharacterLocomotionManager characterLocomotionManager;
        public CharacterEquipmentManager characterEquipmentManager;
        public LockOnTransform lockOnTransform;

        public float currentStamina = 100;
        public float maxStamina = 100;
        public float currentHealth = 100;
        public float maxHealth = 100;

        public bool canRotate = true;
        public bool canMove = true;
        public bool canDoCombo = true;

        public bool isGrounded;
        public bool isJumping;
        public bool isPerformingAction;
        public bool isSprinting;
        public bool isDodging;
        public bool isDead;
        public bool applyRootMotion;
        protected virtual void Awake()
        {
            characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
            characterEquipmentManager = GetComponent<CharacterEquipmentManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterCombatManager = GetComponent<CharacterCombatManager>();
            characterController = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            
            lockOnTransform = GetComponentInChildren<LockOnTransform>();

        }
        protected virtual void Update()
        {
            animator.SetBool("IsGrounded", isGrounded);
        }
        protected virtual void Start()
        {
        }
    }
}