using HoaxGames;
using UnityEngine;

namespace KrazyKatGames
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
       protected CharacterManager character;

        [Header("Ground Check & Jumping")]
        public Vector3 yVelocity; // THE FORCE AT WHICH OUR CHARACTER IS PULLED UP OR DOWN (Jumping or Falling)
        [SerializeField]
        public float gravityForce = -15f;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] float groundCheckSphereRadius = 1;
        [SerializeField] float groundedYVelocity = -20f; // THE FORCE AT WHICH OUR CHARACTER IS STICKING TO THE GROUND WHILST THEY ARE GROUNDED
        [SerializeField] float fallStartYVelocity = -5;
        protected bool fallingVelocityHasBeenSet = false;
        [SerializeField] protected float inAirTimer = 0;
        public FootIK footIk;


        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
        protected virtual void Start()
        {
            footIk = GetComponent<FootIK>();
        }

        protected virtual void Update()
        {
            HandleGroundCheck();

            if (character.isGrounded)
            {
                //  IF WE ARE NOT ATTEMPTING TO JUMP OR MOVE UPWARD
                if (yVelocity.y < 0)
                {
                    inAirTimer = 0;
                    fallingVelocityHasBeenSet = false;
                    yVelocity.y = groundedYVelocity;
                }
            }
            else
            {
                //  IF WE ARE NOT JUMPING, AND OUR FALLING VELOCITY HAS NOT BEEN SET
                if (!character.isJumping && !fallingVelocityHasBeenSet)
                {
                    fallingVelocityHasBeenSet = true;
                    yVelocity.y = fallStartYVelocity;
                }

                inAirTimer = inAirTimer + Time.deltaTime;
                character.animator.SetFloat("InAirTimer", inAirTimer);

                yVelocity.y += gravityForce * Time.deltaTime;
            }

            //  THERE SHOULD ALWAYS BE SOME FORCE APPLIED TO THE Y VELOCITY
            character.characterController.Move(yVelocity * Time.deltaTime);
        }

        protected void HandleGroundCheck()
        {
            // character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);

            if (footIk && footIk.getGroundedResult() != null)
            {
                character.isGrounded = footIk.getGroundedResult().isGrounded;
            }
            else
            {
                character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
            }
        }

        #region Animation Events
        public void EnableCanRotate()
        {
            character.canRotate = true;
        }
        public void DisableCanRotate()
        {
            character.canRotate = false;
        }
        #endregion
    }
}