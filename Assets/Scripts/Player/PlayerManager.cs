namespace KrazyKatGames
{
    public class PlayerManager : CharacterManager
    {
        public PlayerLocomotionManager playerLocomotionManager;
        public PlayerEquipmentManager playerEquipmentManager;
        public PlayerAnimatorManager playerAnimatorManager;
        public PlayerCombatManager playerCombatManager;
        public PlayerCamera playerCamera;
        public LockOnTransform lockOnTransform;

        public bool isStrafing = true;

        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();

            lockOnTransform = GetComponentInChildren<LockOnTransform>();
        }
        protected override void Start()
        {
            base.Start();


            playerCamera = PlayerCamera.instance;
        }
        protected override void Update()
        {
            base.Update();
            playerLocomotionManager.HandleAllMovement();

            animator.SetBool("HasBow", playerCombatManager.hasBow);
            animator.SetBool("IsAiming", playerCombatManager.isAiming);
            animator.SetBool("IsArmed", playerCombatManager.isArmed);
        }
    }
}