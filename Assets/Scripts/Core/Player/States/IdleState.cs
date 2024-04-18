using UnityEngine;
using SLOTC.Utils.StateMachine;
using System;

namespace SLOTC.Core.Player.States
{
    public class IdleState : IState
    {
        private int _idleAnimHash = Animator.StringToHash("Idle");
        private readonly string _uniqueID = Guid.NewGuid().ToString();
        private PlayerMover _playerMover;
        private Animator _animator;
        private float _animTransitionDuration;

        public IdleState(PlayerMover playerMover, Animator animator, float animTransitionDuration)
        {
            _playerMover = playerMover;
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
        }

        public string GetID()
        {
            return _uniqueID;
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
        }
    }
}