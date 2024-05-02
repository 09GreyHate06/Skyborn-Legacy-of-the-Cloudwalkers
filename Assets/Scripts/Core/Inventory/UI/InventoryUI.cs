using UnityEngine;

namespace SLOTC.Core.Inventory.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] InventoryItemSlotUI _inventoryItemSlotUIPrefab;
        [SerializeField] Transform _inventoryItemsHolder;

        private Inventory _inventory;

        private void Awake()
        {
            _inventory = Inventory.GetPlayerInventory();
        }

        private void OnEnable()
        {
            _inventory.OnInventoryChanged += Redraw;
            Redraw();
        }

        private void OnDisable()
        {
            _inventory.OnInventoryChanged -= Redraw;
        }

        private void Redraw()
        {
            foreach (Transform child in _inventoryItemsHolder)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < _inventory.GetSize(); i++)
            {
                InventoryItemSlotUI slot = Instantiate(_inventoryItemSlotUIPrefab, _inventoryItemsHolder);
                slot.Set(_inventory, i);
            }
        }
    }
}