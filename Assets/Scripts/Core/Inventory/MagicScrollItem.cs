using SLOTC.Core.Combat;
using SLOTC.Core.Stats;
using UnityEngine;

namespace SLOTC.Core.Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Magic scroll")]
    public class MagicScrollItem : ActionItem
    {
        [field: SerializeField] public Magic Magic { get; private set; }

        public override bool Use(Status user)
        {
            Magic.Use(user);
            return true;
        }
    }
}