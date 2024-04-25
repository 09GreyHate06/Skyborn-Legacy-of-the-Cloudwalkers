
using UnityEngine.AI;
using SLOTC.Utils.StateMachine;
using UnityEngine;

namespace SLOTC.Core.Enemy.States
{
    public class IdleState : IState
    {
        private readonly int _idleAnimHash = Animator.StringToHash("Idle");

        private readonly NavMeshAgent _navMeshAgent;
        private readonly Animator _animator;
        private readonly float _animTransitionDuration;

        public IdleState(NavMeshAgent navMeshAgent, Animator animator, float animTransitionDuration)
        {
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            _animTransitionDuration = animTransitionDuration;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _navMeshAgent.ResetPath();
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