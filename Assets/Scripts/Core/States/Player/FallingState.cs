using UnityEngine;
using SLOTC.Core.Combat;
using SLOTC.Core.Input;
using SLOTC.Core.Movement.Player;

namespace SLOTC.Core.States.Player
{
    public class FallingState : MoveableState
    {
        private readonly int _fallingAnimHash = Animator.StringToHash("Falling");
        private readonly PlayerInput _playerInput;
        private readonly TargetLocker _targetLocker;
        private readonly Animator _animator;
        private readonly float _animTransitionDuration;

        public FallingState(PlayerMover playerMover, PlayerInput playerInput, TargetLocker targetLocker, Animator animator, float animTransitionDuration, float moveSpeed, float rotationSpeed)
            : base(playerMover, moveSpeed, rotationSpeed)
        {
            _playerInput = playerInput;
            _targetLocker = targetLocker;
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
        }

        public override string GetID()
        {
            return GetType().ToString();
        }

        public override void OnEnter()
        {
            _animator.CrossFadeInFixedTime(_fallingAnimHash, _animTransitionDuration);
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