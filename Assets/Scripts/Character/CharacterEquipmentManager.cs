using UnityEngine;

namespace KrazyKatGames
{
    public class CharacterEquipmentManager : MonoBehaviour
    {
        private CharacterManager character;


        public virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
  
        }

        public virtual void Start()
        {
      
        }

        public virtual void EquipWeaponItem(WeaponItem weaponItem)
        {
           
        }

    }
}