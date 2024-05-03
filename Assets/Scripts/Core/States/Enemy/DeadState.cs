
using Animancer;
using SLOTC.Core.Combat.Animation;
using SLOTC.Core.Movement.Enemy;
using SLOTC.Utils.StateMachine;
using UnityEngine;

namespace SLOTC.Core.States.Enemy
{
    public class DeadState : IState
    {
        private readonly EnemyMover _enemyMover;
        private readonly AnimancerComponent _animancer;
        private readonly ClipTransition _dieAnim;

        public bool CanExit { get; set; }

        public DeadState(EnemyMover enemyMover, AnimancerComponent animancer, ClipTransition dieAnim)
        {
            _enemyMover = enemyMover;
            _animancer = animancer;
            _dieAnim = dieAnim;
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