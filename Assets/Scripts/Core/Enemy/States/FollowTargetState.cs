
using SLOTC.Utils.StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace SLOTC.Core.Enemy.States
{
    public class FollowTargetState : IState
    {
        private readonly int _moveAnimHash = Animator.StringToHash("Move");

        private readonly NavMeshAgent _navMeshAgent;
        private readonly Transform _target;
        private readonly Animator _animator;
        private readonly float _animTransitionDuration;

        public FollowTargetState(NavMeshAgent navMeshAgent, Transform target, Animator animator, float animTransitionDuration)
        {
            _navMeshAgent = navMeshAgent;
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
            _navMeshAgent.destination = _target.position;
        }
    }
}