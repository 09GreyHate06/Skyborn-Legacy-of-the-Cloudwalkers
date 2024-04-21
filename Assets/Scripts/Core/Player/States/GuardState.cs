using UnityEngine;
using SLOTC.Utils.StateMachine;

namespace SLOTC.Core.Player.States
{
    public class GuardState : IState
    {
        private int _guardAnimHash = Animator.StringToHash("Guard");
        private Animator _animator;
        private float _animTransitionDuration;
        private PlayerMover _playerMover;

        public GuardState(PlayerMover playerMover, Animator animator, float animTransitionDuration)
        {
            _playerMover = playerMover;
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _playerMover.velocity.x = 0.0f;
            _playerMover.velocity.z = 0.0f;
            _animator.CrossFadeInFixedTime(_guardAnimHash, _animTransitionDuration);
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
        }
    }
}