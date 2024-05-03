using System;
using System.Collections.Generic;
using UnityEngine;
using SLOTC.Core.Stats;
using SLOTC.Core.Saving;
using UnityEngine.InputSystem;
using SLOTC.Utils;

namespace SLOTC.Core.Inventory
{
    public class Equipment : MonoBehaviour, ISaveable
    {
        [SerializeField] EquipableItem _defaultWeapon;
        //[SerializeField] EquipableItem temp;
        [SerializeField] GameObject _weaponHolder;

        private Dictionary<EquipLocation, EquipableItem> _equipments = new Dictionary<EquipLocation, EquipableItem>();
        private Status _status;

        public event Action OnEquipChanged;

        //private bool _equipping;

        private void OnValidate()
        {
            //if (_defaultWeapon != null && _defaultWeapon.EquipLocation != EquipLocation.Weapon)
            //    Debug.LogError("You set DefaultWeapon as " + _defaultWeapon.name + " which is not a weapon");
        }

        private void Awake()
        {
            _status = GetComponent<Status>();
        }

        private void Start()
        {
            Equip(_defaultWeapon);
        }

        public static Equipment GetPlayerEquipment()
        {
            return GameObject.FindWithTag(Tags.Player).GetComponent<Equipment>();
        }

        private void Update()
        {
            //if (Keyboard.current.tKey.wasPressedThisFrame)
            //{
            //    Unequip(EquipLocation.Weapon);
            //}
            //if (Keyboard.current.eKey.wasPressedThisFrame)
            //{
            //    Equip(temp);
            //}
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
            //_equipping = true;

            Unequip(equipableItem.EquipLocation);
            _status.AddModifiers(equipableItem);
            _equipments[equipableItem.EquipLocation] = equipableItem;

            if (equipableItem.EquipLocation == EquipLocation.Weapon)
                Instantiate(equipableItem.WeaponMesh, _weaponHolder.transform);

            OnEquipChanged?.Invoke();

            //_equipping = false;
            return true;
        }

        public bool Unequip(EquipLocation location)
        {
            if (!_equipments.ContainsKey(location))
                return false;

            EquipableItem item = _equipments[location];
            _status.RemoveModifiers(item);
            _equipments.Remove(location);

            if (location == EquipLocation.Weapon && _weaponHolder.transform.childCount > 0)
            {
                Destroy(_weaponHolder.transform.GetChild(0).gameObject);
                //// equip default if not equipping
                //if (!_equipping)
                //{
                //    Equip(_defaultWeapon);
                //}
            }

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
