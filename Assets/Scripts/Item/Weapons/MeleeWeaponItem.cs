using System;
using UnityEngine;

namespace KrazyKatGames
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Items/EquippableItems/WeaponItem/MeleeWeaponItem")]
    public class MeleeWeaponItem : WeaponItem
    {
        public string[] attackCombos; 
        public string[] airAttackCombos; 

    }
}