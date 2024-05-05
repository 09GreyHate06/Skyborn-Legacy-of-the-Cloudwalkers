
using SLOTC.Utils.StateMachine;
using UnityEngine;
using SLOTC.Core.Combat;
using System;
using SLOTC.Core.Combat.Animation;
using Animancer;

namespace SLOTC.Core.Enemy.States
{
    public class AttackState : IState
    {
        private readonly EnemyMover _enemyMover;
        private readonly AnimancerComponent _animancer;
        private readonly Transform _target;
        private readonly WeaponHandler _weaponHandler;
        private readonly SingleAttack[] _attacks;
        private readonly AnimancerEvent.Sequence[] _attacksAnimEvents;
        private readonly float _comboGraceTime;

        private SingleAttack _activeAttack;
        private int _comboCounter;
        private float _lastAttackTime = float.MinValue;

        public bool CanExit { get; set; }

        public event Action OnAnimationEnded;

        public AttackState(EnemyMover enemyMover, AnimancerComponent animancer, Transform target, WeaponHandler weaponHandler, SingleAttack[] attacks, float comboGraceTime)
        {
            _enemyMover = enemyMover;
            _animancer = animancer;
            _target = target;
            _weaponHandler = weaponHandler;
            _attacks = attacks;
            _comboGraceTime = comboGraceTime;

            _attacksAnimEvents = new AnimancerEvent.Sequence[attacks.Length];
            for (int i = 0; i < attacks.Length; i++)
            {
                var events = new AnimancerEvent.Sequence(_attacks[i].AttackAnim.Events);
                events.AddCallback(CombatAnimationEventNames.ApplyForce, ApplyForce);
                events.AddCallback(CombatAnimationEventNames.ActivateWeapon, ActivateWeapon);
                events.AddCallback(CombatAnimationEventNames.DeactivateWeapon, DeactivateWeapon);
                events.AddCallback(CombatAnimationEventNames.Exit, AnimationEnded);
                events.OnEnd = null;
                _attacksAnimEvents[i] = events;
            }
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            CanExit = false;

            _enemyMover.ResetPath();

            float timeSinceLastAttack = Time.realtimeSinceStartup - _lastAttackTime;
            if (timeSinceLastAttack > _comboGraceTime)
                _comboCounter = 0;

            int comboIndex = _comboCounter++ % _attacks.Length;
            _activeAttack = _attacks[comboIndex];

            AnimancerState state = _animancer.Play(_activeAttack.AttackAnim);
            state.Events = _attacksAnimEvents[comboIndex];
        }

        public void OnExit()
        {
            _activeAttack = null;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_activeAttack == null)
                return;

            Vector3 toTarget = _target.position - _enemyMover.transform.position;
            toTarget.y = 0.0f;
            Quaternion headingQuat = Quaternion.LookRotation(toTarget, Vector3.up);
            _enemyMover.transform.rotation = Quaternion.RotateTowards(_enemyMover.transform.rotation, headingQuat, _enemyMover.AngularSpeed * deltaTime);
        }

        private void ApplyForce()
        {
            Vector3 right = _enemyMover.transform.right * _activeAttack.Force.x;
            Vector3 up = _enemyMover.transform.up * _activeAttack.Force.y;
            Vector3 forward = _enemyMover.transform.forward * _activeAttack.Force.z;
            _enemyMover.AddForce(right + up + forward);
        }

        private void ActivateWeapon()
        {
            _weaponHandler.Activate(_activeAttack);
        }

        private void DeactivateWeapon()
        {
            _lastAttackTime = Time.realtimeSinceStartup;
            _weaponHandler.Deactivate();
            CanExit = true;
        }

        private void AnimationEnded()
        {
            OnAnimationEnded?.Invoke();
        }
    }
}