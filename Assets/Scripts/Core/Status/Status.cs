using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SLOTC.Core.Stats
{
    [Serializable]
    public struct Stat
    {
        public StatType type;
        public int value;
    }

    public class Status : MonoBehaviour
    {
        [SerializeField] Attributes _attributes;
        [SerializeField] Stat[] _stats = new Stat[8];
        [SerializeField] UnityEvent _onStatChanged;
        [SerializeField] int _maxStatValue = 999999;
        private Dictionary<string /*ID*/, StatModifier[]> _modifiers = new Dictionary<string /*ID*/, StatModifier[]>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats.Length != 8)
                _stats = new Stat[8];

            for (int i = 0; i < _stats.Length; i++)
            {
                _stats[i].type = (StatType)i;
            }

            if (_attributes == null) return;

            UpdateStats();
        }
#endif

        public int GetStat(StatType type)
        {
            return _stats.First(x => x.type == type).value;
        }

        public bool ModifierExists(IStatModifier source)
        {
            return _modifiers.ContainsKey(source.GetID());
        }

        public bool AddModifiers(IStatModifier source)
        {
            if (_modifiers.ContainsKey(source.GetID()))
                return false;

            _modifiers[source.GetID()] = source.GetStatModifiers();
            UpdateStats();
            return true;
        }

        public bool RemoveModifiers(IStatModifier source)
        {
            if (!_modifiers.ContainsKey(source.GetID()))
                return false;

            _modifiers.Remove(source.GetID());
            UpdateStats();
            return true;
        }

        public void UpdateStats()
        {
            for (int i = 0; i < _stats.Length; i++)
            {
                StatType type = (StatType)i;
                _stats[i].type = type;
                int b = CalcBaseStat(type);
                if (GetModifiers(type, out int flat, out int percent))
                    _stats[i].value = Mathf.Clamp(Mathf.FloorToInt(b * (1.0f + percent * 0.01f + ((float)flat / b))), 0, _maxStatValue);
                else
                    _stats[i].value = b;
            }

            _onStatChanged?.Invoke();
        }

        private int CalcBaseStat(StatType type)
        {
            switch (type)
            {
                case StatType.HitPoints:
                    // HP =  (Vitality * 12) + (Strength * 2)
                    return Mathf.Clamp(Mathf.FloorToInt(_attributes.GetAttribute(AttributeType.Vitality) * 12 + _attributes.GetAttribute(AttributeType.Strength) * 2), 0, _maxStatValue);

                case StatType.SkillPoints:
                    // SP = (Intelligence * 10) + (Vitality * 3)
                    return Mathf.Clamp(Mathf.FloorToInt(_attributes.GetAttribute(AttributeType.Intelligence) * 10 + _attributes.GetAttribute(AttributeType.Vitality) * 3), 0, _maxStatValue);

                case StatType.PhysicalDamage:
                    // PDMG =  (Strength * 1.2) + (Dexterity * 0.5)
                    return Mathf.Clamp(Mathf.FloorToInt(_attributes.GetAttribute(AttributeType.Strength) * 1.2f + _attributes.GetAttribute(AttributeType.Dexterity) * 0.5f), 0, _maxStatValue);

                case StatType.MagicDamage:
                    // MDMG = (Intelligence * 1.5)
                    return Mathf.Clamp(Mathf.FloorToInt(_attributes.GetAttribute(AttributeType.Intelligence) * 1.5f), 0, _maxStatValue);

                case StatType.Defense:
                    // DEF =  (Vitality * 0.6) + (Dexterity * 0.4)
                    return Mathf.Clamp(Mathf.FloorToInt(_attributes.GetAttribute(AttributeType.Vitality) * 0.6f + _attributes.GetAttribute(AttributeType.Dexterity) * 0.4f), 0, _maxStatValue);

                case StatType.MagicDefense:
                    // MDEF =  (Intelligence * 0.8) + (Vitality * 0.2)
                    return Mathf.Clamp(Mathf.FloorToInt(_attributes.GetAttribute(AttributeType.Intelligence) * 0.8f + _attributes.GetAttribute(AttributeType.Vitality) * 0.2f), 0, _maxStatValue);

                case StatType.CriticalHitChance:
                    // CRIT% =  (Dexterity * 0.1) + (Luck * 0.4)
                    return Mathf.Clamp(Mathf.FloorToInt(_attributes.GetAttribute(AttributeType.Dexterity) * 0.1f + _attributes.GetAttribute(AttributeType.Luck) * 0.4f), 0, _maxStatValue);

                case StatType.CriticalHitBonusDamage:
                    // CRIT ATK% = (Strength * 1.5) +  (Luck * 0.2)
                    return Mathf.Clamp(Mathf.FloorToInt(_attributes.GetAttribute(AttributeType.Strength) * 1.5f + _attributes.GetAttribute(AttributeType.Luck) * 0.2f), 0, _maxStatValue);

            }

            return -1;
        }

        private bool GetModifiers(StatType type, out int flat, out int percent)
        {
            flat = 0;
            percent = 0;

            foreach (KeyValuePair<string, StatModifier[]> pair in _modifiers)
            {
                foreach (StatModifier modifier in pair.Value)
                {
                    if (modifier.statType != type) continue;

                    if (modifier.modifierType == ModifierType.Flat)
                        flat += modifier.value;
                    else if (modifier.modifierType == ModifierType.Percent)
                        percent += modifier.value;
                }
            }

            return flat != 0 || percent != 0;
        }
    }
}