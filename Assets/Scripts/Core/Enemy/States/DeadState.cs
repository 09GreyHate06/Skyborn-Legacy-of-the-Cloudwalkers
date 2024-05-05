
using Animancer;
using SLOTC.Core.Combat.Animation;
using SLOTC.Core.Stats;
using SLOTC.Utils.StateMachine;
using UnityEngine;

namespace SLOTC.Core.Enemy.States
{
    public class DeadState : IState
    {
        private readonly EnemyMover _enemyMover;
        private readonly AnimancerComponent _animancer;
        private readonly ClipTransition _dieAnim;
        private readonly Reward _reward;

        public bool CanExit { get; set; }

        public DeadState(EnemyMover enemyMover, Reward reward, AnimancerComponent animancer, ClipTransition dieAnim)
        {
            _enemyMover = enemyMover;
            _animancer = animancer;
            _dieAnim = dieAnim;
            _reward = reward;
        }


        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            CanExit = false;
            _enemyMover.ResetPath();
            var state = _animancer.Play(_dieAnim);
            var playerInventory = Inventory.Inventory.GetPlayerInventory();
            var exp = playerInventory.GetComponent<Experience>();
            _reward.GrandReward(exp, playerInventory);
            state.Events.SetCallback(CombatAnimationEventNames.Exit, () => Object.Destroy(_enemyMover.gameObject, 1.5f));
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
        }
    }
}