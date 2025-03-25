using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KrazyKatGames
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        private PlayerManager player;

        public List<WeaponItem> weaponItems;
        public WeaponItem unarmedItem;

        public WeaponHolderSlot[] slots;
        public WeaponHolderSlot leftHandSlot;
        public WeaponHolderSlot rightHandSlot;

        private int currentIndex = -1;

        public override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
            slots = GetComponentsInChildren<WeaponHolderSlot>();

            if (slots == null || slots.Length == 0)
            {
                Debug.LogError("No WeaponHolderSlot components found on this character.");
            }
        }

        public override void Start()
        {
            base.Start();

            foreach (var slot in slots)
            {
                if (slot.weaponSlotType == WeaponSlotType.LeftHand)
                {
                    leftHandSlot = slot;
                }
                else if (slot.weaponSlotType == WeaponSlotType.RightHand)
                {
                    rightHandSlot = slot;
                }
            }

            if (weaponItems == null || weaponItems.Count == 0)
            {
                Debug.LogWarning("No equippable items available to initialize.");
                return;
            }

            if (unarmedItem == null)
            {
                Debug.LogError("Unarmed item is not assigned! Please assign it in the Inspector.");
                return;
            }

            // Initialize the current index and equip the unarmed item
            currentIndex = 0;
            EquipWeaponItem(unarmedItem);
        }

        public override void EquipWeaponItem(WeaponItem weaponItem)
        {
            if (weaponItem == null)
            {
                Debug.LogError("Attempted to equip a null item.");
                return;
            }
            
            // If an item is already equipped, unequip it first
            if (currentEquippedItem != null)
            {
                Debug.Log($"Unequipping current item: {currentEquippedWeaponItem.name}");
                UnequipWeaponItem();
                //return;
            }

            // Play equip animation
            player.characterAnimatorManager.PlayTargetActionAnimation("Equip", true, true);
            Debug.Log($"Equipping item: {weaponItem.name}");

            // Clear previous weapon models from both hand slots
            ClearWeaponSlot(leftHandSlot);
            ClearWeaponSlot(rightHandSlot);

            // Instantiate the new weapon model based on the type
            if (weaponItem.weaponType == WeaponType.Bow)
            {
                currentEquippedItem = Instantiate(weaponItem.weaponModel, leftHandSlot.transform);
                player.playerCombatManager.hasBow = true;
                player.playerCombatManager.isArmed = false;
            }
            else if (weaponItem.weaponType == WeaponType.Melee)
            {
                currentEquippedItem = Instantiate(weaponItem.weaponModel, rightHandSlot.transform);
                player.playerCombatManager.hasBow = false;
                player.playerCombatManager.isArmed = true;
            }
            else if (weaponItem.weaponType == WeaponType.Unarmed)
            {
                player.playerCombatManager.hasBow = false;
                player.playerCombatManager.isArmed = false;
                currentEquippedItem = null;
            }

            // Set combo amount from weapon if applicable
            MeleeWeaponItem meleeWeaponItem = weaponItem as MeleeWeaponItem;
            if (meleeWeaponItem != null)
            {
                player.playerCombatManager.maxComboCount = meleeWeaponItem.attackCombos.Length;
            }
            currentEquippedWeaponItem = weaponItem;

            IgnoreCollisionBetweenPlayerAndWeapon();
        }

        private void ClearWeaponSlot(WeaponHolderSlot slot)
        {
            if (slot.transform.childCount > 0)
            {
                foreach (Transform child in slot.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        public void UnequipWeaponItem()
        {
            player.characterAnimatorManager.PlayTargetActionAnimation("Unequip", true, true);
            Debug.Log($"Unequipping item: {currentEquippedWeaponItem.name}");

            ClearWeaponSlot(leftHandSlot);
            ClearWeaponSlot(rightHandSlot);

            // Destroy current equipped item if it exists (for safety)
            if (currentEquippedItem != null)
            {
                Destroy(currentEquippedItem);
            }

            // Reset combat manager states
            player.playerCombatManager.hasBow = false;
            player.playerCombatManager.isArmed = false;
            player.playerCombatManager.maxComboCount = 3; // default of unarmed 

            // Clear equipped item references
            currentEquippedItem = null;
            currentEquippedWeaponItem = null;
        }

        public void EquipNextWeaponItem()
        {
            if (weaponItems == null || weaponItems.Count == 0)
            {
                Debug.LogWarning("No equippable items available.");
                return;
            }

            // Increment the current index and wrap around
            currentIndex = (currentIndex + 1) % weaponItems.Count;

            // Equip the next item
            EquipWeaponItem(weaponItems[currentIndex]);
        }

        public void EquipPreviousWeaponItem()
        {
            if (weaponItems == null || weaponItems.Count == 0)
            {
                Debug.LogWarning("No equippable items available.");
                return;
            }

            // Decrement the current index and wrap around
            currentIndex = (currentIndex - 1 + weaponItems.Count) % weaponItems.Count;

            // Equip the previous item
            EquipWeaponItem(weaponItems[currentIndex]);
        }

        private void IgnoreCollisionBetweenPlayerAndWeapon()
        {
            if (currentEquippedItem != null)
            {
                Collider[] playerColliders = player.GetComponentsInChildren<Collider>();
                Collider weaponCollider = currentEquippedItem.GetComponentInChildren<Collider>();

                if (weaponCollider != null)
                {
                    foreach (Collider col in playerColliders)
                    {
                        Physics.IgnoreCollision(weaponCollider, col);
                    }
                }
            }
        }
    }
}
