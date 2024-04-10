using System;
using System.Linq;
using UnityEngine;
using SLOTC.Core.Saving;
using SLOTC.Utils;

namespace SLOTC.Core.Inventory
{
    [Serializable]
    public struct InventorySlot
    {
        public InventoryItem item;
        public int qty;
    }



    public class Inventory : MonoBehaviour, ISaveable
    {
        [SerializeField] InventorySlot[] _slots;

        public event Action OnInventoryChanged;

        public static Inventory GetPlayerInventory()
        {
            return GameObject.FindWithTag(Tags.Player).GetComponent<Inventory>();
        }

        public bool HasSpaceFor(InventoryItem item)
        {
            return FindSlot(item) >= 0;
        }

        public int GetSize()
        {
            return _slots.Length;
        }

        public bool AddToFirstEmptySlot(InventoryItem item, int qty)
        {
            int i = FindSlot(item);

            if (i < 0)
            {
                return false;
            }

            _slots[i].item = item;
            _slots[i].qty += qty;
            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool HasItem(InventoryItem item)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                InventoryItem invItem = _slots[i].item;
                if (invItem != null && invItem.ItemID == item.ItemID)
                {
                    return true;
                }
            }
            return false;
        }

        public InventoryItem GetItemInSlot(int slot)
        {
            return _slots[slot].item;
        }

        public int GetQtyInSlot(int slot)
        {
            return _slots[slot].qty;
        }

        public void RemoveFromSlot(int slot, int qty)
        {
            _slots[slot].qty -= qty;
            if (_slots[slot].qty <= 0)
            {
                _slots[slot].qty = 0;
                _slots[slot].item = null;
            }
            OnInventoryChanged?.Invoke();
        }

        public bool AddItemToSlot(int slot, InventoryItem item, int qty)
        {
            if (_slots[slot].item != null)
            {
                return AddToFirstEmptySlot(item, qty);
            }

            var i = FindStack(item);
            if (i >= 0)
            {
                slot = i;
            }

            _slots[slot].item = item;
            _slots[slot].qty += qty;
            OnInventoryChanged?.Invoke();
            return true;
        }

        private int FindSlot(InventoryItem item)
        {
            int i = FindStack(item);
            if (i < 0)
            {
                i = FindEmptySlot();
            }

            return i;
        }

        private int FindEmptySlot()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].item == null)
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindStack(InventoryItem item)
        {
            if (!item.Stackable)
            {
                return -1;
            }

            for (int i = 0; i < _slots.Length; i++)
            {
                InventoryItem invItem = _slots[i].item;
                if (invItem != null && invItem.ItemID == item.ItemID)
                {
                    return i;
                }
            }
            return -1;
        }

        public object CaptureState()
        {
            var slotStrings = new InventorySlotRecord[_slots.Length];
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].item != null)
                {
                    slotStrings[i].itemID = _slots[i].item.ItemID;
                    slotStrings[i].qty = _slots[i].qty;
                }
            }

            return slotStrings;
        }

        public void RestoreState(object state)
        {
            var slotStrings = (InventorySlotRecord[])state;
            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i].item = InventoryItem.GetFromID(slotStrings[i].itemID);
                _slots[i].qty = slotStrings[i].qty;
            }
            OnInventoryChanged?.Invoke();
        }

        [System.Serializable]
        private struct InventorySlotRecord
        {
            public string itemID;
            public int qty;
        }
    }
}
