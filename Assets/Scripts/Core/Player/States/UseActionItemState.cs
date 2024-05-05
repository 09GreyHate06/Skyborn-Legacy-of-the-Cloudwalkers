using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Movement.Player;
using Animancer;
using SLOTC.Core.Inventory;
using SLOTC.Core.Combat.Animation;
using SLOTC.Core.Stats;
using System;

namespace SLOTC.Core.Player.States
{
    public class UseActionItemState : IState
    {
        private readonly PlayerMover _playerMover;
        private readonly AnimancerComponent _animancer;
        private readonly ClipTransition _castAnim;
        private readonly ActionItemBar _actionItemBar;

        private AnimancerEvent.Sequence _animEvents;
        private int _actionItemBarSlotIndex;

        public event Action OnAnimationEnded;
        public bool CanExit { get; set; }

        public UseActionItemState(PlayerMover playerMover, AnimancerComponent animancer, ClipTransition castAnim, ActionItemBar actionItemBar)
        {
            _playerMover = playerMover;
            _animancer = animancer;
            _castAnim = castAnim;
            _actionItemBar = actionItemBar;

            _animEvents = new AnimancerEvent.Sequence(castAnim.Events);
            _animEvents.SetCallback(CombatAnimationEventNames.DoAction, Use);
            _animEvents.SetCallback(CombatAnimationEventNames.Exit, () => OnAnimationEnded?.Invoke());
            _animEvents.OnEnd = null;
        }

        public void SetActionBarSlotIndex(int index)
        {
            _actionItemBarSlotIndex = index;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            CanExit = false;

            if(_actionItemBar.GetActionItem(_actionItemBarSlotIndex) == null)
            {
                CanExit = true;
                OnAnimationEnded?.Invoke();
                return;
            }

            _playerMover.velocity.x = 0.0f;
            _playerMover.velocity.y = 0.0f;
            _animancer.Play(_castAnim).Events = _animEvents;
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
        }

        private void Use()
        {
            ActionItem a = _actionItemBar.Use(_actionItemBarSlotIndex);
            a.Use(_playerMover.GetComponent<Status>());
            CanExit = true;
        }
    }
}