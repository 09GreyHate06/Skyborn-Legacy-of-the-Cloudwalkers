using SLOTC.Core.Combat;
using SLOTC.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SLOTC.Core.Stats.UI
{
    public class StatusUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _lvlValueTxt;
        [SerializeField] TextMeshProUGUI _expValueTxt;
        [SerializeField] TextMeshProUGUI _nextValueTxt;

        [SerializeField] TextMeshProUGUI _hpValueTxt;
        [SerializeField] TextMeshProUGUI _spValueTxt;
        [SerializeField] TextMeshProUGUI _strValueTxt;
        [SerializeField] TextMeshProUGUI _defValueTxt;

        [SerializeField] TextMeshProUGUI _airResistValueTxt;
        [SerializeField] TextMeshProUGUI _waterResistValueTxt;
        [SerializeField] TextMeshProUGUI _earthResistValueTxt;
        [SerializeField] TextMeshProUGUI _fireResistValueTxt;

        private Experience _exp;
        private Status _stat;
        private HitPoint _hp;
        private SkillPoint _sp;

        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag(Tags.Player);
            _exp = player.GetComponent<Experience>();
            _stat = player.GetComponent<Status>();
            _hp = player.GetComponent<HitPoint>();
            _sp = player.GetComponent<SkillPoint>();
        }

        private void Update()
        {
            _hpValueTxt.text = _hp.CurrentHitPoints + "/" + _hp.MaxHitPoints;
            _spValueTxt.text = _sp.CurrentSkillPoints + "/" + _sp.MaxSkillPoints;
        }

        private void OnEnable()
        {
            _stat.OnStatChanged.AddListener(UpdateStatValues);
            _exp.OnExperienceGained += UpdateExpValues;

            UpdateExpValues(_exp.ExperiencePoints);
            UpdateStatValues();
        }

        private void OnDisable()
        {
            _stat.OnStatChanged.RemoveListener(UpdateStatValues);
            _exp.OnExperienceGained -= UpdateExpValues;
        }

        public void UpdateExpValues(int _)
        {
            _lvlValueTxt.text = _exp.CurrentLevel.ToString();
            _expValueTxt.text = _exp.ExperiencePoints.ToString();
            _nextValueTxt.text = _exp.GetExperienceProgression(_exp.CurrentLevel + 1).ToString();
        }

        public void UpdateStatValues()
        {
            //if (!isActiveAndEnabled) return; // because unity event calls this even when disabled

            _strValueTxt.text = _stat.GetStat(StatType.Strength).ToString();
            _defValueTxt.text = _stat.GetStat(StatType.Defense).ToString();

            _airResistValueTxt.text = _stat.GetStat(StatType.AirResistance) + "%";
            _waterResistValueTxt.text = _stat.GetStat(StatType.WaterResistance) + "%";
            _earthResistValueTxt.text = _stat.GetStat(StatType.EarthResistance) + "%";
            _fireResistValueTxt.text = _stat.GetStat(StatType.FireResistance) + "%";
        }
    }
}