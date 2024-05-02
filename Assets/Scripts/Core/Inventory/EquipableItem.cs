using UnityEngine;
using SLOTC.Core.Stats;

namespace SLOTC.Core.Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Equipable Item")]
    public class EquipableItem : InventoryItem, IStatModifier
    {
        [SerializeField] StatModifier[] _statModifiers;
        [SerializeField] EquipLocation _equipLocations;

        [Header("Weapon settings only")]
        [SerializeField] GameObject _weaponMeshPrefab;


        public EquipLocation EquipLocation { get { return _equipLocations; } }
        public GameObject WeaponMesh { get { return _weaponMeshPrefab; } }


        private void OnValidate()
        {
            if (_equipLocations == EquipLocation.Weapon && _weaponMeshPrefab == null)
            {
                Debug.LogWarning("EquipableLocation is Weapon but WeaponMeshPrefab is null");
            }
        }

        public string GetID()
        {
            return ItemID;
        }

        public StatModifier[] GetStatModifiers()
        {
            return _statModifiers;
        }
    }
}