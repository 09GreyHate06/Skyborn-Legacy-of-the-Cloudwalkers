using UnityEngine;
using UnityEngine.AI;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Enemy.States;
using System;
using SLOTC.Core.Combat;

namespace SLOTC.Core.Enemy
{
    public class MobEnemyController : MonoBehaviour
    {
        [Header("Combat Settings")]
        [SerializeField] GameObject _target;
        [SerializeField] Attack[] _combo;
        [SerializeField] float _comboGraceTime;
        [SerializeField] float _attackRate;
        [SerializeField] float _followTargetDistance;
        [SerializeField] float _viewConeAngle;
        [SerializeField] float _attackRange;

        [Space(10)]
        [Header("Locomotion Settings")]
        [SerializeField] NavMeshAgent _navMeshAgent;
        
        [Space(10)]
        [Header("Patrol Settings")] 
        [SerializeField] Transform[] _patrolPoints;
        [SerializeField] float _idleDurationBeforePatrol;

        [Header("Animation Settings")]
        [SerializeField] Animator _animator;
        [SerializeField] float _toIdleAnimTransitionDuration = 0.25f;
        [SerializeField] float _toMoveAnimTransitionDuration = 0.25f;

        private float _timeSinceIdle;

        private StateMachine _stateMachine;

        private float _timeSinceLastAttack = float.MaxValue;
        private bool _isAttacking;
        private bool _attackAnimationEnded = true;

        void Awake()
        {
            _stateMachine = new StateMachine();
            IdleState idleState = new IdleState(_navMeshAgent, _animator, _toIdleAnimTransitionDuration);
            PatrolState patrolState = new PatrolState(_navMeshAgent, _patrolPoints, _animator, _toMoveAnimTransitionDuration);
            FollowTargetState followTargetState = new FollowTargetState(_navMeshAgent, _target.transform, _animator, _toMoveAnimTransitionDuration);
            AttackState attackState = new AttackState(_navMeshAgent, _animator, _target.transform, _combo, _comboGraceTime);

            attackState.OnEvent += OnAttackStateEvent;

            void AT(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

            // to IdleState
            AT(patrolState, idleState, () => { return !_navMeshAgent.hasPath; });
            AT(followTargetState, idleState, () => { return Vector3.Distance(transform.position, _target.transform.position) > _followTargetDistance; });
            AT(attackState, idleState, () => { return _attackAnimationEnded && Vector3.Distance(transform.position, _target.transform.position) > _followTargetDistance; });

            // to PatrolState
            AT(idleState, patrolState, () => { return _timeSinceIdle >= _idleDurationBeforePatrol; });

            // to FollowTargetState
            bool toFollowStateBase() 
            {
                Vector3 toTarget = _target.transform.position - transform.position;
                return Vector3.Distance(transform.position, _target.transform.position) <= _followTargetDistance && Vector3.Angle(transform.forward, toTarget) <= _viewConeAngle * 0.5f;
            }
            AT(idleState, followTargetState, toFollowStateBase);
            AT(patrolState, followTargetState, toFollowStateBase);
            AT(attackState, followTargetState, () => 
            {
                float d = Vector3.Distance(transform.position, _target.transform.position);
                return !_isAttacking && d > _attackRange && d <= _followTargetDistance; 
            });

            // to AttackState
            _stateMachine.AddAnyTransition(attackState, true, () =>
            {
                return _timeSinceLastAttack >= _attackRate && Vector3.Distance(transform.position, _target.transform.position) <= _attackRange && !_isAttacking;
            });

            _stateMachine.SetState(idleState);
        }

        // Update is called once per frame
        void Update()
        {
            if (!_navMeshAgent.hasPath && !_isAttacking)
                _timeSinceIdle += Time.deltaTime;
            else
                _timeSinceIdle = 0.0f;

            if (!_isAttacking)
                _timeSinceLastAttack += Time.deltaTime;
            else
                _timeSinceLastAttack = 0.0f;

            _stateMachine.OnUpdate(Time.deltaTime);
        }

        private void OnAttackStateEvent(AttackState.EventType type)
        {
            switch (type)
            {
                case AttackState.EventType.Enter:
                    _isAttacking = true;
                    _attackAnimationEnded = false;
                    break;

                case AttackState.EventType.AttackEnded:
                    _isAttacking = false;
                    break;

                case AttackState.EventType.AnimationEnded:
                    _attackAnimationEnded = true;
                    break;

                case AttackState.EventType.Exit:
                    _isAttacking = false;
                    _attackAnimationEnded = true;
                    break;
            }
        }
    }
}