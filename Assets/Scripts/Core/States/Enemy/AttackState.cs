
using SLOTC.Utils.StateMachine;
using UnityEngine;
using SLOTC.Core.Combat;
using System;
using SLOTC.Core.Combat.Animation;
using SLOTC.Core.Movement.Enemy;

namespace SLOTC.Core.States.Enemy
{
    public class AttackState : IState
    {
        public enum EventType
        {
            Enter,
            AttackEnded,
            AnimationEnded,
            Exit
        }

        private readonly EnemyMover _enemyMover;
        private readonly Animator _animator;
        private readonly CombatAnimationEvent _combatAnimationEvent;
        private readonly Transform _target;
        private readonly WeaponHandler _weaponHandler;
        private readonly SingleAttack[] _combo;
        private readonly float _comboGraceTime;
        private readonly float _transitionDuration;

        private SingleAttack _activeAttack;
        private int _comboCounter;
        private float _lastAttackTime = float.MinValue;

        public event Action<EventType> OnEvent;

        public AttackState(EnemyMover enemyMover, Animator animator, float transitionDuration, Transform target, WeaponHandler weaponHandler, SingleAttack[] combo, float comboGraceTime)
        {
            _enemyMover = enemyMover;
            _animator = animator;
            _transitionDuration = transitionDuration;
            _target = target;
            _weaponHandler = weaponHandler;
            _combo = combo;
            _comboGraceTime = comboGraceTime;
            _combatAnimationEvent = _animator.GetComponent<CombatAnimationEvent>();
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _combatAnimationEvent.Listeners += OnCombatAnimEvent;
            _enemyMover.ResetPath();
            //if (!_playerMover.IsGrounded)
            //{
            //    _playerMover.velocity.y = 5.0f;
            //}

            float timeSinceLastAttack = Time.realtimeSinceStartup - _lastAttackTime;
            if (timeSinceLastAttack > _comboGraceTime)
                _comboCounter = 0;

            int comboIndex = _comboCounter++ % _combo.Length;
            _activeAttack = _combo[comboIndex];
            _animator.CrossFadeInFixedTime(_activeAttack.AnimNameHash, _transitionDuration);

            OnEvent?.Invoke(EventType.Enter);
        }

        public void OnExit()
        {
            _combatAnimationEvent.Listeners -= OnCombatAnimEvent;
            _activeAttack = null;
            OnEvent?.Invoke(EventType.Exit);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_activeAttack == null)
                return;

            Vector3 toTarget = _target.position - _enemyMover.transform.position;
            toTarget.y = 0.0f;
            Quaternion headingQuat = Quaternion.LookRotation(toTarget, Vector3.up);
            _enemyMover.transform.rotation = Quaternion.RotateTowards(_enemyMover.transform.rotation, headingQuat, _enemyMover.AngularSpeed * deltaTime);

            //if (_applyForceFlag && n >= _activeAttack.AnimNormalizedTimeToApplyForce)
            //{
            //    Vector3 right = _playerMover.transform.right * _activeAttack.Force.x;
            //    Vector3 up = _playerMover.transform.up * _activeAttack.Force.y;
            //    Vector3 forward = _playerMover.transform.forward * _activeAttack.Force.z;
            //    _playerMover.AddForce(right + up + forward);
            //    _applyForceFlag = false;
            //}
        }

        private void ApplyForce()
        {
            Vector3 right = _enemyMover.transform.right * _activeAttack.Force.x;
            Vector3 up = _enemyMover.transform.up * _activeAttack.Force.y;
            Vector3 forward = _enemyMover.transform.forward * _activeAttack.Force.z;
            _enemyMover.AddForce(right + up + forward);
        }

        private void OnCombatAnimEvent(CombatAnimationEvent.Type type)
        {
            switch (type)
            {
                case CombatAnimationEvent.Type.ApplyForce:
                    ApplyForce();
                    break;

                case CombatAnimationEvent.Type.ActivateWeapon:
                    _weaponHandler.Activate(_activeAttack);
                    break;

                case CombatAnimationEvent.Type.DeactivateWeapon:
                    _weaponHandler.Deactivate();
                    _lastAttackTime = Time.realtimeSinceStartup;
                    OnEvent?.Invoke(EventType.AttackEnded);
                    break;

                case CombatAnimationEvent.Type.AnimationEnded:
                    OnEvent?.Invoke(EventType.AnimationEnded);
                    break;
            }
        }
    }
}