using UnityEngine;
using SLOTC.Core.Stats;
using System.Linq;

namespace SLOTC.Core.Combat
{
    public static class CombatCommonFunc
    {
        public static readonly int MaxDamage = 9999;
        public static int CalcDamage(Status user, Status target, DamageModifier[] damageModifiers) 
        {
            int physicalFlat = 0;
            int physicalPercent = 0;
            int airFlat = 0;
            int airPercent = 0;
            int waterFlat = 0;
            int waterPercent = 0;
            int earthFlat/*lol*/ = 0;
            int earthPercent = 0;
            int fireFlat = 0;
            int firePercent = 0;

            bool hasPhysicalDamage = damageModifiers.Any(x => x.type == DamageType.Physical);

            foreach (DamageModifier damageMod in damageModifiers)
            {
                if (damageMod.type == DamageType.Physical)
                {
                    if (damageMod.modType == ModifierType.Flat)
                        physicalFlat += damageMod.value;
                    else if (damageMod.modType == ModifierType.Percent)
                        physicalPercent += damageMod.value;
                }
                else if (damageMod.type == DamageType.Air)
                {
                    if (damageMod.modType == ModifierType.Flat)
                        airFlat += damageMod.value;
                    else if (damageMod.modType == ModifierType.Percent)
                        airPercent += damageMod.value;
                }
                else if (damageMod.type == DamageType.Water)
                {
                    if (damageMod.modType == ModifierType.Flat)
                        waterFlat += damageMod.value;
                    else if (damageMod.modType == ModifierType.Percent)
                        waterPercent += damageMod.value;
                }
                else if (damageMod.type == DamageType.Earth)
                {
                    if (damageMod.modType == ModifierType.Flat)
                        earthFlat += damageMod.value;
                    else if (damageMod.modType == ModifierType.Percent)
                        earthPercent += damageMod.value;
                }
                else if (damageMod.type == DamageType.Fire)
                {
                    if (damageMod.modType == ModifierType.Flat)
                        fireFlat += damageMod.value;
                    else if (damageMod.modType == ModifierType.Percent)
                        firePercent += damageMod.value;
                }
            }

            float damage = 0.0f;
            if(hasPhysicalDamage)
            {
                damage += user.GetStat(StatType.Strength) * (1.0f + physicalPercent * 0.01f) + physicalFlat - target.GetStat(StatType.Defense);
            }

            float userMagicDamge = user.GetStat(StatType.MagicStrength);

            if(airFlat != 0 || airPercent != 0)
            {
                damage += userMagicDamge * (1.0f + airPercent * 0.01f) + airFlat - target.GetStat(StatType.AirResistance);
            }

            if(waterFlat != 0 || waterPercent != 0)
            {
                damage += userMagicDamge * (1.0f + waterPercent * 0.01f) + waterFlat - target.GetStat(StatType.WaterResistance);
            }

            if (earthFlat != 0 || earthPercent != 0)
            {
                damage += userMagicDamge * (1.0f + earthPercent * 0.01f) + earthFlat - target.GetStat(StatType.EarthResistance);
            }

            if (fireFlat != 0 || firePercent != 0)
            {
                damage += userMagicDamge * (1.0f + firePercent * 0.01f) + fireFlat - target.GetStat(StatType.FireResistance);
            }

            Debug.Log(damage);
            return Mathf.FloorToInt(Mathf.Clamp(damage, 0.0f, MaxDamage));
        }
    }
}