using UnityEngine;

namespace KrazyKatGames
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        private PlayerManager _player;
        private Vector3 _moveDirection;
        private Vector3 _targetRotationDirection;
        private Vector3 _dodgeDirection;

        [Header("Movement Settings")]
        [SerializeField] private float walkingSpeed = 2f;
        [SerializeField] private float runningSpeed = 4f;
        [SerializeField] private float sprintingSpeed = 6f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float inAirMovementSpeedMultiplier = 0.35f;
        
        [SerializeField] private float lockOnRotationSpeed = 10f;

        [Header("Jump Settings")]
        [SerializeField]
        public float jumpHeight = 0.1f;

        [Header("Stamina Costs")]
        [SerializeField] private int dodgeStaminaCost = 10;
        [SerializeField] private float jumpStaminaCost = 10f;
        [SerializeField] private int sprintingStaminaCost = 2;

        protected override void Awake()
        {
            base.Awake();
            _player = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();
            UpdateAnimatorParameters();
        }

        private void UpdateAnimatorParameters()
        {
            if (_player.isStrafing)
            {
                _player.playerAnimatorManager.UpdateAnimatorMovementParameters(
                    PlayerInputManager.Instance.movementInput.x / 2,
                    PlayerInputManager.Instance.movementInput.y / 2,
                    _player.isSprinting
                );
            }
            else
            {
                _player.playerAnimatorManager.UpdateAnimatorMovementParameters(
                    0,
                    PlayerInputManager.Instance.moveAmount,
                    _player.isSprinting
                );
            }
        }

        public void HandleAllMovement()
        {
            if (!_player.canMove && !_player.canRotate) return;

            CalculateMovementDirection();

            if (!_player.playerCombatManager.lockOnTarget)
            {
                HandleMovement();
                HandleRotation();
            }
            else
            {
                HandleLockOnMovement();
                HandleLockOnRotation();
            }
        }

        private void CalculateMovementDirection()
        {
            if (_player.isStrafing)
                CalculateStrafingMovementDirection();
            else
                CalculateFreeMovementDirection();
        }

        private void CalculateStrafingMovementDirection()
        {
            Vector3 cameraForward = _player.playerCamera.transform.forward;
            Vector3 cameraRight = _player.playerCamera.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            _moveDirection = cameraForward * PlayerInputManager.Instance.movementInput.y +
                             cameraRight * PlayerInputManager.Instance.movementInput.x;
            _moveDirection.Normalize();
        }

        private void CalculateFreeMovementDirection()
        {
            _moveDirection = _player.playerCamera.transform.forward * PlayerInputManager.Instance.vertical_Input +
                             _player.playerCamera.transform.right * PlayerInputManager.Instance.horizontal_Input;
            _moveDirection.Normalize();
            _moveDirection.y = 0;
        }

        private void HandleMovement()
        {
            if (_player.isGrounded && !_player.isJumping)
                HandleGroundedMovement();
            else
                HandleInAirMovement();
        }
        private void HandleLockOnMovement()
        {
            if (_player.playerCombatManager.lockOnTarget == null || _player.isDead) return;

            // Use input for movement direction
            Vector3 cameraForward = _player.playerCamera.transform.forward;
            Vector3 cameraRight = _player.playerCamera.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            _moveDirection = cameraForward * PlayerInputManager.Instance.movementInput.y +
                             cameraRight * PlayerInputManager.Instance.movementInput.x;
            _moveDirection.Normalize();

            // Move the character
            float speed = _player.isSprinting? runningSpeed : walkingSpeed;
            _player.characterController.Move(_moveDirection * speed * Time.deltaTime);
        }

        private void HandleLockOnRotation()
        {
            if (_player.playerCombatManager.lockOnTarget == null || _player.isDead) return;

            // Calculate direction to the lock-on target
            Vector3 directionToTarget = _player.playerCombatManager.lockOnTarget.transform.position - transform.position;
            directionToTarget.y = 0; // Ignore vertical rotation

            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    lockOnRotationSpeed * Time.deltaTime
                );
            }
        }

        private void HandleGroundedMovement()
        {
            if (_player.isDead || !_player.canMove) return;

            float speed = GetCurrentSpeed();
            _player.characterController.Move(_moveDirection * speed * Time.deltaTime);
        }

        private void HandleInAirMovement()
        {
            float speed = GetCurrentSpeed() * inAirMovementSpeedMultiplier;
            _player.characterController.Move(_moveDirection * speed * Time.deltaTime);
        }

        private float GetCurrentSpeed()
        {
            float currentSpeed = PlayerInputManager.Instance.moveAmount > 0.5f ? runningSpeed : walkingSpeed;

            if (_player.isStrafing)
            {
                currentSpeed = walkingSpeed;
                if (_player.isSprinting)
                    currentSpeed = runningSpeed;
            }
            else if (_player.isSprinting)
            {
                currentSpeed = sprintingSpeed;
            }
            return currentSpeed;
        }

        private void HandleRotation()
        {
            if (_player.isDead || !_player.canRotate) return;

            if (_player.isStrafing)
                HandleStrafingRotation();
            else
                HandleFreeRotation();
        }

        private void HandleStrafingRotation()
        {
            Vector3 cameraForward = _player.playerCamera.cameraObject.transform.forward;
            cameraForward.y = 0;
            _targetRotationDirection = cameraForward.normalized;

            if (PlayerInputManager.Instance.movementInput != Vector2.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_targetRotationDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private void HandleFreeRotation()
        {
            _targetRotationDirection = _player.playerCamera.cameraObject.transform.forward * PlayerInputManager.Instance.vertical_Input +
                                       _player.playerCamera.cameraObject.transform.right * PlayerInputManager.Instance.horizontal_Input;
            _targetRotationDirection.Normalize();
            _targetRotationDirection.y = 0;

            if (_targetRotationDirection == Vector3.zero)
                _targetRotationDirection = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(_targetRotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public void HandleSprinting()
        {
            if (_player.isPerformingAction || _player.currentStamina <= 0)
            {
                _player.isSprinting = false;
                return;
            }

            _player.isSprinting = PlayerInputManager.Instance.moveAmount >= 0.5f;

            if (_player.isSprinting)
                _player.currentStamina -= sprintingStaminaCost * Time.deltaTime;
        }

        public void AttemptToPerformDodge()
        {
          //  if (_player.isPerformingAction || _player.currentStamina <= 0) return;

            if (_player.isStrafing)
            {
                Vector2 input = PlayerInputManager.Instance.movementInput;
                if (input.y > 0)
                    PerformForwardRoll();
                else if (input.y < 0 && Mathf.Abs(input.x) < 0.1f)
                    PerformBackRoll();
                else if (input.x > 0)
                    PerformRightRoll();
                else if (input.x < 0)
                    PerformLeftRoll();
                else PerformBackRoll();
            }
            else
            {
                if (PlayerInputManager.Instance.moveAmount > 0)
                    PerformForwardRoll();
                else
                    PerformBackRoll();
            }

            _player.currentStamina -= dodgeStaminaCost;
        }

        protected internal void AlignCharacter()
        {
            Vector3 lookRotation = Vector3.zero;
            lookRotation = _player.playerCamera.cameraObject.transform.forward * PlayerInputManager.Instance.vertical_Input +
                           _player.playerCamera.cameraObject.transform.right * PlayerInputManager.Instance.horizontal_Input;
            lookRotation.y = 0;
            lookRotation.Normalize();

            _player.transform.rotation = Quaternion.LookRotation(lookRotation);
        }
        private void PerformForwardRoll()
        {
            AlignCharacter();
            _player.isDodging = true;
            _player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true, false, false);
        }

        private void PerformBackRoll()
        {
            _player.isDodging = true;
            _player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Backward_01", true, true, false, false);
        }
        private void PerformLeftRoll()
        {
            _player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Left_01", true, true, false, false);
            _player.isDodging = true;
        }
        private void PerformRightRoll()
        {
            _player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Right_01", true, true, false, false);
            _player.isDodging = true;
        }
        public void AttemptToPerformJump()
        {
            if (CanJump())
            {
                _player.currentStamina -= jumpStaminaCost;
                _player.playerAnimatorManager.PlayTargetActionAnimation("Jump_Start", false, true, true, true);
                _player.isJumping = true;
            }
        }
        private bool CanJump()
        {
            return !_player.isPerformingAction && _player.currentStamina > 0 && !_player.isJumping && _player.isGrounded;
        }
        public void ApplyJumpingVelocity()
        {
            yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
        }

        public void AlignCharacterToMoveDirection()
        {
            // Retrieve the look direction of the player
            Vector3 lookDirection = _player.lockOnTransform.transform.forward;

            // Ignore any vertical rotation by flattening the direction
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                // Rotate the player to face the look direction
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                _player.transform.rotation =
                    Quaternion.Slerp(_player.transform.rotation, targetRotation, Time.deltaTime * 250f); // Adjust rotation speed with Time.deltaTime
            }
        }
    }
}