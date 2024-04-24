using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Combat;
using System;
using System.Globalization;
using UnityEngine.InputSystem;

namespace SLOTC.Core.Player.States
{
    public class AttackState : MoveableState
    {
        public enum EventType
        {
            Enter,
            AttackEnded,
            AnimationEnded,
            Exit
        }

        private readonly Animator _animator;
        private readonly PlayerInput _playerInput;
        private readonly Attack[] _combo;
        private readonly float _comboGraceTime;
        private int _comboCounter;
        private float _lastAttackTime = float.MinValue;


        private Attack _activeAttack;
        private bool _applyForceFlag;

        private bool _eventAttackEndedCalled;
        private bool _eventAnimationEndedCalled;
        public Action<EventType> OnEvent;


        public AttackState(PlayerMover playerMover, PlayerInput playerInput, Animator animator, Attack[] combo, float comboGraceTime, float rotationSpeed)
            : base(playerMover, 0.0f, rotationSpeed)
        {
            _playerInput = playerInput;
            _animator = animator;
            _combo = combo;
            _comboGraceTime = comboGraceTime;
        }

        public override string GetID() 
        {
            return GetType().ToString();
        }

        public override void OnEnter()
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
            _eventAttackEndedCalled = false;
            _eventAnimationEndedCalled = false;
            OnEvent?.Invoke(EventType.Enter);
        }

        public override void OnExit()
        {
            _applyForceFlag = false;
            _eventAttackEndedCalled = true;
            _eventAnimationEndedCalled = true;
            _activeAttack = null;
            OnEvent?.Invoke(EventType.Exit);
        }

        public override void OnUpdate(float deltaTime)
        {
            Vector2 inputAxis = _playerInput.Axis;
            FreeLookMove(inputAxis, inputAxis.magnitude, deltaTime);

            if (_activeAttack == null)
                return;

            float n = GetNormalizedTime();

            if (_applyForceFlag && n >= _activeAttack.AnimNormalizedTimeToApplyForce)
            {
                Vector3 right = _playerMover.transform.right * _activeAttack.Force.x;
                Vector3 up = _playerMover.transform.up * _activeAttack.Force.y;
                Vector3 forward = _playerMover.transform.forward * _activeAttack.Force.z;
                _playerMover.AddForce(right + up + forward);
                _applyForceFlag = false;
            }

            if(!_eventAttackEndedCalled && n >= _activeAttack.AnimNormalizedExitTime)
            {
                OnEvent?.Invoke(EventType.AttackEnded);
                _lastAttackTime = Time.realtimeSinceStartup;
                _eventAttackEndedCalled = true;
            }

            if(!_eventAnimationEndedCalled && n >= 1.0f - float.Epsilon)
            {
                OnEvent?.Invoke(EventType.AnimationEnded);
                _eventAnimationEndedCalled = true;
            }
        }

        protected float GetNormalizedTime()
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