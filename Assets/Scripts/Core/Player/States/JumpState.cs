using UnityEngine;
using SLOTC.Utils.StateMachine;

namespace SLOTC.Core.Player.States
{
    public class JumpState : IState
    {
        private int _jumpAnimHash = Animator.StringToHash("Jump");
        private PlayerMover _playerMover;
        private Animator _animator;
        private float _jumpForce;
        private float _animTransitionDuration;

        public JumpState(PlayerMover playerMover, Animator animator, float animTransitionDuration, float jumpForce)
        {
            _playerMover = playerMover;
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
            _jumpForce = jumpForce;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            if (_playerMover.IsGrounded)
                _playerMover.velocity.y = _jumpForce;

            _animator.CrossFadeInFixedTime(_jumpAnimHash, _animTransitionDuration);
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
        }
    }
}