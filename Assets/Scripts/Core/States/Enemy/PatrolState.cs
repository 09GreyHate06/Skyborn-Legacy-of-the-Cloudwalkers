
using SLOTC.Core.Movement.Enemy;
using SLOTC.Utils.StateMachine;
using UnityEngine;

namespace SLOTC.Core.States.Enemy
{
    public class PatrolState : IState
    {
        private readonly int _moveAnimHash = Animator.StringToHash("Move");

        private readonly EnemyMover _enemyMover;
        private readonly Transform[] _patrolPoints;
        private readonly Animator _animator;
        private readonly float _animTransitionDuration;

        public PatrolState(EnemyMover enemyMover, Transform[] patrolPoints, Animator animator, float animTransitionDuration)
        {
            _enemyMover = enemyMover;
            _patrolPoints = patrolPoints;
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
            int r = Random.Range(0, _patrolPoints.Length);
            _enemyMover.Destination = _patrolPoints[r].position;
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
        }
    }
}