
using UnityEngine;
using SLOTC.Utils.UI.Dragging;

namespace SLOTC.Core.Inventory.UI
{
    public class InventoryItemSlotUI : MonoBehaviour, IDragSlot<InventoryItem>, IItemHolder
    {
        [SerializeField] InventoryItemUI _inventoryItemUI;

        private int _index;
        private Inventory _inventory;

        public void Set(Inventory inventory, int index)
        {
            _inventory = inventory;
            _index = index;
            _inventoryItemUI.Set(_inventory.GetItemInSlot(index), _inventory.GetQtyInSlot(index));
        }

        public void AddItems(InventoryItem item, int qty)
        {
            _inventory.AddItemToSlot(_index, item, qty);
        }

        public InventoryItem GetItem()
        {
            return _inventory.GetItemInSlot(_index);
        }

        public int GetQty()
        {
            return _inventory.GetQtyInSlot(_index);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (_inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void RemoveItems(int qty)
        {
            _inventory.RemoveFromSlot(_index, qty);
        }
    }
}