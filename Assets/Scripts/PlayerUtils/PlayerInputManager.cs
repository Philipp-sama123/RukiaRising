using UnityEngine;
using UnityEngine.SceneManagement;

namespace KrazyKatGames
{
    public class PlayerInputManager : MonoBehaviour
    {
        //  SINGLETON
        public static PlayerInputManager Instance;

        //  INPUT CONTROLS
        private PlayerControls _playerControls;


        //  LOCAL PLAYER
        public PlayerManager player;

        [Header("Camera Movement Inputs")]
        [SerializeField] Vector2 camera_Input;

        public float cameraVertical_Input;
        public float cameraHorizontal_Input;

        [Header("Movement Inputs")]
        public Vector2 movementInput;
        public float vertical_Input;
        public float horizontal_Input;
        public float moveAmount;

        [Header("Action Inputs")]
        [SerializeField] bool dodge_Input = false;
        [SerializeField] bool sprint_Input = false;
        [SerializeField] bool jump_Input = false;
        [SerializeField] bool attack_Input = false;
        [SerializeField] bool interaction_Input = false;
        [SerializeField] bool previous_input = false;
        [SerializeField] bool next_input = false;
        [SerializeField] bool toggle_lock_on_input = false;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            if (_playerControls == null)
            {
                _playerControls = new PlayerControls();
                // Movement
                _playerControls.Player.Move.performed += i => movementInput = i.ReadValue<Vector2>();
                _playerControls.Player.Look.performed += i => camera_Input = i.ReadValue<Vector2>();
                // Actions
                _playerControls.Player.Dodge.performed += i => dodge_Input = true;
                _playerControls.Player.Jump.performed += i => jump_Input = true;
                _playerControls.Player.Attack.performed += i => attack_Input = true;
                _playerControls.Player.Previous.performed += i => previous_input = true;
                _playerControls.Player.Next.performed += i => next_input = true;
                // Interactions
                _playerControls.Player.Interact.performed += i => interaction_Input = true;

                //  HOLDING THE INPUT, SETS THE BOOL TO TRUE
                _playerControls.Player.Sprint.performed += i => sprint_Input = true;
                //  RELEASING THE INPUT, SETS THE BOOL TO FALSE
                _playerControls.Player.Sprint.canceled += i => sprint_Input = false;


                _playerControls.Player.ToggleStrafing.performed += i => toggle_lock_on_input = true;
            }

            _playerControls.Enable();
        }

        //  IF WE MINIMIZE OR LOWER THE WINDOW, STOP ADJUSTING INPUTS
        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    _playerControls.Enable();
                }
                else
                {
                    _playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandleAllInputs();
        }

        private void HandleAllInputs()
        {
            HandlePlayerMovementInput();
            HandleCameraMovementInput();

            HandleDodgeInput();
            HandleSprintInput();
            HandleJumpInput();

            HandleAttackInput();
            HandleInteractionInput();

            HandleLockOnInput();
            HandleNextOrPreviousInput();
        }
        private void HandleNextOrPreviousInput()
        {
            if (next_input)
            {
                next_input = false;
                player.playerEquipmentManager.EquipNextWeaponItem();
            }
            if (previous_input)
            {
                previous_input = false;
                player.playerEquipmentManager.EquipPreviousWeaponItem();
            }
        }
        //  MOVEMENT
        private void HandlePlayerMovementInput()
        {
            vertical_Input = movementInput.y;
            horizontal_Input = movementInput.x;

            //  RETURNS THE ABSOLUTE NUMBER, (Meaning number without the negative sign, so its always positive)
            moveAmount = Mathf.Clamp01(Mathf.Abs(vertical_Input) + Mathf.Abs(horizontal_Input));

            //  WE CLAMP THE VALUES, SO THEY ARE 0, 0.5 OR 1 (OPTIONAL)
            if (moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5 && moveAmount <= 1)
            {
                moveAmount = 1;
            }
            //moveAmount *= 0.5f;
        }
        private void HandleCameraMovementInput()
        {
            cameraVertical_Input = camera_Input.y;
            cameraHorizontal_Input = camera_Input.x;
        }

        //  ACTION
        private void HandleDodgeInput()
        {
            if (dodge_Input)
            {
                dodge_Input = false;
                player.playerLocomotionManager.AttemptToPerformDodge();
            }
        }
        private void HandleInteractionInput()
        {
            if (interaction_Input)
            {
                interaction_Input = false;
                // ToDo: Player Interact
            }
        }

        private void HandleAttackInput()
        {
            if (attack_Input)
            {
                attack_Input = false;
                player.playerCombatManager.AttemptToPerformAttack();
                // player.
            }
        }
        private void HandleSprintInput()
        {
            if (sprint_Input)
            {
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.isSprinting = false;
            }
        }
        private void HandleJumpInput()
        {
            if (jump_Input)
            {
                jump_Input = false;
                player.playerLocomotionManager.AttemptToPerformJump();
            }
        }
        private void HandleLockOnInput()
        {
            if (toggle_lock_on_input)
            {
                toggle_lock_on_input = false;

                if (player.playerCombatManager.lockOnTarget == null)
                {
                    player.playerCombatManager.FindAndLockOnClosestEnemy();
                }
                else
                {
                    player.playerCombatManager.ClearLockOnTarget();
                }
            }
        }
    }
}