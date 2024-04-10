using SLOTC.Core.Combat;
using SLOTC.Core.Inventory;
using SLOTC.Core.Stats;
using TMPro;
using UnityEngine;

public class StatDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _strTxt;
    [SerializeField] TextMeshProUGUI _intTxt;
    [SerializeField] TextMeshProUGUI _dexTxt;
    [SerializeField] TextMeshProUGUI _vitTxt;
    [SerializeField] TextMeshProUGUI _lukTxt;
    [SerializeField] TextMeshProUGUI _attPtsTxt;
    [SerializeField] TextMeshProUGUI _lvlTxt;
    [SerializeField] TextMeshProUGUI _expTxt;
    [SerializeField] TextMeshProUGUI _hpTxt;
    [SerializeField] TextMeshProUGUI _mpTxt;
    [SerializeField] TextMeshProUGUI _atkTxt;
    [SerializeField] TextMeshProUGUI _matkTxt;
    [SerializeField] TextMeshProUGUI _defTxt;
    [SerializeField] TextMeshProUGUI _mdefTxt;
    [SerializeField] TextMeshProUGUI _crtChanceTxt;
    [SerializeField] TextMeshProUGUI _crtTxt;

    private Experience _exp;
    private Status _stat;
    private Attributes _attributes;
    private HitPoint _hp;
    private MagicPoint _mp;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _exp = player.GetComponent<Experience>();
        _stat = player.GetComponent<Status>();
        _attributes = player.GetComponent<Attributes>();
        _hp = player.GetComponent<HitPoint>();
        _mp = player.GetComponent<MagicPoint>();
    }

    private void Update()
    {
        _hpTxt.text = _hp.CurrentHitPoints + "/" + _hp.MaxHitPoints;
        _mpTxt.text = _mp.CurrentMagicPoints + "/" + _mp.MaxMagicPoints;

        _attPtsTxt.text = _attributes.AttributePoints.ToString();
    }

    private void OnEnable()
    {
        _exp.OnExperienceGained += UpdateExp;

        UpdateExp(_exp.ExperiencePoints);
        UpdateAttributes();
        UpdateStats();
    }

    private void OnDisable()
    {
        _exp.OnExperienceGained -= UpdateExp;
    }

    public void UpdateExp(int _)
    {
        _lvlTxt.text = _exp.CurrentLevel.ToString();
        _expTxt.text = _exp.ExperiencePoints.ToString();
    }

    public void UpdateStats()
    {
        if (!isActiveAndEnabled) return; // because unity event calls this even when disabled

        _atkTxt.text = _stat.GetStat(StatType.AttackDamage).ToString();
        _matkTxt.text = _stat.GetStat(StatType.MagicAttackDamage).ToString();
        _defTxt.text = _stat.GetStat(StatType.Defense).ToString();
        _mdefTxt.text = _stat.GetStat(StatType.MagicDefense).ToString();
        _crtChanceTxt.text = _stat.GetStat(StatType.CriticalHitChance) + "%";
        _crtTxt.text = _stat.GetStat(StatType.CriticalHitBonusDamage) + "%";
    }

    public void UpdateAttributes()
    {
        if (!isActiveAndEnabled) return; // because unity event calls this even when disabled

        _strTxt.text = _attributes.GetAttribute(AttributeType.Strength).ToString();
        _intTxt.text = _attributes.GetAttribute(AttributeType.Intelligence).ToString();
        _dexTxt.text = _attributes.GetAttribute(AttributeType.Dexterity).ToString();
        _vitTxt.text = _attributes.GetAttribute(AttributeType.Vitality).ToString();
        _lukTxt.text = _attributes.GetAttribute(AttributeType.Luck).ToString();
    }

    public void AddPointToSTR()
    {
        _attributes.AddPoints(AttributeType.Strength, 1);
    }

    public void AddPointToINT()
    {
        _attributes.AddPoints(AttributeType.Intelligence, 1);
    }

    public void AddPointToDEX()
    {
        _attributes.AddPoints(AttributeType.Dexterity, 1);
    }

    public void AddPointToVIT()
    {
        _attributes.AddPoints(AttributeType.Vitality, 1);
    }

    public void AddPointToLUK()
    {
        _attributes.AddPoints(AttributeType.Luck, 1);
    }
}
