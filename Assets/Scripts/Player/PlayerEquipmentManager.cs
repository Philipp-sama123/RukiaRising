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

            Debug.Log($"Equipping item: {weaponItem.name}");

            // Destroy the existing weapon model in the left-hand slot
            if (leftHandSlot.transform.childCount > 0)
            {
                foreach (Transform child in leftHandSlot.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            if (rightHandSlot.transform.childCount > 0)
            {
                foreach (Transform child in rightHandSlot.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            if (currentEquippedItem != null)
            {
                Destroy(currentEquippedItem);
            }

            // Instantiate the new weapon model
            if (weaponItem.weaponModel != null)
            {
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
            }
            currentEquippedWeaponItem = weaponItem;

            // as is safer to use than a direct cast!!
            MeleeWeaponItem meleeWeaponItem = weaponItem as MeleeWeaponItem;
            if (meleeWeaponItem != null)
            {
                player.playerCombatManager.maxComboCount = meleeWeaponItem.attackCombos.Length;
            }
            IgnoreCollisionBetweenPlayerAndWeapon();
            // Additional logic (if any) to handle stats, animations, etc., can be added here
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
    }
}