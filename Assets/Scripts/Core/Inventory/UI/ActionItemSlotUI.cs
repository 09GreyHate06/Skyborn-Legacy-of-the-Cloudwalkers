
using UnityEngine;
using SLOTC.Utils.UI.Dragging;
using SLOTC.Utils;

namespace SLOTC.Core.Inventory.UI
{
    public class ActionItemSlotUI : MonoBehaviour, IDragSlot<InventoryItem>, IItemHolder
    {
        [SerializeField] InventoryItemUI _inventoryItemUI;
        [SerializeField] int _index;

        private ActionItemBar _actionItemBar;

        private void Awake()
        {
            _actionItemBar = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<ActionItemBar>();
        }

        private void OnEnable()
        {
            _actionItemBar.OnActionItemBarChanged += Redraw;
            Redraw();
        }

        private void OnDisable()
        {
            _actionItemBar.OnActionItemBarChanged -= Redraw;
        }

        public void AddItems(InventoryItem item, int qty)
        {
            _actionItemBar.AddActionItem(item, _index, qty);
        }

        public InventoryItem GetItem()
        {
            return _actionItemBar.GetActionItem(_index);
        }

        public int GetQty()
        {
            return _actionItemBar.GetQty(_index);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return _actionItemBar.MaxAcceptable(item, _index);
        }

        public void RemoveItems(int qty)
        {
            _actionItemBar.RemoveItems(_index, qty);
        }

        public void Redraw()
        {
            _inventoryItemUI.Set(GetItem(), GetQty());
        }
    }
}