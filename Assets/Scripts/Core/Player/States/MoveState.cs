using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Combat;

namespace SLOTC.Core.Player.States
{
    public class MoveState : IState
    {
        private readonly int _moveAnimHash = Animator.StringToHash("Move");
        private readonly int _moveMagnitudeParamHash = Animator.StringToHash("MoveMagnitude");
        private readonly int _inputXParamHash = Animator.StringToHash("InputX");
        private readonly int _inputYParamHash = Animator.StringToHash("InputY");

        private readonly PlayerMover _playerMover;
        private readonly PlayerInput _playerInput;
        private readonly TargetLocker _targetLocker;
        private readonly Animator _animator;
        private readonly float _moveSpeed;
        private readonly float _rotationSpeed;
        private readonly float _animTransitionDuration;
        private readonly float _animDampTime;

        public MoveState(PlayerMover playerMover, PlayerInput playerInput, TargetLocker targetLocker, Animator animator, float animTransitionDuration, float animDampTime, float moveSpeed, float rotationSpeed)
        {
            _playerMover = playerMover;
            _playerInput = playerInput;
            _targetLocker = targetLocker;
            _animator = animator;
            _moveSpeed = moveSpeed;
            _rotationSpeed = rotationSpeed;
            _animTransitionDuration = animTransitionDuration;
            _animDampTime = animDampTime;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _animator.CrossFadeInFixedTime(_moveAnimHash, _animTransitionDuration);
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
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

                velocity = _moveSpeed * inputMagnitude * heading;
            }

            velocity.y = _playerMover.velocity.y;
            _playerMover.velocity = velocity;
            _animator.SetFloat(_moveMagnitudeParamHash, inputMagnitude, _animDampTime, deltaTime);
            _animator.SetFloat(_inputXParamHash, 0.0f, _animDampTime, deltaTime);
            _animator.SetFloat(_inputYParamHash, 1.0f, _animDampTime, deltaTime);
        }

        private void TargetLocked(float deltaTime)
        {
            Vector2 inputAxis = _playerInput.Axis;
            float inputMagnitude = Mathf.Clamp01(inputAxis.magnitude);

            Vector3 velocity = Vector3.zero;
            if (inputMagnitude > float.Epsilon)
            {
                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0.0f;
                forward = forward.normalized;

                Quaternion headingQuat = Quaternion.LookRotation(forward, Vector3.up);
                _playerMover.transform.rotation = Quaternion.RotateTowards(_playerMover.transform.rotation, headingQuat, _rotationSpeed * deltaTime);

                Vector3 right = Vector3.Cross(Vector3.up, forward);
                Vector3 heading = (right * inputAxis.x + forward * inputAxis.y).normalized;

                velocity = _moveSpeed * inputMagnitude * heading;
            }

            velocity.y = _playerMover.velocity.y;
            _playerMover.velocity = velocity;
            _animator.SetFloat(_inputXParamHash, inputAxis.x, _animDampTime, deltaTime);
            _animator.SetFloat(_inputYParamHash, inputAxis.y, _animDampTime, deltaTime);
            _animator.SetFloat(_moveMagnitudeParamHash, inputMagnitude, _animDampTime, deltaTime);
        }
    }
}