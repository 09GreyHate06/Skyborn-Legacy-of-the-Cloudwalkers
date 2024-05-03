using SLOTC.Core.Saving;
using SLOTC.Core.Stats;
using UnityEngine;

namespace SLOTC.Core.Combat
{
    public class SkillPoint : MonoBehaviour, ISaveable
    {
        [SerializeField] int _currentSkillPoints;
        [SerializeField] int _maxSkillPoints;

        public int CurrentSkillPoints { get { return _currentSkillPoints; } }
        public int MaxSkillPoints { get { return _maxSkillPoints; } }

        private void OnValidate()
        {
            _currentSkillPoints = Mathf.Clamp(_currentSkillPoints, 0, _maxSkillPoints);
        }

        public void Use(int value)
        {
            _currentSkillPoints = Mathf.Clamp(_currentSkillPoints - value, 0, _maxSkillPoints);
        }

        public void Regen(int value)
        {
            _currentSkillPoints = Mathf.Clamp(_currentSkillPoints + value, 0, _maxSkillPoints);
        }

        public object CaptureState()
        {
            return _currentSkillPoints;
        }

        public void RestoreState(object state)
        {
            _currentSkillPoints = (int)state;
        }

        public void UpdateMaxSkillPoints()
        {
            _maxSkillPoints = GetComponent<Status>().GetStat(StatType.SkillPoints);
            _currentSkillPoints = Mathf.Clamp(_currentSkillPoints, 0, _maxSkillPoints);
        }
    }

}