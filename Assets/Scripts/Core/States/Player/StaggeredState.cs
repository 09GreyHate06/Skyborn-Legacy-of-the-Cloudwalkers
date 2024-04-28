using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Movement.Player;
using System;
using SLOTC.Core.Combat.Animation;

namespace SLOTC.Core.States.Player
{
    public class StaggeredState : IState
    {
        private readonly int _getHitAnimHash = Animator.StringToHash("GetHit");

        private readonly float _animTransitionDuration;
        private readonly PlayerMover _playerMover;
        private readonly Animator _animator;
        private readonly CombatAnimationEvent _combatAnimationEvent;
        private readonly float _animDampTime;

        public event Action OnStaggerEnd;
        
        public StaggeredState(PlayerMover playerMover, Animator animator, float animTransitionDuration, float animDampTime)
        {
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
            _playerMover = playerMover;
            _animDampTime = animDampTime;

            _combatAnimationEvent = _animator.GetComponent<CombatAnimationEvent>();
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _combatAnimationEvent.Listeners += OnCombatAnimEvent;
            _playerMover.velocity.x = 0.0f;
            _playerMover.velocity.z = 0.0f;
            _animator.CrossFadeInFixedTime(_getHitAnimHash, _animTransitionDuration);
        }

        public void OnExit()
        {
            _combatAnimationEvent.Listeners -= OnCombatAnimEvent;
        }

        public void OnUpdate(float deltaTime)
        {
        }

        private void OnCombatAnimEvent(CombatAnimationEvent.Type type)
        {
            if(type == CombatAnimationEvent.Type.AnimationEnded)
                OnStaggerEnd?.Invoke();
        }
    }
}