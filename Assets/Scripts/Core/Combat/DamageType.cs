using SLOTC.Core.Stats;

namespace SLOTC.Core.Combat
{
    public enum DamageType
    {
        // physical
        Physical,

        // magics
        // set to stat type value for easy access
        Air = StatType.AirResistance, 
        Water = StatType.WaterResistance,
        Earth = StatType.EarthResistance,
        Fire = StatType.FireResistance,
    }
}