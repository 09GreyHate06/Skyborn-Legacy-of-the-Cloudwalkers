using SLOTC.Core.Saving;
using SLOTC.Core.Stats;
using System;
using System.Linq;
using UnityEngine;

namespace SLOTC.Core.Combat
{
    public class HitPoint : MonoBehaviour, ISaveable
    {
        [SerializeField] int _currentHitPoints;
        [SerializeField] int _maxHitPoints;

        private Action<int> _onTakeDamage;

        public event Action<int> OnTakeDamage
        {
            add
            {
                if (_onTakeDamage == null || !_onTakeDamage.GetInvocationList().Contains(value))
                {
                    _onTakeDamage += value;
                }
            }
            remove
            {
                _onTakeDamage -= value;
            }
        }
        public int CurrentHitPoints { get { return _currentHitPoints; } }
        public int MaxHitPoints { get { return _maxHitPoints; } }
        public bool IsDead { get; private set; } = false;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _currentHitPoints = Mathf.Clamp(_currentHitPoints, 0, _maxHitPoints);
        }
#endif

        public void TakeDamage(int damage)
        {
            _currentHitPoints = Mathf.Clamp(_currentHitPoints - damage, 0, _maxHitPoints);

            if (_currentHitPoints <= 0)
                IsDead = true;

            _onTakeDamage?.Invoke(damage);
        }

        public void Regen(int value)
        {
            if (IsDead) return; // can't heal dead people

            _currentHitPoints = Mathf.Clamp(_currentHitPoints + value, 0, _maxHitPoints);
        }

        public void Revive(int reviveHeal)
        {
            if (reviveHeal <= 0) return;

            _currentHitPoints = Mathf.Clamp(_currentHitPoints + reviveHeal, 0, _maxHitPoints);
            IsDead = false;
        }

        public void UpdateMaxHitPoints()
        {
            _maxHitPoints = GetComponent<Status>().GetStat(StatType.HitPoints);
            _currentHitPoints = Mathf.Clamp(_currentHitPoints, 0, _maxHitPoints);
        }

        public object CaptureState()
        {
            return _currentHitPoints;
        }

        public void RestoreState(object state)
        {
            _currentHitPoints = (int)state;
            IsDead = _currentHitPoints <= 0;
        }
    }
}
