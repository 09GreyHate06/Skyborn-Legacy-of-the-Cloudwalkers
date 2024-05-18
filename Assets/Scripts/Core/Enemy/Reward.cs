
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
                float r = UnityEngine.Random.Range(0.0f, 100.0f);

                if (!inventory.HasSpaceFor(item.item) || r > item.chance)
                    continue;

                inventory.AddToFirstEmptySlot(item.item, item.qty);
            }
        }
    }

    [Serializable]
    public struct ItemReward
    {
        [Range(0.0f, 100.0f)] public float chance;
        public InventoryItem item;
        public int qty;
    }
}