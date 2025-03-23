using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KrazyKatGames
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;

        public PlayerManager player;
        public Camera cameraObject;
        public Transform cameraPivotTransform;

        [Header("Camera Settings")]
        public float leftAndRightRotationSpeed = 220;
        [SerializeField] float upAndDownRotationSpeed = 220;
        [SerializeField] float minimumPivot = -30; //  THE LOWEST POINT YOU ARE ABLE TO LOOK DOWN
        [SerializeField] float maximumPivot = 60; //  THE HIGHEST POINT YOU ARE ABLE TO LOOK UP

        [SerializeField]
        private float cameraSmoothSpeed = 1; // THE BIGGER THIS NUMBER, THE LONGER FOR THE CAMERA TO REACH ITS POSITION DURING MOVEMENT

        [Header("Camera Values")]
        private Vector3 cameraVelocity;
        private float leftAndRightLookAngle;
        private float upAndDownLookAngle;


        [SerializeField] float lockOnTargetFollowSpeed = 0.2f;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            // Lock the cursor and make it invisible
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
        private void LateUpdate()
        {
            HandleAllCameraActions();
        }
        public void HandleAllCameraActions()
        {
            if (player != null)
            {
                HandleRotations();
                HandleFollowTarget();
            }
        }


        private void HandleRotations()
        {
            if (player.playerCombatManager.lockOnTarget == null)
            {
                //  ROTATE LEFT AND RIGHT BASED ON HORIZONTAL MOVEMENT ON THE RIGHT JOYSTICK
                leftAndRightLookAngle += (PlayerInputManager.Instance.cameraHorizontal_Input * leftAndRightRotationSpeed) * Time.deltaTime;
                //  ROTATE UP AND DOWN BASED ON VERTICAL MOVEMENT ON THE RIGHT JOYSTICK
                upAndDownLookAngle -= (PlayerInputManager.Instance.cameraVertical_Input * upAndDownRotationSpeed) * Time.deltaTime;
                //  CLAMP THE UP AND DOWN LOOK ANGLE BETWEEN A MIN AND MAX VALUE
                upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);


                Vector3 cameraRotation = Vector3.zero;
                Quaternion targetRotation;

                //  ROTATE THIS GAMEOBJECT LEFT AND RIGHT
                cameraRotation.y = leftAndRightLookAngle;
                targetRotation = Quaternion.Euler(cameraRotation);
                transform.rotation = targetRotation;

                //  ROTATE THE PIVOT GAMEOBJECT UP AND DOWN
                cameraRotation = Vector3.zero;
                cameraRotation.x = upAndDownLookAngle;
                targetRotation = Quaternion.Euler(cameraRotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
            else
            {
          
                // Lock-on behavior
                Vector3 rotationDirection = player.playerCombatManager.lockOnTarget.transform.position - transform.position;
                rotationDirection.Normalize();
                rotationDirection.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

                // Handle camera pivot for lock-on with clamped X rotation
                rotationDirection = player.playerCombatManager.lockOnTarget.transform.position - cameraPivotTransform.position;
                rotationDirection.Normalize();

                // Calculate the target rotation for the pivot
                Quaternion pivotTargetRotation = Quaternion.LookRotation(rotationDirection);

                // Extract and clamp only the X rotation
                float clampedXAngle = Mathf.Clamp(pivotTargetRotation.eulerAngles.x, minimumPivot, maximumPivot);

                // Preserve Y and Z of the current local rotation
                Vector3 currentEulerAngles = cameraPivotTransform.localEulerAngles;
                cameraPivotTransform.localRotation = Quaternion.Euler(clampedXAngle, currentEulerAngles.y, currentEulerAngles.z);

                // Save rotations for smooth transitions when unlocking
                leftAndRightLookAngle = transform.eulerAngles.y;
                upAndDownLookAngle = clampedXAngle;
            }
        }
        private void HandleFollowTarget()
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity,
                cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition;
        }
    }
}