
using SLOTC.Core.Movement.Enemy;
using SLOTC.Utils.StateMachine;
using UnityEngine;

namespace SLOTC.Core.States.Enemy
{
    public class IdleState : IState
    {
        private readonly int _idleAnimHash = Animator.StringToHash("Idle");

        private readonly EnemyMover _enemyMover;
        private readonly Animator _animator;
        private readonly float _animTransitionDuration;

        public IdleState(EnemyMover enemyMover, Animator animator, float animTransitionDuration)
        {
            _enemyMover = enemyMover;
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _enemyMover.ResetPath();
            _animator.CrossFadeInFixedTime(_idleAnimHash, _animTransitionDuration);
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
        }
    }
}