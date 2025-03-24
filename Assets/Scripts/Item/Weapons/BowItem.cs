using UnityEngine;

namespace KrazyKatGames
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Items/EquippableItems/WeaponItem/BowItem")]
    public class BowItem : WeaponItem
    {
        private void Awake()
        {
            weaponType = WeaponType.Bow;
        }
    }
}