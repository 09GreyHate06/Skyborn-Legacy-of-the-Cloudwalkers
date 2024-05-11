
using Animancer;
using SLOTC.Core.Movement.Enemy;
using SLOTC.Utils.StateMachine;
using UnityEngine;

namespace SLOTC.Core.States.Enemy
{
    public class FollowTargetState : IState
    {
        private readonly EnemyMover _enemyMover;
        private readonly Transform _target;
        private readonly AnimancerComponent _animancer;
        private readonly ClipTransition _moveAnim;

        private AnimancerState _curAnimState;

        public bool CanExit { get; set; }

        public FollowTargetState(EnemyMover enemyMover, Transform target, AnimancerComponent animancer, ClipTransition moveAnim)
        {
            _enemyMover = enemyMover;
            _target = target;
            _animancer = animancer;
            _moveAnim = moveAnim;
        }


        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            CanExit = true;
            if (_curAnimState == null || !_curAnimState.IsPlaying)
                _curAnimState = _animancer.Play(_moveAnim);
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
            _enemyMover.Destination = _target.position;
        }
    }
}