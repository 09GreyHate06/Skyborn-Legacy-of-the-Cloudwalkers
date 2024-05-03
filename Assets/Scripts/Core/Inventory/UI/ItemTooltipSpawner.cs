using SLOTC.Utils.UI.Tooltip;
using UnityEngine;

namespace SLOTC.Core.Inventory.UI
{
    [RequireComponent(typeof(IItemHolder))]
    public class ItemTooltipSpawner : TooltipSpawner
    {
        public override bool CanCreateTooltip()
        {
            InventoryItem item = GetComponent<IItemHolder>().GetItem();

            if (!item) return false;

            return true;
        }

        public override void UpdateTooltip(GameObject tooltip)
        {
            ItemTooltip itemTooltip = tooltip.GetComponent<ItemTooltip>();
            if (!itemTooltip) return;

            InventoryItem item = GetComponent<IItemHolder>().GetItem();
            itemTooltip.Setup(item);
        }
    }
}