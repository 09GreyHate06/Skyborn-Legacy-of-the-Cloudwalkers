using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.States.Enemy;
using System;
using SLOTC.Core.Combat;
using SLOTC.Core.Movement.Enemy;
using System.ComponentModel;
using SLOTC.Utils;

namespace SLOTC.Core.Controller.Enemy
{
    public class MobEnemyController : MonoBehaviour
    {
        [Header("Combat Settings")]
        [SerializeField] Transform _target;
        [SerializeField] WeaponHandler _weaponHandler;
        [SerializeField] SingleAttack[] _combo;
        [SerializeField] float _comboGraceTime;
        [SerializeField] float _attackRate;
        [SerializeField] float _followTargetDistance;
        [SerializeField] float _attackRange;
        [SerializeField] float _targetLockedDuration;

        [Space(10)]
        [Header("Locomotion Settings")]
        [SerializeField] EnemyMover _enemyMover;
        [SerializeField] Transform[] _patrolPoints;
        [SerializeField] float _idleDurationBeforePatrol;

        [Header("Animation Settings")]
        [SerializeField] Animator _animator;
        [SerializeField] float _toIdleAnimTransitionDuration = 0.25f;
        [SerializeField] float _toMoveAnimTransitionDuration = 0.25f;
        [SerializeField] float _toAttackAnimTransitionDuration = 0.25f;
        [SerializeField] float _toStaggerAnimTransitionDuration = 0.25f;

        private float _timeSinceIdle;

        private StateMachine _stateMachine;

        private float _timeSinceLastAttack = float.MaxValue;
        private bool _isTargetLocked;
        private float _timeSinceTargetLocked;
        private bool _isAttacking;
        private bool _attackAnimationEnded = true;
        private bool _isStaggering;
        private bool _shouldStagger;

        private void Awake()
        {
            if (_target == null)
                _target = GameObject.FindWithTag(Tags.Player).transform;
        }

        private void Start()
        {
            _stateMachine = new StateMachine();
            IdleState idleState = new IdleState(_enemyMover, _animator, _toIdleAnimTransitionDuration);
            PatrolState patrolState = new PatrolState(_enemyMover, _patrolPoints, _animator, _toMoveAnimTransitionDuration);
            FollowTargetState followTargetState = new FollowTargetState(_enemyMover, _target.transform, _animator, _toMoveAnimTransitionDuration);
            AttackState attackState = new AttackState(_enemyMover, _animator, _toAttackAnimTransitionDuration, _target.transform, _weaponHandler, _combo, _comboGraceTime);

            attackState.OnEvent += OnAttackStateEvent;
            GetComponent<HitPoint>().OnTakeDamage += (_) => _isTargetLocked = true;

            void AT(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            float TargetDist() => Vector3.Distance(transform.position, _target.position);

            // to IdleState
            AT(patrolState, idleState, () => !_enemyMover.HasPath);
            AT(followTargetState, idleState, () => !_enemyMover.HasPath || (TargetDist() > _followTargetDistance && !_isTargetLocked) || (TargetDist() <= _attackRange && _timeSinceLastAttack < _attackRate));
            AT(attackState, idleState, () => !_enemyMover.HasPath && _attackAnimationEnded && _timeSinceLastAttack < _attackRate);

            // to PatrolState
            AT(idleState, patrolState, () => _timeSinceIdle >= _idleDurationBeforePatrol && TargetDist() > _followTargetDistance);

            // to FolloowTargetState
            bool FollowTargetStateBase()
            {
                Vector3 toTarget = _target.position - transform.position;
                float dist = toTarget.magnitude;
                float angleToTarget = Vector3.Angle(toTarget, transform.position);
                return (dist <= _followTargetDistance || _isTargetLocked) && dist > _attackRange;
            }
            AT(idleState, followTargetState, FollowTargetStateBase);
            AT(patrolState, followTargetState, FollowTargetStateBase);
            AT(attackState, followTargetState, () => FollowTargetStateBase() && !_isAttacking);

            // to AttackState
            _stateMachine.AddAnyTransition(attackState, true, () =>
            {
                return _timeSinceLastAttack >= _attackRate && TargetDist() <= _attackRange && !_isAttacking && !_isStaggering;
            });

            // StaggeredState
            if (TryGetComponent(out Knockbackable knockbackable))
            {
                knockbackable.OnKnockback += (Knockbackable.KnockbackType knockbackType) =>
                {
                    if (knockbackType == Knockbackable.KnockbackType.Stagger && !_isAttacking)
                        _shouldStagger = true;
                };

                StaggeredState staggeredState = new StaggeredState(_enemyMover, _animator, _toStaggerAnimTransitionDuration);
                staggeredState.OnEvent += OnStaggerStateEvent;

                _stateMachine.AddAnyTransition(staggeredState, false, () => _shouldStagger);

                // transitions
                AT(staggeredState, idleState, () => !_enemyMover.HasPath && !_isStaggering);
                AT(staggeredState, followTargetState, () => FollowTargetStateBase() && !_isStaggering);
            }

/*            // to IdleState
            {
                AT(patrolState, idleState, () => !_enemyMover.HasPath);
                AT(followTargetState, idleState, () => Vector3.Distance(transform.position, _target.transform.position) > _followTargetDistance);
                AT(attackState, idleState, () => _attackAnimationEnded);
            }

            // to FollowTargetState
            bool toFollowStateBase() 
            {
                Vector3 toTarget = _target.transform.position - transform.position;
                float dist = toTarget.magnitude;
                float angleToTarget = Vector3.Angle(transform.forward, toTarget);
                float hViewConeAngle = _viewConeAngle * 0.5f;
                return (dist > _attackRange && dist <= _followTargetDistance && angleToTarget <= hViewConeAngle) || _isTargetLocked;
            }
            {
                AT(idleState, followTargetState, toFollowStateBase);
                AT(patrolState, followTargetState, toFollowStateBase);
                AT(attackState, followTargetState, () => toFollowStateBase() && !_isAttacking);
            }


            // to PatrolState
            AT(idleState, patrolState, () => _timeSinceIdle >= _idleDurationBeforePatrol && !_isTargetLocked);


            // to AttackState
            _stateMachine.AddAnyTransition(attackState, true, () =>
            {
                return _timeSinceLastAttack >= _attackRate && Vector3.Distance(transform.position, _target.transform.position) <= _attackRange && !_isAttacking && !_isStaggering;
            });

            // StaggeredState
            if (TryGetComponent(out Knockbackable knockbackable))
            {
                knockbackable.OnKnockback += (Knockbackable.KnockbackType knockbackType) =>
                {
                    if (knockbackType == Knockbackable.KnockbackType.Stagger && !_isAttacking)
                        _shouldStagger = true;
                };

                StaggeredState staggeredState = new StaggeredState(_enemyMover, _animator, _toStaggerAnimTransitionDuration);
                staggeredState.OnEvent += OnStaggerStateEvent;

                _stateMachine.AddAnyTransition(staggeredState, false, () => _shouldStagger);

                AT(staggeredState, idleState, () => !_enemyMover.HasPath && !_isStaggering);
                AT(staggeredState, followTargetState, () => toFollowStateBase() && !_isStaggering);
            }*/

            _stateMachine.SetState(idleState);
        }

        // Update is called once per frame
        void Update()
        {
            _stateMachine.OnUpdate(Time.deltaTime);


            if (!_enemyMover.HasPath && !_isAttacking)
                _timeSinceIdle += Time.deltaTime;
            else
                _timeSinceIdle = 0.0f;

            if (!_isAttacking)
                _timeSinceLastAttack += Time.deltaTime;
            else
                _timeSinceLastAttack = 0.0f;

            if (_isTargetLocked)
                _timeSinceTargetLocked += Time.deltaTime;
            else
                _timeSinceTargetLocked = 0.0f;

            if (_timeSinceTargetLocked >= _targetLockedDuration)
                _isTargetLocked = false;

            _shouldStagger = false;
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

        private void OnStaggerStateEvent(StaggeredState.EventType type)
        {
            switch (type)
            {
                case StaggeredState.EventType.Enter:
                    _isStaggering = true;
                    break;
                case StaggeredState.EventType.StaggerEnded:
                    _isStaggering = false;
                    break;
                case StaggeredState.EventType.Exit:
                    _isStaggering = false;
                    break;
            }
        }
    }
}