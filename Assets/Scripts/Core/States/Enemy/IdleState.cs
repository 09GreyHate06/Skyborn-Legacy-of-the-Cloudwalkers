
using Animancer;
using SLOTC.Core.Movement.Enemy;
using SLOTC.Utils.StateMachine;
using UnityEngine;

namespace SLOTC.Core.States.Enemy
{
    public class IdleState : IState
    {
        private readonly EnemyMover _enemyMover;
        private readonly AnimancerComponent _animancer;
        private readonly ClipTransition _idleAnim;

        private AnimancerState _curAnimState;
        
        public bool CanExit { get; set; }
        
        public IdleState(EnemyMover enemyMover, AnimancerComponent animancer, ClipTransition idleAnim)
        {
            _enemyMover = enemyMover;
            _animancer = animancer;
            _idleAnim = idleAnim;
        }


        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            CanExit = true;
            _enemyMover.ResetPath();
            if(_curAnimState == null || !_curAnimState.IsPlaying)
                _curAnimState = _animancer.Play(_idleAnim);
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
        }
    }
}