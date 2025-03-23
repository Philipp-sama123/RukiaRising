using UnityEngine;

namespace KrazyKatGames
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Items/EquippableItems/WeaponItems/WeaponItem")]
    public class WeaponItem : EquippableItem
    {
        public WeaponType weaponType;
        public GameObject weaponModel;

    }
}