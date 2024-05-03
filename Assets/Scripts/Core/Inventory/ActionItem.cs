
using SLOTC.Core.Stats;
using UnityEngine;

namespace SLOTC.Core.Inventory
{
    public abstract class ActionItem : InventoryItem
    {
        [field: SerializeField] public bool IsConsumable { get; private set; }

        public abstract bool Use(Status user);
    }
}