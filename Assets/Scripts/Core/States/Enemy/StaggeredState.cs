using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Movement.Enemy;
using System;
using SLOTC.Core.Combat.Animation;

namespace SLOTC.Core.States.Enemy
{
    public class StaggeredState : IState
    {
        public enum EventType
        {
            Enter,
            StaggerEnded,
            Exit,
        }

        private readonly int _staggerAnimHash = Animator.StringToHash("Stagger");

        private readonly float _animTransitionDuration;
        private readonly EnemyMover _enemyMover;
        private readonly Animator _animator;
        private readonly CombatAnimationEvent _combatAnimationEvent;

        public event Action<EventType> OnEvent;

        public StaggeredState(EnemyMover playerMover, Animator animator, float animTransitionDuration)
        {
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
            _enemyMover = playerMover;

            _combatAnimationEvent = _animator.GetComponent<CombatAnimationEvent>();
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _combatAnimationEvent.Listeners += OnCombatAnimEvent;
            _enemyMover.ResetPath();
            _animator.CrossFadeInFixedTime(_staggerAnimHash, _animTransitionDuration);
            OnEvent?.Invoke(EventType.Enter);
        }

        public void OnExit()
        {
            _combatAnimationEvent.Listeners -= OnCombatAnimEvent;
            OnEvent?.Invoke(EventType.Exit);
        }

        public void OnUpdate(float deltaTime)
        {
        }

        private void OnCombatAnimEvent(CombatAnimationEvent.Type type)
        {
            if (type == CombatAnimationEvent.Type.ExitTime)
                OnEvent?.Invoke(EventType.StaggerEnded);
        }
    }
}