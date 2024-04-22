using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Combat;
using System;
using System.Globalization;
using UnityEngine.InputSystem;

namespace SLOTC.Core.Player.States
{
    public class AttackState : IState
    {
        private readonly PlayerMover _playerMover;
        private readonly Animator _animator;
        private readonly Attack[] _combo;
        private readonly float _comboGraceTime;
        private readonly PlayerInput _playerInput;
        private readonly float _rotationSpeed;
        private int _comboCounter;
        private float _lastAttackTime = float.MinValue;


        private Attack _activeAttack;
        private bool _applyForceFlag;

        private bool _onAttackFinishedCalled = true;
        private bool _onAnimationFinishedCalled = true;
        public event Action OnAttackFinished;
        public event Action OnAnimationFinished;

        public AttackState(PlayerMover playerMover, PlayerInput playerInput, Animator animator, Attack[] combo, float comboGraceTime, float rotationSpeed)
        {
            _playerMover = playerMover;
            _playerInput = playerInput;
            _animator = animator;
            _combo = combo;
            _comboGraceTime = comboGraceTime;
            _rotationSpeed = rotationSpeed;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _playerMover.velocity.x = 0.0f;
            _playerMover.velocity.z = 0.0f;
            //if (!_playerMover.IsGrounded)
            //{
            //    _playerMover.velocity.y = 5.0f;
            //}

            float timeSinceLastAttack = Time.realtimeSinceStartup - _lastAttackTime;
            if (timeSinceLastAttack > _comboGraceTime)
                _comboCounter = 0;

            int comboIndex = _comboCounter++ % _combo.Length;
            _activeAttack = _combo[comboIndex];
            _animator.CrossFade(_activeAttack.AnimNameHash, _activeAttack.AnimNormalizedTransitionDuration, 0, _activeAttack.AnimNormalizedTransitionOffset);

            _applyForceFlag = true;
            _onAttackFinishedCalled = false;
            _onAnimationFinishedCalled = false;
        }

        public void OnExit()
        {
            _activeAttack = null;
        }

        public void OnUpdate(float deltaTime)
        {
            Vector2 inputAxis = _playerInput.Axis;
            float inputMagnitude = Mathf.Clamp01(inputAxis.magnitude);

            Vector3 velocity = Vector3.zero;
            if (inputMagnitude > float.Epsilon)
            {
                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0.0f;
                forward = forward.normalized;
                Vector3 right = Camera.main.transform.right;
                Vector3 heading = (right * inputAxis.x + forward * inputAxis.y).normalized;

                Quaternion headingQuat = Quaternion.LookRotation(heading, Vector3.up);
                _playerMover.transform.rotation = Quaternion.RotateTowards(_playerMover.transform.rotation, headingQuat, _rotationSpeed * deltaTime);
            }

            if (_activeAttack == null)
                return;

            if(_applyForceFlag && GetNormalizedTime() >= _activeAttack.AnimNormalizedTimeToApplyForce)
            {
                Vector3 right = _playerMover.transform.right * _activeAttack.Force.x;
                Vector3 up = _playerMover.transform.up * _activeAttack.Force.y;
                Vector3 forward = _playerMover.transform.forward * _activeAttack.Force.z;
                _playerMover.AddForce(right + up + forward);
                _applyForceFlag = false;
            }

            if (!_onAttackFinishedCalled && IsAttackFinished())
            {
                _lastAttackTime = Time.realtimeSinceStartup;
                OnAttackFinished?.Invoke();
                _onAttackFinishedCalled = true;
            }

            if(!_onAnimationFinishedCalled && IsAnimationFinished())
            {
                OnAnimationFinished?.Invoke();
                _onAnimationFinishedCalled = true;
            }
        }

        private bool IsAttackFinished()
        {
            return GetNormalizedTime() >= _activeAttack.AnimNormalizedExitTime;
        }

        private bool IsAnimationFinished()
        {
            return GetNormalizedTime() >= 1.0f - float.Epsilon;
        }

        private float GetNormalizedTime()
        {
            AnimatorStateInfo currentInfo = _animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextInfo = _animator.GetNextAnimatorStateInfo(0);

            if (currentInfo.shortNameHash == _activeAttack.AnimNameHash)
                return currentInfo.normalizedTime;
            else if (nextInfo.shortNameHash == _activeAttack.AnimNameHash)
                return nextInfo.normalizedTime;
            else
                return 0.0f;
        }
    }
}