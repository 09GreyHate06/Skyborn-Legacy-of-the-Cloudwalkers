
using SLOTC.Core.Movement.Enemy;
using SLOTC.Utils.StateMachine;
using UnityEngine;

namespace SLOTC.Core.States.Enemy
{
    public class FollowTargetState : IState
    {
        private readonly int _moveAnimHash = Animator.StringToHash("Move");

        private readonly EnemyMover _enemyMover;
        private readonly Transform _target;
        private readonly Animator _animator;
        private readonly float _animTransitionDuration;

        public FollowTargetState(EnemyMover enemyMover, Transform target, Animator animator, float animTransitionDuration)
        {
            _enemyMover = enemyMover;
            _target = target;
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _animator.CrossFadeInFixedTime(_moveAnimHash, _animTransitionDuration);
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