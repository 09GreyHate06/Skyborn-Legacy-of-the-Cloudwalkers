using SLOTC.Utils.UI.Dragging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SLOTC.Core.Inventory.UI
{
    public class InventoryItemUI : DragItem<InventoryItem>
    {
        [SerializeField] TextMeshProUGUI _qtyTxt;

        /// <summary>
        /// set qty to less than 1 to deactivate qtyTxt
        /// </summary>
        /// <param name="item"></param>
        /// <param name="qty"></param>
        public void Set(InventoryItem item, int qty)
        {
            Image image = GetComponent<Image>();
            if (!item)
            {
                image.enabled = false;
            }
            else
            {
                image.enabled = true;
                image.sprite = item.Sprite;
                _qtyTxt.text = qty.ToString();
            }

            _qtyTxt.gameObject.SetActive(qty > 0);
        }
    }
}
