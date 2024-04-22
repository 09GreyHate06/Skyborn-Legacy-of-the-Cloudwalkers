using UnityEngine;
using SLOTC.Utils.StateMachine;

namespace SLOTC.Core.Player.States
{
    public class IdleState : IState
    {
        private readonly int _idleAnimHash = Animator.StringToHash("Idle");
        private readonly int _moveMagnitudeParamHash = Animator.StringToHash("MoveMagnitude");
        private readonly Animator _animator;
        private readonly float _animTransitionDuration;
        private readonly PlayerMover _playerMover;
        private readonly float _animDampTime;
        
        public IdleState(PlayerMover playerMover, Animator animator, float animTransitionDuration, float animDampTime)
        {
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
            _playerMover = playerMover;
            _animDampTime = animDampTime;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _playerMover.velocity = Vector3.zero;
            _animator.CrossFadeInFixedTime(_idleAnimHash, _animTransitionDuration);
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
            _animator.SetFloat(_moveMagnitudeParamHash, 0.0f, _animDampTime, deltaTime);
        }
    }
}