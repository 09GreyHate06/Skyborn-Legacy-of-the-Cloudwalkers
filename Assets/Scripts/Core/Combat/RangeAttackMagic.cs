
using SLOTC.Core.Stats;
using SLOTC.Utils;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace SLOTC.Core.Combat
{
    [CreateAssetMenu(menuName = "Combat/Range Attack Magic")]
    public class RangeAttackMagic : AttackMagic
    {
        [Header("Range Settings")]
        [SerializeField] bool _isImpactAOE;
        [SerializeField] float _impactAOERadius;
        [SerializeField] Projectile _projectile;

        public bool IsImpactAOE { get { return _isImpactAOE; } }
        public float ImpactAOERadius { get { return _impactAOERadius; } }

        public override void Use(Status user)
        {
            Instantiate(CastFX, user.GetComponentInChildren<CastFxSpawnPoint>().transform);
            Projectile p = Instantiate(_projectile, user.GetComponentInChildren<CastFxSpawnPoint>().transform.position, Quaternion.LookRotation(user.transform.forward, Vector3.up));
            Physics.IgnoreCollision(user.GetComponent<Collider>(), p.GetComponent<Collider>());


            p.OnHit += (Collision c) =>
            {
                OnProjectileHit(p, c, user);
            };
        }

        private void OnProjectileHit(Projectile p, Collision hit, Status user)
        {
            string targetTag = user.CompareTag(Tags.Player) ? Tags.Enemy : Tags.Player;
            
            if (!_isImpactAOE && hit.gameObject.CompareTag(targetTag))
            {
                if (!hit.gameObject.TryGetComponent(out Hitbox hitbox))
                    return;

                Status target = hitbox.Owner;
                if (!target.GetComponent<HitPoint>().IsDead)
                {
                    ApplyDamage(user, target);
                }

            }
            else if (_isImpactAOE)
            {
                Collider[] cols = Physics.OverlapSphere(p.transform.position, _impactAOERadius);
                List<Status> alreadyDealtWith = new List<Status>();
                for (int i = 0; i < cols.Length; i++)
                {
                    Hitbox targetHitbox = cols[i].GetComponent<Hitbox>();
                    if (!cols[i].CompareTag(targetTag) || targetHitbox == null || alreadyDealtWith.Contains(targetHitbox.Owner)) 
                        continue;

                    Status target = targetHitbox.Owner;

                    if (!target.GetComponent<HitPoint>().IsDead)
                        ApplyDamage(user, target);

                    alreadyDealtWith.Add(target);
                }
            }
        }
    }
}