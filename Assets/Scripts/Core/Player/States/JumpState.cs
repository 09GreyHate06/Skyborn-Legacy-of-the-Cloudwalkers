using UnityEngine;
using SLOTC.Core.Combat;
using SLOTC.Core.Input;
using SLOTC.Core.Movement.Player;
using Animancer;

namespace SLOTC.Core.Player.States
{
    public class JumpState : MoveableState
    {
        private readonly PlayerInput _playerInput;
        private readonly TargetLocker _targetLocker;
        private readonly AnimancerComponent _animancer;
        private readonly ClipTransition _jumpAnim;
        private readonly float _jumpForce;

        private AudioSource _audioSource;
        private AudioClip _jumpSFX;
        public override bool CanExit { get; set; }

        public JumpState(PlayerMover playerMover, PlayerInput playerInput, TargetLocker targetLocker, AnimancerComponent animancer, ClipTransition jumpAnim, AudioSource audioSource, AudioClip jumpSFX, float moveSpeed, float rotationSpeed, float jumpForce)
            : base(playerMover, moveSpeed, rotationSpeed)
        {
            _playerInput = playerInput;
            _targetLocker = targetLocker;
            _animancer = animancer;
            _audioSource = audioSource;
            _jumpSFX = jumpSFX;
            _jumpAnim = jumpAnim;
            _jumpForce = jumpForce;
        }


        public override string GetID()
        {
            return GetType().ToString();
        }

        public override void OnEnter()
        {
            CanExit = true;
            _playerMover.velocity.y = _jumpForce;

            _animancer.Play(_jumpAnim);
            _audioSource.clip = _jumpSFX;
            _audioSource.loop = false;
            _audioSource.Play();
        }

        public override void OnExit()
        {
            _audioSource.Stop();
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