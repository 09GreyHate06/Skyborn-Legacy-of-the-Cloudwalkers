using SLOTC.Core.Saving;
using SLOTC.Core.Stats;
using UnityEngine;

namespace SLOTC.Core.Combat
{
    public class MagicPoint : MonoBehaviour, ISaveable
    {
        [SerializeField] int _currentMagicPoints;
        [SerializeField] int _maxMagicPoints;

        public int CurrentMagicPoints { get { return _currentMagicPoints; } }
        public int MaxMagicPoints { get { return _maxMagicPoints; } }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _currentMagicPoints = Mathf.Clamp(_currentMagicPoints, 0, _maxMagicPoints);
        }
#endif

        public void Use(int value)
        {
            _currentMagicPoints = Mathf.Clamp(_currentMagicPoints - value, 0, _maxMagicPoints);
        }

        public void Regen(int value)
        {
            _currentMagicPoints = Mathf.Clamp(_currentMagicPoints + value, 0, _maxMagicPoints);
        }

        public object CaptureState()
        {
            return _currentMagicPoints;
        }

        public void RestoreState(object state)
        {
            _currentMagicPoints = (int)state;
        }

        public void UpdateMaxMagicPoints()
        {
            _maxMagicPoints = GetComponent<Status>().GetStat(StatType.MagicPoints);
            _currentMagicPoints = Mathf.Clamp(_currentMagicPoints, 0, _maxMagicPoints);
        }
    }

}