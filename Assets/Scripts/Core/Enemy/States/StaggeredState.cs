using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Movement.Enemy;
using System;
using SLOTC.Core.Combat.Animation;
using Animancer;

namespace SLOTC.Core.Enemy.States
{
    public class StaggeredState : IState
    {
        private readonly EnemyMover _enemyMover;
        private readonly AnimancerComponent _animancer;
        private readonly ClipTransition _staggerAnim;
        private readonly AnimancerEvent.Sequence _staggerAnimEvents;

        public bool CanExit { get; set; }

        public event Action OnAnimationEnded;

        public StaggeredState(EnemyMover enemyMover, AnimancerComponent animancer, ClipTransition staggerAnim)
        {
            _enemyMover = enemyMover;
            _animancer = animancer;
            _staggerAnim = staggerAnim;

            _staggerAnimEvents = new AnimancerEvent.Sequence(_staggerAnim.Events);
            _staggerAnimEvents.SetCallback(CombatAnimationEventNames.StaggerEnded, () => CanExit = true);
            _staggerAnimEvents.SetCallback(CombatAnimationEventNames.Exit, () => OnAnimationEnded?.Invoke());
            _staggerAnimEvents.OnEnd = null;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            CanExit = false;

            _enemyMover.ResetPath();
            AnimancerState state = _animancer.Play(_staggerAnim);
            state.Events = _staggerAnimEvents;
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
        }
    }
}