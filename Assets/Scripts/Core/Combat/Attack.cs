using UnityEngine;
using SLOTC.Core.Stats;
using SLOTC.Core.GlobalSetting;

namespace SLOTC.Core.Combat
{
    [CreateAssetMenu(fileName = "New Attack", menuName = "Combat/Attack")]
    public class Attack : ScriptableObject
    {
        [field: SerializeField] string _animName;
        [field: SerializeField, Tooltip("Combo attacks must have the same tags.")] string _animTag;
        [field: SerializeField] public float TransitionDuration { get; private set; }
        [SerializeField] DamageModifier[] _damageModifiers;

        [field: SerializeField] public float ComboAttackExitNormalizedTime { get; private set; }
        [field: SerializeField] public Vector3 Force { get; private set; }
        [field: SerializeField] public float ForceNormalizedTime { get; private set; }
        [field: SerializeField] public float KnockbackForce { get; private set; }

        [HideInInspector, SerializeField] public int _animNameHash;
        [HideInInspector, SerializeField] public int _animTagHash;

        public int AnimNameHash { get { return _animNameHash; } }
        public int AnimTagHash { get { return _animTagHash; } }

        private void OnValidate()
        {
            _animNameHash = Animator.StringToHash(_animName);
            _animTagHash = Animator.StringToHash(_animTag);
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