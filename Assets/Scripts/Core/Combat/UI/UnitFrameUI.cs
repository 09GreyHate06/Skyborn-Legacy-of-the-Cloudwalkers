
using SLOTC.Core.Stats;
using SLOTC.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SLOTC.Core.Combat.UI
{
    public class UnitFrameUI : MonoBehaviour
    {
        [field: SerializeField] public HitPoint HitPoint { get; set; }
        [field: SerializeField] public SkillPoint SkillPoint { get; set; }
        [SerializeField] Image _hpBar;
        [SerializeField] Image _spBar;
        [SerializeField] TextMeshProUGUI _hpPercentageTxt;
        [SerializeField] TextMeshProUGUI _spPercentageTxt;
        [SerializeField] bool _hasLevel = true;
        [SerializeField] Experience _exp;
        [SerializeField] Image _expBar;
        [SerializeField] GameObject _lvlFrame;
        [SerializeField] TextMeshProUGUI _lvlText;

        public bool HasLevel
        {
            get
            {
                return _hasLevel;
            }
            set
            {
                _hasLevel = value;
                if (_lvlFrame != null)
                    _lvlFrame.SetActive(_hasLevel);

                if (_expBar != null)
                    _expBar.gameObject.SetActive(_hasLevel);
            }
        }

        private void OnValidate()
        {
            if (_lvlFrame != null)
                _lvlFrame.SetActive(_hasLevel);

            if (_expBar)
                _expBar.gameObject.SetActive(_hasLevel);
        }

        // Update is called once per frame
        void Update()
        {
            float hpRatio = (float)HitPoint.CurrentHitPoints / HitPoint.MaxHitPoints;
            float spRatio = (float)SkillPoint.CurrentSkillPoints / SkillPoint.MaxSkillPoints;
            _hpBar.fillAmount = hpRatio;
            _spBar.fillAmount = spRatio;

            _hpPercentageTxt.text = (int)Mathf.Clamp(hpRatio * 100.0f, 0, 100) + "%";
            _spPercentageTxt.text = (int)Mathf.Clamp(spRatio * 100.0f, 0, 100) + "%";

            if (_hasLevel && _lvlFrame != null)
                _lvlText.text = _exp.CurrentLevel.ToString();

            if (_hasLevel && _expBar != null)
                _expBar.fillAmount = (float)_exp.ExperiencePoints / _exp.GetExperienceProgression(_exp.CurrentLevel + 1);
        }
    }
}