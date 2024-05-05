
using SLOTC.Core.Inventory;
using SLOTC.Core.Stats;
using System;
using UnityEngine;

namespace SLOTC.Core.Enemy
{
    public class Reward : MonoBehaviour
    {
        [SerializeField] int _exp;
        [SerializeField] ItemReward[] _itemsRewards;

        public void GrandReward(Experience exp, Inventory.Inventory inventory)
        {
            exp.GainExperience(_exp);
            foreach (var item in _itemsRewards)
            {
                if (!inventory.HasSpaceFor(item.item))
                    continue;

                inventory.AddToFirstEmptySlot(item.item, item.qty);
            }
        }
    }

    [Serializable]
    public struct ItemReward
    {
        public InventoryItem item;
        public int qty;
    }
}