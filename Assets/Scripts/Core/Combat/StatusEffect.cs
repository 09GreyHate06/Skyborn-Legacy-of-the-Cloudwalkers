
using SLOTC.Core.Stats;
using System;
using System.ComponentModel;
using UnityEngine;

namespace SLOTC.Core.Combat
{
    [Serializable]
    public class StatusEffect : IStatModifier
    {
        [SerializeField, HideInInspector] string _id = Guid.NewGuid().ToString();
        [SerializeField] StatModifier[] _statModifiers;
        [field: SerializeField] public DamageModifier DamagePerSeconds { get; private set; }
        [field: SerializeField] public int HPRegenPerSeconds { get; private set; }
        [field: SerializeField] public int SPRegenPerSeconds { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }

        //public float movespeedFlatModifier;
        //public int movespeedPercentModifier;

        public string GetID()
        {
            return _id;
        }

        public StatModifier[] GetStatModifiers()
        {
            return _statModifiers;
        }
    }
}