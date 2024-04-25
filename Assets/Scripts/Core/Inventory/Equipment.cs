using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SLOTC.Core.Stats;
using SLOTC.Core.Saving;

namespace SLOTC.Core.Inventory
{
    public class Equipment : MonoBehaviour, ISaveable
    {
        private Dictionary<EquipLocation, EquipableItem> _equipments = new Dictionary<EquipLocation, EquipableItem>();
        private Status _status;

        public event Action OnEquipChanged;

        private void Awake()
        {
            _status = GetComponent<Status>();
        }

        public bool HasEquipmentAt(EquipLocation location)
        {
            return _equipments.ContainsKey(location);
        }

        public EquipableItem GetEquipment(EquipLocation location)
        {
            if (_equipments.ContainsKey(location))
                return _equipments[location];

            return null;
        }

        public bool Equip(EquipableItem equipableItem)
        {
            Unequip(equipableItem.EquipLocation);
            _status.AddModifiers(equipableItem);
            _equipments[equipableItem.EquipLocation] = equipableItem;

            OnEquipChanged?.Invoke();
            return true;
        }

        public bool Unequip(EquipLocation location)
        {
            if (!_equipments.ContainsKey(location))
                return false;

            EquipableItem item = _equipments[location];
            _status.RemoveModifiers(item);
            _equipments.Remove(location);

            OnEquipChanged?.Invoke();
            return true;
        }

        public object CaptureState()
        {
            var equippedItemsForSerialization = new Dictionary<EquipLocation, string>();
            foreach (var pair in _equipments)
            {
                equippedItemsForSerialization[pair.Key] = pair.Value.ItemID;
            }

            return equippedItemsForSerialization;
        }

        public void RestoreState(object state)
        {
            _equipments = new Dictionary<EquipLocation, EquipableItem>();

            var equippedItemsForSerialization = (Dictionary<EquipLocation, string>)state;

            foreach (var pair in equippedItemsForSerialization)
            {
                var item = (EquipableItem)InventoryItem.GetFromID(pair.Value);
                if (item != null)
                {
                    Equip(item);
                }
            }
        }
    }
}
