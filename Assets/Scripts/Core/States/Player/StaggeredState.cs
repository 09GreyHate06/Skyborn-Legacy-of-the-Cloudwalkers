using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Movement.Player;
using System;
using SLOTC.Core.Combat.Animation;
using Animancer;

namespace SLOTC.Core.States.Player
{
    public class StaggeredState : IState
    {
        private readonly PlayerMover _playerMover;
        private readonly AnimancerComponent _animancer;
        private readonly ClipTransition _staggerAnim;
        private readonly AnimancerEvent.Sequence _staggerAnimEvents;

        public bool CanExit { get; set; }

        public event Action OnAnimationEnded;
        
        public StaggeredState(PlayerMover playerMover, AnimancerComponent animancer, ClipTransition staggerAnim)
        {
            _animancer = animancer;
            _staggerAnim = staggerAnim;
            _playerMover = playerMover;

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
            _playerMover.velocity.x = 0.0f;
            _playerMover.velocity.z = 0.0f;

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