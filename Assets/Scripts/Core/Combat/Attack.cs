using UnityEngine;
using SLOTC.Core.Stats;
using SLOTC.Core.GlobalSetting;
using System.ComponentModel;

namespace SLOTC.Core.Combat
{
    [CreateAssetMenu(fileName = "New Attack", menuName = "Combat/Attack")]
    public class Attack : ScriptableObject
    {
        [Header("Core Settings")]
        [SerializeField] DamageModifier[] _damageModifiers;
        [field: SerializeField] public Vector3 Force { get; private set; }
        [field: SerializeField] public float KnockbackForce { get; private set; }

        [Space(10)]
        [Header("Animation Settings")]
        [field: SerializeField] string _animName;
        [field: SerializeField] public float AnimNormalizedTimeToApplyForce { get; private set; }
        [field: SerializeField] public float AnimNormalizedExitTime { get; private set; }
        [field: SerializeField] public float AnimNormalizedTransitionDuration { get; private set; }
        [field: SerializeField] public float AnimNormalizedTransitionOffset { get; private set; }

        [ReadOnly(true), SerializeField] int _animNameHash;
        

        public int AnimNameHash { get { return _animNameHash; } }

        private void OnValidate()
        {
            _animNameHash = Animator.StringToHash(_animName);
        }

        public int CalcDamage(Status user, Status target)
        {
            if (_damageModifiers == null) 
                return 0;

            int physicalFlat = 0;
            int physicalPercent = 0;
            foreach (DamageModifier damage in _damageModifiers)
            {
                if(damage.type == DamageType.Physical)
                {
                    if (damage.modType == ModifierType.Flat)
                        physicalFlat += damage.value;
                    else if (damage.modType == ModifierType.Percent)
                        physicalPercent += damage.value;
                }
            }

            int physicalTotal = Mathf.FloorToInt(
                Mathf.Clamp(user.GetStat(StatType.PhysicalDamage) * (1.0f + physicalPercent * 0.01f) + physicalFlat - target.GetStat(StatType.Defense),
                0.0f, GlobalSettings.MaxDamage));

            // todo magic damage

            return physicalTotal;
        } 
    }
}