using UnityEngine;
using SLOTC.Core.Stats;
using Unity.VisualScripting;
using System;

namespace SLOTC.Core.Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Equipable Item")]
    public class EquipableItem : InventoryItem
    {
        [SerializeField] StatModifier[] _statModifiers;
        [SerializeField] EquipLocation _equipLocations;

        public EquipLocation EquipLocation { get { return _equipLocations; } }
        public  StatModifier[] StatModifiers { get { return (StatModifier[])_statModifiers.Clone(); } }
    }
}