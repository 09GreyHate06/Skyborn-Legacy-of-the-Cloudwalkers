using SLOTC.Core.Stats;
using System;
using UnityEngine;

namespace SLOTC.Core.Combat
{
    [Serializable]
    public struct DamageModifier
    {
        public DamageType type;
        [Tooltip("Percent is useless on elemental damage")]
        public ModifierType modType;
        public int value;
    }
}