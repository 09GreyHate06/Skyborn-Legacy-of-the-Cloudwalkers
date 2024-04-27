using UnityEngine;
using SLOTC.Core.Combat;
using SLOTC.Core.Input;
using SLOTC.Core.Movement.Player;

namespace SLOTC.Core.States.Player
{
    public class JumpState : MoveableState
    {
        private readonly int _jumpAnimHash = Animator.StringToHash("Jump");
        private readonly PlayerInput _playerInput;
        private readonly TargetLocker _targetLocker;
        private readonly Animator _animator;
        private readonly float _jumpForce;
        private readonly float _animTransitionDuration;

        public JumpState(PlayerMover playerMover, PlayerInput playerInput, TargetLocker targetLocker, Animator animator, float animTransitionDuration, float moveSpeed, float rotationSpeed, float jumpForce)
            : base(playerMover, moveSpeed, rotationSpeed)
        {
            _playerInput = playerInput;
            _targetLocker = targetLocker;
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
            _jumpForce = jumpForce;
        }

        public override string GetID()
        {
            return GetType().ToString();
        }

        public override void OnEnter()
        {
            _playerMover.velocity.y = _jumpForce;
            _animator.CrossFadeInFixedTime(_jumpAnimHash, _animTransitionDuration);
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
            Vector2 inputAxis = _playerInput.Axis;
            if (_targetLocker.HasTarget)
                TargetLockedMove(inputAxis, inputAxis.magnitude, deltaTime);
            else
                FreeLookMove(inputAxis, inputAxis.magnitude, deltaTime);
        }
    }
}