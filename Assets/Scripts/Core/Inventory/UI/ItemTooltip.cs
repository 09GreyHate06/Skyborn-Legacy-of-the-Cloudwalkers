
using UnityEngine;
using TMPro;

namespace SLOTC.Core.Inventory.UI
{
    public class ItemTooltip : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _titleTxt;
        [SerializeField] TextMeshProUGUI _bodyTxt;

        public void Setup(InventoryItem item)
        {
            _titleTxt.text = item.DisplayName;
            _bodyTxt.text = item.Description;
        }
    }
}