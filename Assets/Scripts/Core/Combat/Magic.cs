using SLOTC.Core.Stats;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace SLOTC.Core.Combat
{
    public abstract class Magic : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField] int _mpCost;
        [SerializeField] GameObject _castFX;

        [Header("Buffs/Debuffs")]
        [SerializeField] StatusEffect _statusEffect;

        public int MPCost { get { return _mpCost; } }
        protected GameObject CastFX { get { return _castFX; } }

        public abstract void Use(Status user);

        protected void ApplyStatusEffect(Status user, StatusAffectable target)
        {
            target.AddEffect(user, _statusEffect);
        }
    }

    public abstract class AttackMagic : Magic
    {
        [Header("Damage")]
        [SerializeField] DamageModifier[] _damageModifiers;
        [SerializeField] Vector3 _targetForce;

        protected void ApplyDamage(Status user, Status target)
        {
            target.GetComponent<HitPoint>().TakeDamage(CombatCommonFunc.CalcDamage(user, target, _damageModifiers));

            Vector3 right = user.transform.right * _targetForce.x;
            Vector3 up = user.transform.up * _targetForce.y;
            Vector3 forward = user.transform.forward * _targetForce.z;
            Vector3 forceDir = right + up + forward;
            float magnitude = forceDir.magnitude;
            if (magnitude > float.Epsilon)
                forceDir /= magnitude;

            target.GetComponent<Knockbackable>().Knockback(forceDir, magnitude);

            if (target.TryGetComponent(out StatusAffectable statusAffectable))
                ApplyStatusEffect(target, statusAffectable);
        }
    }
}
