using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Combat;
using System;
using System.Diagnostics;
using System.Globalization;

namespace SLOTC.Core.Player.States
{
    public class AttackState : IState
    {
        private PlayerMover _playerMover;
        private Animator _animator;
        private Attack[] _combo;
        private int _comboCounter;
        private float _comboGraceTime;
        private float _lastAttackTime = float.MinValue;

        private bool _applyForceFlag;

        public event Action OnAttackFinished;

        public AttackState(PlayerMover playerMover, Animator animator, Attack[] combo, float comboGraceTime)
        {
            _playerMover = playerMover;
            _animator = animator;
            _combo = combo;
            _comboGraceTime = comboGraceTime;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            if (!IsComboFinished())
                return;

            _playerMover.velocity.x = 0;
            _playerMover.velocity.z = 0;

            float timeSinceLastAttack = Time.unscaledTime - _lastAttackTime;
            if (timeSinceLastAttack > _comboGraceTime)
                _comboCounter = 0;

            Attack attack = _combo[_comboCounter++ % _combo.Length];
            _animator.CrossFadeInFixedTime(attack.AnimNameHash, attack.TransitionDuration);

            _applyForceFlag = false;
        }

        public void OnExit()
        {
            _lastAttackTime = Time.unscaledTime;
        }

        public void OnUpdate(float deltaTime)
        {
            int comboIndex = Mathf.Max(0, (_comboCounter - 1) % _combo.Length);
            if (!_applyForceFlag && GetNormalizedTime() >= _combo[comboIndex].ForceNormalizedTime)
            {
                Attack attack = _combo[comboIndex];
                Vector3 right = _playerMover.transform.right * attack.Force.x;
                Vector3 up = _playerMover.transform.up * attack.Force.y;
                Vector3 forward = _playerMover.transform.forward * attack.Force.z;
                _playerMover.AddForce(right + up + forward);
                _applyForceFlag = true;
            }

            if (IsAttackFinished())
            {
                OnAttackFinished?.Invoke();
            }
        }

        private bool IsComboFinished()
        {
            float n = GetNormalizedTime();
            int comboIndex = Mathf.Max(0, (_comboCounter - 1) % _combo.Length);
            return n < 0.0f || n >= _combo[comboIndex].ComboAttackExitNormalizedTime;
        }

        private bool IsAttackFinished()
        {
            return GetNormalizedTime() >= 1.0f - float.Epsilon;
        }

        private float GetNormalizedTime()
        {
            AnimatorStateInfo currentInfo = _animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextInfo = _animator.GetNextAnimatorStateInfo(0);
            Attack attack = _combo[0]; // they all have the same tag
            if (_animator.IsInTransition(0) && nextInfo.tagHash == attack.AnimTagHash)
            {
                return nextInfo.normalizedTime;
            }
            else if (!_animator.IsInTransition(0) && currentInfo.tagHash == attack.AnimTagHash)
            {
                return currentInfo.normalizedTime;
            }
            else
            {
                return -1.0f;
            }
        }
    }
}