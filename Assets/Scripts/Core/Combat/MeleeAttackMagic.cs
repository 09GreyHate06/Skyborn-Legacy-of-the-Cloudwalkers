using SLOTC.Core.Stats;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using SLOTC.Utils;

namespace SLOTC.Core.Combat
{
    [CreateAssetMenu(menuName = "Combat/Melee Attack Magic")]
    public class MeleeAttackMagic : AttackMagic
    {
        [Header("Melee Settings")]
        [SerializeField] bool _isAOE;
        [SerializeField] float _range;
        [SerializeField] float _damageDelay;
        [SerializeField] GameObject _fx;

        private const float _frontHAngle = 60.0f;
        public float Range { get { return _range; } }

        public override void Use(Status user)
        {
            Instantiate(CastFX, user.GetComponentInChildren<CastFxSpawnPoint>().transform);
            user.StartCoroutine(UseCo(user));
        }

        private IEnumerator UseCo(Status user)
        {
            Instantiate(_fx, user.GetComponentInChildren<AuraAOEFXSpawnPoint>().transform);

            yield return new WaitForSeconds(_damageDelay);

            string targetTag = user.CompareTag(Tags.Player) ? Tags.Enemy : Tags.Player;
            Collider[] cols = Physics.OverlapSphere(user.transform.position, _range);
            List<Status> alreadyDealtWith = new List<Status>();
            for (int i = 0; i < cols.Length; i++)
            {
                Hitbox targetHitbox = cols[i].GetComponent<Hitbox>();
                if (!cols[i].CompareTag(targetTag) || targetHitbox == null || alreadyDealtWith.Contains(targetHitbox.Owner)) 
                    continue;

                Status target = targetHitbox.Owner;

                if (CanAttack(user, target))
                    ApplyDamage(user, target);

                alreadyDealtWith.Add(target);
            }
        }

        private bool CanAttack(Status user, Status target)
        {
            if (target.GetComponent<HitPoint>().IsDead) 
                return false;
            if (_isAOE) 
                return true;

            Vector3 toTarget = target.transform.position - user.transform.position;
            toTarget.y = 0;
            float c = Mathf.Cos(_frontHAngle * Mathf.Deg2Rad);
            float dot = Vector3.Dot(toTarget.normalized, user.transform.forward);
            if (dot >= c)
                return true;

            return false;
        }
    }
}
