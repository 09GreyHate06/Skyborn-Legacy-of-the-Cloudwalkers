using UnityEngine;
using SLOTC.Utils.StateMachine;

namespace SLOTC.Core.Player.States
{
    public class FreeLookMoveState : IState
    {
        private readonly int _freeLookAnimHash = Animator.StringToHash("FreeLookMove");
        private readonly int _moveMagnitudeParamHash = Animator.StringToHash("MoveMagnitude");

        private readonly PlayerMover _playerMover;
        private readonly Animator _animator;
        private readonly float _moveSpeed;
        private readonly float _rotationSpeed;
        private readonly float _animTransitionDuration;
        private readonly float _animDampTime;
        private readonly PlayerInput _playerInput;

        public FreeLookMoveState(PlayerMover playerMover, PlayerInput playerInput, Animator animator, float animTransitionDuration, float animDampTime, float moveSpeed, float rotationSpeed)
        {
            _playerMover = playerMover;
            _animator = animator;
            _moveSpeed = moveSpeed;
            _rotationSpeed = rotationSpeed;
            _animTransitionDuration = animTransitionDuration;
            _animDampTime = animDampTime;
            _playerInput = playerInput;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _animator.CrossFadeInFixedTime(_freeLookAnimHash, _animTransitionDuration);
        }

        public void OnExit()
        {
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

                velocity = _moveSpeed * inputMagnitude * heading;
            }

            velocity.y = _playerMover.velocity.y;
            _playerMover.velocity = velocity;
            _animator.SetFloat(_moveMagnitudeParamHash, inputMagnitude, _animDampTime, deltaTime);
        }
    }
}