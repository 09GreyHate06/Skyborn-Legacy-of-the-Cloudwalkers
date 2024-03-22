using System;

namespace SLOTC.Core.Stats
{
    [Serializable]
    public struct StatModifier
    {
        public StatType statType;
        public ModifierType modifierType;
        public int value;
    }
}
