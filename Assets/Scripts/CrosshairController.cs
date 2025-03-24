using UnityEngine;
using UnityEngine.UI;

namespace KrazyKatGames
{
    public class CrosshairController : MonoBehaviour
    {
        [SerializeField] private PlayerCombatManager playerCombat;
        [SerializeField] private Image crosshairImage;
        [SerializeField] private Camera mainCamera;

        [Header("Settings")]
        [SerializeField] private bool smoothMovement = true;
        [SerializeField] private float movementSmoothness = 5f;

        private RectTransform crosshairRect;
        private Vector3 screenPosition;

        private void Awake()
        {
            crosshairRect = crosshairImage.GetComponent<RectTransform>();
            if (!mainCamera) mainCamera = Camera.main;
        }

        private void Update()
        {
            UpdateCrosshairVisibility();
            UpdateCrosshairPosition();
        }

        private void UpdateCrosshairVisibility()
        {
            crosshairImage.enabled = playerCombat.hasBow && playerCombat.isAiming;
        }
        private void UpdateCrosshairPosition()
        {
            if (!crosshairImage.enabled) return;

            if (playerCombat.lockOnTarget != null)
            {
                Vector3 targetPosition = playerCombat.lockOnTarget.lockOnTransform.transform.position;
                screenPosition = mainCamera.WorldToScreenPoint(targetPosition);

                if (screenPosition.z < 0)
                {
                    crosshairImage.enabled = false;
                    return;
                }
            }
            else
            {
                Vector3 targetPosition = playerCombat.transform.position + playerCombat.transform.forward * 50f;
                screenPosition = mainCamera.WorldToScreenPoint(targetPosition);
            }

            if (smoothMovement)
            {
                crosshairRect.position = Vector3.Lerp(
                    crosshairRect.position,
                    screenPosition,
                    Time.deltaTime * movementSmoothness
                );
            }
            else
            {
                crosshairRect.position = screenPosition;
            }
        }
    }
}