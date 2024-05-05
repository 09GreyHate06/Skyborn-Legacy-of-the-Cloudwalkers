
using SLOTC.Core.Saving;
using System;
using UnityEngine;

namespace SLOTC.Core.Inventory
{
    public class ActionItemBar : MonoBehaviour, ISaveable
    {
        private DockedItemSlot[] _dockedItemSlots = new DockedItemSlot[10];

        public event Action OnActionItemBarChanged;

        public ActionItem GetActionItem(int index)
        {
            if (index >= _dockedItemSlots.Length || index < 0)
                return null;

            return _dockedItemSlots[index].item;
        }

        public int GetQty(int index)
        {
            if (index >= _dockedItemSlots.Length || index < 0)
                return -1;

            return _dockedItemSlots[index].qty;
        }

        public void AddActionItem(InventoryItem item, int index, int qty)
        {
            if (item == null || index >= _dockedItemSlots.Length || index < 0) return;

            if (object.ReferenceEquals(_dockedItemSlots[index].item, item))
            {
                _dockedItemSlots[index].qty += qty;
            }
            else
            {
                var slot = new DockedItemSlot();
                slot.item = item as ActionItem;
                slot.qty = qty;
                _dockedItemSlots[index] = slot;
            }

            OnActionItemBarChanged?.Invoke();
        }

        public ActionItem Use(int index)
        {
            if (index >= _dockedItemSlots.Length || index < 0 || _dockedItemSlots[index].item == null) 
                return null;

            ActionItem item = _dockedItemSlots[index].item;
            if (_dockedItemSlots[index].item.IsConsumable)
                RemoveItems(index, 1);

            return item;
        }

        public bool RemoveItems(int index, int qty)
        {
            if (index >= _dockedItemSlots.Length || index < 0 || _dockedItemSlots[index].item == null) return false;

            _dockedItemSlots[index].qty -= qty;
            if (_dockedItemSlots[index].qty <= 0)
            {
                _dockedItemSlots[index] = new DockedItemSlot();
            }

            OnActionItemBarChanged?.Invoke();
            return true;
        }

        public int MaxAcceptable(InventoryItem item, int index)
        {
            var actionItem = item as ActionItem;
            if (!actionItem) return 0;

            if (index >= _dockedItemSlots.Length || index < 0)
                return 0;


            if (_dockedItemSlots[index].item != null && !object.ReferenceEquals(_dockedItemSlots[index].item, item))
                return 0;

            if (actionItem.Stackable)
            {
                return int.MaxValue;
            }
            if (_dockedItemSlots[index].item != null)
                return 0;

            return 1;
        }

        public object CaptureState()
        {
            SerializableDockedItemSlot[] state = new SerializableDockedItemSlot[_dockedItemSlots.Length];
            for (int i = 0; i < _dockedItemSlots.Length; i++)
            {
                var slot = _dockedItemSlots[i];
                if (!slot.item) continue;
                state[i].itemID = slot.item.ItemID;
                Debug.Log(slot.item.ItemID);
                state[i].qty = slot.qty;
            }

            return state;
        }

        public void RestoreState(object state)
        {
            var state_ = (SerializableDockedItemSlot[])state;
            for (int i = 0; i < _dockedItemSlots.Length; i++)
            {
                AddActionItem(InventoryItem.GetFromID(state_[i].itemID), i, state_[i].qty);
            }
        }

        [Serializable]
        private struct DockedItemSlot
        {
            public ActionItem item;
            public int qty;
        }

        [Serializable]
        private struct SerializableDockedItemSlot
        {
            public string itemID;
            public int qty;
        }
    }
}