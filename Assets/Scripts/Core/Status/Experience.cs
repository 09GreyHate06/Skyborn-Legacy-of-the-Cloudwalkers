
using SLOTC.Core.Saving;
using System;
using System.Linq;
using UnityEngine;

namespace SLOTC.Core.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [Tooltip("expToLvl = mult * lvl^2")]
        [SerializeField] float _experienceProgressionMult;
        [SerializeField] int _maxLevel;
        [SerializeField] int[] _experienceProgression;

        private Action<int> _onLevelUp;
        private Action<int> _onExperienceGained;

        public int CurrentLevel { get; private set; } = 1;
        public int ExperiencePoints { get; private set; } = 0;

        public event Action<int> OnLevelUp
        {
            add
            {
                if (_onLevelUp == null || !_onLevelUp.GetInvocationList().Contains(value))
                {
                    _onLevelUp += value;
                }
            }
            remove
            {
                _onLevelUp -= value;
            }
        }

        public event Action<int> OnExperienceGained
        {
            add
            {
                if (_onExperienceGained == null || !_onExperienceGained.GetInvocationList().Contains(value))
                {
                    _onExperienceGained += value;
                }
            }
            remove
            {
                _onExperienceGained -= value;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetExperienceProgression();
        }
#endif 

        public void GainExperience(int exp)
        {
            if (CurrentLevel >= _maxLevel)
            {
                ExperiencePoints = 0;
                return;
            }

            ExperiencePoints += exp;
            _onExperienceGained?.Invoke(ExperiencePoints);

            if (ExperiencePoints < _experienceProgression[CurrentLevel + 1]) return;

            CurrentLevel++;

            int experienceLeftOver = ExperiencePoints - _experienceProgression[CurrentLevel];
            ExperiencePoints = 0;

            _onLevelUp?.Invoke(CurrentLevel);

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
        }
    }
}