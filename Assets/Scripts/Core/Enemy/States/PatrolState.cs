
using SLOTC.Utils.StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace SLOTC.Core.Enemy.States
{
    public class PatrolState : IState
    {
        private readonly int _moveAnimHash = Animator.StringToHash("Move");

        private readonly NavMeshAgent _navMeshAgent;
        private readonly Transform[] _patrolPoints;
        private readonly Animator _animator;
        private readonly float _animTransitionDuration;

        public PatrolState(NavMeshAgent navMeshAgent, Transform[] patrolPoints, Animator animator, float animTransitionDuration)
        {
            _navMeshAgent = navMeshAgent;
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
            _navMeshAgent.destination = _patrolPoints[r].position;
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
        }
    }
}