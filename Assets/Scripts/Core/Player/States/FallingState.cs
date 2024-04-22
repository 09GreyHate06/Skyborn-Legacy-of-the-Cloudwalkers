using UnityEngine;
using SLOTC.Utils.StateMachine;

namespace SLOTC.Core.Player.States
{
    public class FallingState : IState
    {
        private readonly int _fallingAnimHash = Animator.StringToHash("Falling");
        private readonly Animator _animator;
        private readonly float _animTransitionDuration;

        public FallingState(Animator animator, float animTransitionDuration)
        {
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _animator.CrossFadeInFixedTime(_fallingAnimHash, _animTransitionDuration);
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
        }
    }
}