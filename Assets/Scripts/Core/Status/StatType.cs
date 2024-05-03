
namespace SLOTC.Core.Stats
{
    public enum StatType : int
    {
        ///<summary>abbreviated as HP</summary>
        HitPoints,

        ///<summary>abbreviated as SP</summary>
        SkillPoints,

        ///<summary>abbreviated as PDMG</summary>
        PhysicalDamage,

        ///<summary>abbreviated as MDMG</summary>
        MagicDamage,

        ///<summary>abbreviated as DEF</summary>
        Defense,

        ///<summary>abbreviated as Crit%</summary>
        CriticalHitChance,

        ///<summary>abbreviated as Crit ATK%</summary>
        CriticalHitBonusDamage,

        AirResistance,
        EarthResistance,
        WaterResistance,
        FireResistance
    }
}