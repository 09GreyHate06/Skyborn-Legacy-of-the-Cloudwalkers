using UnityEngine;
using SLOTC.Utils.StateMachine;

namespace SLOTC.Core.Player.States
{
    public class MoveState : IState
    {
        private int _moveAnimHash = Animator.StringToHash("Move");
        private int _moveMagnitudeParamHash = Animator.StringToHash("MoveMagnitude");

        private PlayerMover _playerMover;
        private Animator _animator;
        private float _moveSpeed;
        private float _rotationSpeed;
        private float _animTransitionDuration;
        private float _animDampTime;
        PlayerInput _playerInput;

        public MoveState(PlayerMover playerMover, Animator animator, float animTransitionDuration, float animDampTime, float moveSpeed, float rotationSpeed)
        {
            _playerMover = playerMover;
            _animator = animator;
            _moveSpeed = moveSpeed;
            _rotationSpeed = rotationSpeed;
            _animTransitionDuration = animTransitionDuration;
            _animDampTime = animDampTime;
            _playerInput = Object.FindObjectOfType<PlayerInput>();
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
            Vector2 inputAxis = _playerInput.Axis;
            float inputMagnitude = Mathf.Clamp01(inputAxis.magnitude);

            if(inputMagnitude > float.Epsilon)
            {
                Vector3 forward = Camera.main.transform.parent.forward;
                Vector3 right = Camera.main.transform.parent.right;
                Vector3 heading = right * inputAxis.x + forward * inputAxis.y;

                Quaternion headingQuat = Quaternion.LookRotation(heading, Vector3.up);
                _playerMover.transform.rotation = Quaternion.RotateTowards(_playerMover.transform.rotation, headingQuat, _rotationSpeed * deltaTime);
                Vector3 velocity = _playerMover.transform.forward * _moveSpeed * inputMagnitude;
                _playerMover.velocity.x = velocity.x;
                _playerMover.velocity.z = velocity.z;
                _animator.SetFloat(_moveMagnitudeParamHash, inputMagnitude, _animDampTime, deltaTime);
            }
        }
    }
}