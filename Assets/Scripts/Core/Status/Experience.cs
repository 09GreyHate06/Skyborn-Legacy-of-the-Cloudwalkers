
using SLOTC.Core.Saving;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SLOTC.Core.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [Tooltip("expToLvl = mult * lvl^2")]
        [SerializeField] float _experienceProgressionMult;
        [SerializeField] int _maxLevel;
        [SerializeField] int[] _experienceProgression;
        [field: SerializeField] public int CurrentLevel { get; private set; } = 1;
        public UnityEvent OnLvlUpdated;

        public event Action<int> OnLevelUp;
        public event Action<int> OnExperienceGained;

        public int ExperiencePoints { get; private set; } = 0;

        private void OnValidate()
        {
            SetExperienceProgression();

            if (CurrentLevel < 1)
                CurrentLevel = 1;

            if (CurrentLevel >= _maxLevel)
                CurrentLevel = _maxLevel;

            OnLvlUpdated?.Invoke();
        }

        public void GainExperience(int exp)
        {
            if (CurrentLevel >= _maxLevel)
            {
                ExperiencePoints = 0;
                return;
            }

            OnExperienceGained?.Invoke(exp);
            ExperiencePoints += exp;

            if (ExperiencePoints < _experienceProgression[CurrentLevel + 1]) return;

            CurrentLevel++;

            int experienceLeftOver = ExperiencePoints - _experienceProgression[CurrentLevel];
            ExperiencePoints = 0;

            OnLevelUp?.Invoke(CurrentLevel);
            OnLvlUpdated?.Invoke();

            // will call until experience points < experience to level up
            GainExperience(experienceLeftOver);
        }

        private void SetExperienceProgression()
        {
            _experienceProgression = new int[_maxLevel + 1];
            for (int i = 1; i < _experienceProgression.Length; i++)
            {
                _experienceProgression[i] = Mathf.FloorToInt(_experienceProgressionMult * Mathf.Pow(i, 2.0f));
            }
        }

        public int GetExperienceProgression(int level)
        {
            if (level >= _experienceProgression.Length || level < 0)
                return 0;

            return _experienceProgression[level];
        }

        public object CaptureState()
        {
            int[] state = new int[2];
            state[0] = ExperiencePoints;
            state[1] = CurrentLevel;

            return state;
        }

        public void RestoreState(object state)
        {
            int[] state_ = (int[])state;
            ExperiencePoints = state_[0];
            CurrentLevel = state_[1];
            OnLvlUpdated?.Invoke();
        }
    }
}