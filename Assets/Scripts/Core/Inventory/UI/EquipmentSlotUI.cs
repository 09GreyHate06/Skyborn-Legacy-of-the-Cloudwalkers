using SLOTC.Utils.UI.Dragging;
using SLOTC.Utils;
using UnityEngine;

namespace SLOTC.Core.Inventory.UI
{
    public class EquipmentSlotUI : MonoBehaviour, IDragSlot<InventoryItem>, IItemHolder
    {
        [SerializeField] InventoryItemUI _inventoryItemUI;
        [SerializeField] EquipLocation _equipLocation;

        private Equipment _equipment;

        private void Awake()
        {
            _equipment = Equipment.GetPlayerEquipment();
        }

        private void OnEnable()
        {
            _equipment.OnEquipChanged += Redraw;
            Redraw();
        }

        private void OnDisable()
        {
            _equipment.OnEquipChanged -= Redraw;
        }

        public void AddItems(InventoryItem item, int qty)
        {
            _equipment.Equip((EquipableItem)item);
        }

        public InventoryItem GetItem()
        {
            return _equipment.GetEquipment(_equipLocation);
        }

        public int GetQty()
        {
            return GetItem() == null ? 0 : 1;
        }

        public int MaxAcceptable(InventoryItem item)
        {
            EquipableItem equipableItem = item as EquipableItem;
            if (equipableItem == null) return 0;
            if (equipableItem.EquipLocation != _equipLocation) return 0;
            if (GetItem() != null) return 0;

            return 1;
        }

        public void RemoveItems(int qty)
        {
            _equipment.Unequip(_equipLocation);
        }

        public void Redraw()
        {
            _inventoryItemUI.Set(_equipment.GetEquipment(_equipLocation), 0);
        }
    }
}