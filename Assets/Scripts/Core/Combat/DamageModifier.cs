using SLOTC.Core.Stats;
using System;

namespace SLOTC.Core.Combat
{
    [Serializable]
    public struct DamageModifier
    {
        public DamageType type;
        public ModifierType modType;
        public int value;
    }
}