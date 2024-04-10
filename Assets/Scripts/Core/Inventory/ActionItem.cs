
using UnityEngine;

namespace SLOTC.Core.Inventory
{
    public abstract class ActionItem : InventoryItem
    {
        [SerializeField] bool _isConsumable;

        public bool IsConsumable { get { return _isConsumable; } }

        
    }
}