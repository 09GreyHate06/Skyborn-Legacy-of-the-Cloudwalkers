using UnityEngine;
using SLOTC.Core.Stats;
using System.Collections.Generic;
using SLOTC.Utils;

namespace SLOTC.Core.Combat
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class MeleeWeaponHandler : WeaponHandler
    {
        private Collider _weaponCollider;
        private List<Status> _alreadyDealtWith = new List<Status>();

        private Attack _activeAttack;
        private string _targetTag;
        private Vector3 _activeAttackKnockbackForceDir;
        private float _activeAttackKnockbackForceMagnitude;

        private void Awake()
        {
            _weaponCollider = GetComponent<Collider>();
            GetComponent<Rigidbody>().isKinematic = true;
        }

        private void Start()
        {
            if (_user.tag == Tags.Player)
                _targetTag = Tags.Enemy;
            else
                _targetTag = Tags.Player;

            Deactivate();
        }

        public override void Activate(Attack attack)
        {
            _alreadyDealtWith.Clear();
            _activeAttack = attack;
            _weaponCollider.enabled = true;

            Vector3 right = _user.transform.right * _activeAttack.TargetForce.x;
            Vector3 up = _user.transform.up * _activeAttack.TargetForce.y;
            Vector3 forward = _user.transform.forward * _activeAttack.TargetForce.z;
            _activeAttackKnockbackForceDir = right + up + forward;
            _activeAttackKnockbackForceMagnitude = _activeAttackKnockbackForceDir.magnitude;
            if(_activeAttackKnockbackForceMagnitude > float.Epsilon)
                _activeAttackKnockbackForceDir /= _activeAttackKnockbackForceMagnitude;
        }

        public override void Deactivate()
        {
            _weaponCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == _weaponCollider || !other.CompareTag(_targetTag) || !other.TryGetComponent(out Hitbox hitbox) || _alreadyDealtWith.Contains(hitbox.Owner))
                return;

            int damage = _activeAttack.CalcDamage(_user, hitbox.Owner);
            hitbox.Owner.GetComponent<HitPoint>().TakeDamage(damage);
            hitbox.Owner.GetComponent<Knockbackable>().Knockback(_activeAttackKnockbackForceDir, _activeAttackKnockbackForceMagnitude);
        }
    }
}