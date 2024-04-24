using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Combat;

namespace SLOTC.Core.Player.States
{
    public class MoveState : MoveableState
    {
        private readonly int _moveAnimHash = Animator.StringToHash("Move");
        private readonly int _moveMagnitudeParamHash = Animator.StringToHash("MoveMagnitude");
        private readonly int _inputXParamHash = Animator.StringToHash("InputX");
        private readonly int _inputYParamHash = Animator.StringToHash("InputY");

        private readonly PlayerInput _playerInput;
        private readonly TargetLocker _targetLocker;
        private readonly Animator _animator;
        private readonly float _animTransitionDuration;
        private readonly float _animDampTime;

        public MoveState(PlayerMover playerMover, PlayerInput playerInput, TargetLocker targetLocker, Animator animator, float animTransitionDuration, float animDampTime, float moveSpeed, float rotationSpeed)
            : base(playerMover, moveSpeed, rotationSpeed)
        {
            _playerInput = playerInput;
            _targetLocker = targetLocker;
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
            _animDampTime = animDampTime;
        }

        public override string GetID()
        {
            return GetType().ToString();
        }

        public override void OnEnter()
        {
            _animator.CrossFadeInFixedTime(_moveAnimHash, _animTransitionDuration);
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
            if(!_targetLocker.HasTarget)
                FreeLook(deltaTime);
            else
                TargetLocked(deltaTime);
        }

        private void FreeLook(float deltaTime)
        {
            Vector2 inputAxis = _playerInput.Axis;
            float inputMagnitude = Mathf.Clamp01(inputAxis.magnitude);
            FreeLookMove(inputAxis, inputMagnitude, deltaTime);

            _animator.SetFloat(_moveMagnitudeParamHash, inputMagnitude, _animDampTime, deltaTime);
            _animator.SetFloat(_inputXParamHash, 0.0f, _animDampTime, deltaTime);
            _animator.SetFloat(_inputYParamHash, 1.0f, _animDampTime, deltaTime);
        }

        private void TargetLocked(float deltaTime)
        {
            Vector2 inputAxis = _playerInput.Axis;
            float inputMagnitude = Mathf.Clamp01(inputAxis.magnitude);
            TargetLockedMove(inputAxis, inputMagnitude, deltaTime);

            _animator.SetFloat(_inputXParamHash, inputAxis.x, _animDampTime, deltaTime);
            _animator.SetFloat(_inputYParamHash, inputAxis.y, _animDampTime, deltaTime);
            _animator.SetFloat(_moveMagnitudeParamHash, inputMagnitude, _animDampTime, deltaTime);
        }
    }
}