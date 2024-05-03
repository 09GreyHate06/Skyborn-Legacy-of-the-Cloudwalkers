
using System.Collections.Generic;
using UnityEngine;
using SLOTC.Core.Stats;

namespace SLOTC.Core.Combat
{
    public class StatusAffectable : MonoBehaviour
    {
        private Dictionary<string, StatusEffectHolder> _statusEffects = new Dictionary<string, StatusEffectHolder>();

        private Status _stats;
        private HitPoint _hp;
        private SkillPoint _sp;

        private Queue<string> _toBeRemoved = new Queue<string>();

        private void Awake()
        {
            _stats = GetComponent<Status>();
            _hp = GetComponent<HitPoint>();
            _sp = GetComponent<SkillPoint>();
        }

        private void Update()
        {
            foreach (var pair in _statusEffects)
            {
                StatusEffectHolder effectHolder = pair.Value;
                effectHolder.time += Time.deltaTime;
                effectHolder.timeOneSec += Time.deltaTime;

                // do vps
                if (effectHolder.timeOneSec >= 1.0f)
                {
                    int damage = CombatCommonFunc.CalcDamage(effectHolder.source, _stats, new DamageModifier[] { effectHolder.statusEffect.DamagePerSeconds });
                    _hp.TakeDamage(damage);
                    _hp.Regen(effectHolder.statusEffect.HPRegenPerSeconds);
                    _sp.Regen(effectHolder.statusEffect.SPRegenPerSeconds);

                    effectHolder.timeOneSec = 0.0f;
                }


                if (effectHolder.time > effectHolder.statusEffect.Duration)
                {
                    _toBeRemoved.Enqueue(pair.Key);
                }
            }

            while (_toBeRemoved.Count > 0)
            {
                string id = _toBeRemoved.Dequeue();

                //if (TryGetComponent(out PlayerMovement playerMovement))
                //{
                //    playerMovement.MoveSpeedPercentModifiers -= sef.statusEffect.movespeedPercentModifier;
                //    playerMovement.MoveSpeedFlatModifiers -= sef.statusEffect.movespeedFlatModifier;
                //}
                //else if (TryGetComponent(out EnemyNavmeshHolder enemyMovement))
                //{
                //    enemyMovement.MoveSpeedPercentModifiers -= sef.statusEffect.movespeedPercentModifier;
                //    enemyMovement.MoveSpeedFlatModifiers -= sef.statusEffect.movespeedFlatModifier;
                //}

                _stats.RemoveModifiers(_statusEffects[id].statusEffect);
                _statusEffects.Remove(id);
            }
        }

        public void AddEffect(Status source, StatusEffect statusEffect)
        {
            Debug.Assert(source != null && statusEffect != null);
            if (_statusEffects.ContainsKey(statusEffect.GetID()))
            {
                _statusEffects[statusEffect.GetID()].time = 0.0f;
                return;
            }

            _stats.AddModifiers(statusEffect);

            //if(TryGetComponent(out PlayerMovement playerMovement))
            //{
            //    playerMovement.MoveSpeedPercentModifiers += statusEffect.movespeedPercentModifier;
            //    playerMovement.MoveSpeedFlatModifiers += statusEffect.movespeedFlatModifier;
            //}
            //else if(TryGetComponent(out EnemyNavmeshHolder enemyMovement))
            //{
            //    enemyMovement.MoveSpeedPercentModifiers += statusEffect.movespeedPercentModifier;
            //    enemyMovement.MoveSpeedFlatModifiers += statusEffect.movespeedFlatModifier;
            //}

            _statusEffects[statusEffect.GetID()] = new StatusEffectHolder(source, statusEffect, 0.0f);
        }

        public void RemoveEffect(StatusEffect statusEffect)
        {
            if (!_statusEffects.ContainsKey(statusEffect.GetID())) return;
            _toBeRemoved.Enqueue(statusEffect.GetID());
        }

        private class StatusEffectHolder
        {
            public Status source;
            public StatusEffect statusEffect;
            public float time;
            public float timeOneSec;

            public StatusEffectHolder(Status source, StatusEffect statusEffect, float time)
            {
                this.source = source;
                this.statusEffect = statusEffect;
                this.time = time;
            }
        }
    }
}