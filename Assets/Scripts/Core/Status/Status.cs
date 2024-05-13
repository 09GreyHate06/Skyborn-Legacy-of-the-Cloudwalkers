using SLOTC.Core.Saving;
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
        [Header("Progression")]
        [SerializeField] Experience _exp;
        [SerializeField] float _hpPerLevel = 10.0f;
        [SerializeField] float _spPerLevel = 5.0f;
        [SerializeField] float _strPerLevel = 2.0f;
        [SerializeField] float _mstrPerLevel = 1.5f;
        [SerializeField] float _defPerLevel = 1.0f;

        [Space(20)]
        [SerializeField] Stat[] _baseStats = new Stat[8];
        [SerializeField] Stat[] _stats = new Stat[8];
        [SerializeField] UnityEvent _onStatChanged;
        [SerializeField] int _maxStatValue = 999999;
        private Dictionary<string /*ID*/, StatModifier[]> _modifiers = new Dictionary<string /*ID*/, StatModifier[]>();

        public UnityEvent OnStatChanged { get { return _onStatChanged; } }

        private void OnValidate()
        {
            if (!_exp)
                _exp = GetComponent<Experience>();

            int enumLength = Enum.GetValues(typeof(StatType)).Length;
            if (_baseStats.Length != enumLength)
                _baseStats = new Stat[enumLength];

            if(_stats.Length != enumLength)
                _stats = new Stat[enumLength];

            for (int i = 0; i < enumLength; i++)
            {
                _baseStats[i].type = (StatType)i;
                _stats[i].type = (StatType)i;
            }

            UpdateStats();
        }

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
                int b = _baseStats[i].value + (_exp != null ? CalcBaseStatLvlBonus(type) : 0);
                if (GetModifiers(type, out int flat, out int percent))
                    _stats[i].value = Mathf.Clamp(Mathf.FloorToInt(b * (1.0f + percent * 0.01f + ((float)flat / b))), 0, _maxStatValue);
                else
                    _stats[i].value = b;
            }

            _onStatChanged?.Invoke();
        }

        private int CalcBaseStatLvlBonus(StatType type)
        {
            switch (type)
            {
                case StatType.HitPoints:
                    return Mathf.FloorToInt(_exp.CurrentLevel * _hpPerLevel);

                case StatType.SkillPoints:
                    return Mathf.FloorToInt(_exp.CurrentLevel * _spPerLevel);

                case StatType.Strength:
                    return Mathf.FloorToInt(_exp.CurrentLevel * _strPerLevel);

                case StatType.MagicStrength:
                    return Mathf.FloorToInt(_exp.CurrentLevel * _mstrPerLevel);

                case StatType.Defense:
                    return Mathf.FloorToInt(_exp.CurrentLevel * _defPerLevel);
            }

            return 0;
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