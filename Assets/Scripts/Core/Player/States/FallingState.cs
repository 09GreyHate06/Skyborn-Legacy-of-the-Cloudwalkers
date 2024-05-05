using UnityEngine;
using SLOTC.Core.Combat;
using SLOTC.Core.Input;
using SLOTC.Core.Movement.Player;
using Animancer;

namespace SLOTC.Core.Player.States
{
    public class FallingState : MoveableState
    {
        private readonly PlayerInput _playerInput;
        private readonly TargetLocker _targetLocker;
        private readonly AnimancerComponent _animancer;
        private readonly ClipTransition _fallingAnim;

        public FallingState(PlayerMover playerMover, PlayerInput playerInput, TargetLocker targetLocker, AnimancerComponent animancer, ClipTransition fallingAnim, float moveSpeed, float rotationSpeed)
            : base(playerMover, moveSpeed, rotationSpeed)
        {
            _playerInput = playerInput;
            _targetLocker = targetLocker;
            _animancer = animancer;
            _fallingAnim = fallingAnim;
        }

        public override bool CanExit { get; set; } = true;

        public override string GetID()
        {
            return GetType().ToString();
        }

        public override void OnEnter()
        {
            CanExit = true;

            _animancer.Play(_fallingAnim);
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
            if (_targetLocker.HasTarget)
                TargetLockedMove(_playerInput.Axis, deltaTime);
            else
                FreeLookMove(_playerInput.Axis, deltaTime);
        }
    }
}