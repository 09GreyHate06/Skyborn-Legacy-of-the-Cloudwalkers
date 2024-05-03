using UnityEngine;
using SLOTC.Core.Stats;

namespace SLOTC.Core.Combat
{
    [CreateAssetMenu(fileName = "New Attack", menuName = "Combat/Attack")]
    public class Attack : ScriptableObject
    {
        [Header("Core Settings")]
        [SerializeField] DamageModifier[] _damageModifiers;
        [field: SerializeField] public Vector3 Force { get; private set; }
        [field: SerializeField] public Vector3 TargetForce { get; private set; }

        public int CalcDamage(Status user, Status target)
        {
            if (_damageModifiers == null) 
                return 0;

            return CombatCommonFunc.CalcDamage(user, target, _damageModifiers);
        } 
    }
}