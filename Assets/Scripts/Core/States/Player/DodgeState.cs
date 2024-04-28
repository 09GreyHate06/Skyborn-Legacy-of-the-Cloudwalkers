using UnityEngine;
using SLOTC.Utils.StateMachine;
using System;
using SLOTC.Core.Combat;
using SLOTC.Core.Combat.Animation;
using SLOTC.Core.Input;
using SLOTC.Core.Movement.Player;

namespace SLOTC.Core.States.Player
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
        private readonly CombatAnimationEvent _combatAnimationEvent;
        private readonly float _animTransitionDuration;
        private readonly float _animDampTime;
        private readonly float _dodgeForce;

        private Vector2 _inputXYParam;

        private bool _eventDodgeEndedCalled;
        private bool _eventAnimationEndedCalled;
        public Action<EventType> OnEvent;

        public DodgeState(PlayerMover playerMover, PlayerInput playerInput, TargetLocker targetLocker, Animator animator, float animTransitionDuration, float animDampTime, float dodgeForce)
        {
            _playerMover = playerMover;
            _playerInput = playerInput;
            _targetLocker = targetLocker;
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
            _animDampTime = animDampTime;
            _dodgeForce = dodgeForce;

            _combatAnimationEvent = _animator.GetComponent<CombatAnimationEvent>();
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _playerMover.velocity.x = 0.0f;
            _playerMover.velocity.z = 0.0f;

            _combatAnimationEvent.Listeners += OnCombatAnimEvent;

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
            _combatAnimationEvent.Listeners -= OnCombatAnimEvent;
            _eventDodgeEndedCalled = true;
            _eventAnimationEndedCalled = true;
            OnEvent?.Invoke(EventType.Exit);
        }

        public void OnUpdate(float deltaTime)
        {
            _animator.SetFloat(_inputXParamHash, _inputXYParam.x, _animDampTime, deltaTime);
            _animator.SetFloat(_inputYParamHash, _inputXYParam.y, _animDampTime, deltaTime);
        }

        private void OnCombatAnimEvent(CombatAnimationEvent.Type type)
        {
            switch(type)
            {
                case CombatAnimationEvent.Type.DodgeEnded:
                    if (_eventDodgeEndedCalled)
                        break;

                    OnEvent?.Invoke(EventType.DodgeEnded);
                    _eventDodgeEndedCalled = true;
                    break;

                case CombatAnimationEvent.Type.ExitTime:
                    if (_eventAnimationEndedCalled) 
                        break;

                    OnEvent?.Invoke(EventType.AnimationEnded);
                    _eventAnimationEndedCalled = true;
                    break;
            }
        }
    }
}