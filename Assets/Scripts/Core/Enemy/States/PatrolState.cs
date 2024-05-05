
using Animancer;
using SLOTC.Utils.StateMachine;
using UnityEngine;

namespace SLOTC.Core.Enemy.States
{
    public class PatrolState : IState
    {
        private readonly EnemyMover _enemyMover;
        private readonly Transform[] _patrolPoints;
        private readonly AnimancerComponent _animancer;
        private readonly ClipTransition _moveAnim;

        private AnimancerState _curAnimState;

        public bool CanExit { get; set; }

        public PatrolState(EnemyMover enemyMover, Transform[] patrolPoints, AnimancerComponent animancer, ClipTransition moveAnim)
        {
            _enemyMover = enemyMover;
            _patrolPoints = patrolPoints;
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