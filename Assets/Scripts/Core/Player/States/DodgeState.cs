using UnityEngine;
using SLOTC.Utils.StateMachine;
using System;
using SLOTC.Core.Combat;

namespace SLOTC.Core.Player.States
{
    public class DodgeState : IState
    {
        public enum EventType
        {
            Enter,
            DodgeEnded,
            AnimationEnded,
            Exit
        }

        private readonly PlayerMover _playerMover;
        private readonly PlayerInput _playerInput;
        private readonly TargetLocker _targetLocker;

        private readonly int _dodgeAnimHash = Animator.StringToHash("Dodge");
        private readonly int _inputXParamHash = Animator.StringToHash("InputX");
        private readonly int _inputYParamHash = Animator.StringToHash("InputY");
        private readonly Animator _animator;
        private readonly float _animNormExitTime;
        private readonly float _animTransitionDuration;
        private readonly float _animDampTime;
        private readonly float _dodgeForce;

        private Vector2 _inputXYParam;

        private bool _eventDodgeEndedCalled;
        private bool _eventAnimationEndedCalled;
        public Action<EventType> OnEvent;

        public DodgeState(PlayerMover playerMover, PlayerInput playerInput, TargetLocker targetLocker, Animator animator, float animNormExitTime, float animTransitionDuration, float animDampTime, float dodgeForce)
        {
            _playerMover = playerMover;
            _playerInput = playerInput;
            _targetLocker = targetLocker;
            _animator = animator;
            _animNormExitTime = animNormExitTime;
            _animTransitionDuration = animTransitionDuration;
            _animDampTime = animDampTime;
            _dodgeForce = dodgeForce;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _animator.CrossFadeInFixedTime(_dodgeAnimHash, _animTransitionDuration);

            if(!_targetLocker.HasTarget || _playerInput.Axis.sqrMagnitude <= float.Epsilon)
            {
                _inputXYParam = new Vector2(0.0f, 1.0f);
                _playerMover.AddForce(_playerMover.transform.forward * _dodgeForce);
            }
            else
            {
                _inputXYParam = _playerInput.Axis;
                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0.0f;
                forward = forward.normalized;
                Vector3 right = Vector3.Cross(Vector3.up, forward);
                Vector3 heading = (right * _inputXYParam.x + forward * _inputXYParam.y).normalized;
                _playerMover.AddForce(heading * _dodgeForce);
            }

            _eventDodgeEndedCalled = false;
            _eventAnimationEndedCalled = false;
            OnEvent?.Invoke(EventType.Enter);
        }

        public void OnExit()
        {
            _eventDodgeEndedCalled = true;
            _eventAnimationEndedCalled = true;
            OnEvent?.Invoke(EventType.Exit);
        }

        public void OnUpdate(float deltaTime)
        {
            if(!_eventDodgeEndedCalled && GetNormalizedTime() >= _animNormExitTime)
            {
                OnEvent?.Invoke(EventType.DodgeEnded);
                _eventDodgeEndedCalled = true;
            }

            if(!_eventAnimationEndedCalled && GetNormalizedTime() >= 1.0f - float.Epsilon)
            {
                OnEvent?.Invoke(EventType.AnimationEnded);
                _eventAnimationEndedCalled = true;
            }

            _animator.SetFloat(_inputXParamHash, _inputXYParam.x, _animDampTime, deltaTime);
            _animator.SetFloat(_inputYParamHash, _inputXYParam.y, _animDampTime, deltaTime);
        }

        private float GetNormalizedTime()
        {
            AnimatorStateInfo currentInfo = _animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextInfo = _animator.GetNextAnimatorStateInfo(0);

            if (currentInfo.shortNameHash == _dodgeAnimHash)
            {
                return currentInfo.normalizedTime;
            }
            else if (nextInfo.shortNameHash == _dodgeAnimHash)
            {
                return nextInfo.normalizedTime;
            }
            else
                return 0.0f;

        }
    }
}