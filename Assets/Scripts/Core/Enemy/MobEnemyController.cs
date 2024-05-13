using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Enemy.States;
using System;
using SLOTC.Core.Combat;
using SLOTC.Core.Movement.Enemy;
using SLOTC.Utils;
using Animancer;
using System.Collections;
using SLOTC.Core.Combat.Animation;

namespace SLOTC.Core.Enemy
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
        [SerializeField] AnimancerComponent _animancer;
        [SerializeField] ClipTransition _idleAnim;
        [SerializeField] ClipTransition _moveAnim;
        [EventNames(CombatAnimationEventNames.StaggerEnded, CombatAnimationEventNames.Exit)]
        [SerializeField] ClipTransition _staggerAnim;
        [EventNames(CombatAnimationEventNames.Exit)]
        [SerializeField] ClipTransition _dieAnim;

        private StateMachine _stateMachine;

        private bool _attackAnimationEnded = true;
        private bool _staggerAnimationEnded = true;
        private bool _shouldStagger;

        private bool _canAttack = true;
        private bool _canPatrol = true;

        private void Awake()
        {
            if (_target == null)
                _target = GameObject.FindWithTag(Tags.Player).transform;
        }

        private void Start()
        {
            _stateMachine = new StateMachine();
            IdleState idleState = new IdleState(_enemyMover, _animancer, _idleAnim);
            PatrolState patrolState = new PatrolState(_enemyMover, _patrolPoints, _animancer, _moveAnim);
            FollowTargetState followTargetState = new FollowTargetState(_enemyMover, _target.transform, _animancer, _moveAnim);
            AttackState attackState = new AttackState(_enemyMover, _animancer, _target.transform, _weaponHandler, _combo, _comboGraceTime);
            StaggeredState staggeredState = new StaggeredState(_enemyMover, _animancer, _staggerAnim);
            DeadState deadState = new DeadState(_enemyMover, _animancer, _dieAnim);

            attackState.OnAnimationEnded += () => _attackAnimationEnded = true;
            staggeredState.OnAnimationEnded += () => _staggerAnimationEnded = true;

            void AT(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            float TargetDist() => Vector3.Distance(transform.position, _target.position);

            // to IdleState
            AT(patrolState, idleState, () => !_enemyMover.HasPath);
            AT(followTargetState, idleState, () => !_enemyMover.HasPath || (TargetDist() > _followTargetDistance) || (TargetDist() <= _attackRange && !_canAttack));
            AT(attackState, idleState, () => !_enemyMover.HasPath && _attackAnimationEnded && !_canAttack);
            AT(staggeredState, idleState, () => !_enemyMover.HasPath && _staggerAnimationEnded);

            // to PatrolState
            AT(idleState, patrolState, () => _canPatrol && TargetDist() > _followTargetDistance);

            // to FolloowTargetState
            bool FollowTargetStateBase()
            {
                Vector3 toTarget = _target.position - transform.position;
                float dist = toTarget.magnitude;
                float angleToTarget = Vector3.Angle(toTarget, transform.position);
                return dist <= _followTargetDistance && dist > _attackRange;
            }
            AT(idleState, followTargetState, FollowTargetStateBase);
            AT(patrolState, followTargetState, FollowTargetStateBase);
            AT(attackState, followTargetState, FollowTargetStateBase);
            AT(staggeredState, followTargetState, FollowTargetStateBase);

            // to AttackState
            _stateMachine.AddAnyTransition(attackState, true, () =>
            {
                string curStateID = _stateMachine.CurrentState.GetID();
                if (!_canAttack || TargetDist() > _attackRange || (!_stateMachine.CurrentState.CanExit && curStateID == staggeredState.GetID()) || curStateID == deadState.GetID())
                    return false;

                _stateMachine.CurrentState.CanExit = true;
                return true;
            });


            // StaggeredState
            {
                GetComponent<Knockbackable>().OnKnockback += (Knockbackable.KnockbackType knockbackType) =>
                {
                    string curStateID = _stateMachine.CurrentState.GetID();
                    if (knockbackType == Knockbackable.KnockbackType.Stagger && (curStateID != attackState.GetID() || _stateMachine.CurrentState.CanExit) && curStateID != deadState.GetID())
                        _shouldStagger = true;
                };
            
            
                _stateMachine.AddAnyTransition(staggeredState, false, () => _shouldStagger);
            }

            // DeadState
            _stateMachine.AddAnyTransition(deadState, false, () => GetComponent<HitPoint>().IsDead);

            _stateMachine.SetState(idleState);

            _stateMachine.OnBeforeChangeState += (IState nextState) =>
            {
                _shouldStagger = false;

                if (_stateMachine.CurrentState.GetID() == attackState.GetID())
                {
                    _attackAnimationEnded = true;
                }
                else if (_stateMachine.CurrentState.GetID() == staggeredState.GetID())
                {
                    _staggerAnimationEnded = true;
                }

                if (nextState.GetID() == attackState.GetID())
                {
                    StartCoroutine(AttackCooldown());
                    _attackAnimationEnded = false;
                }
                else if(nextState.GetID() == idleState.GetID())
                {
                    StartCoroutine(IdleBeforePatrol());
                }
                else if (nextState.GetID() == staggeredState.GetID())
                {
                    _staggerAnimationEnded = false;
                }
            };
        }

        // Update is called once per frame
        void Update()
        {
            _stateMachine.OnUpdate(Time.deltaTime);
        }

        private IEnumerator AttackCooldown()
        {
            _canAttack = false;
            yield return new WaitForSeconds(_attackRate);
            _canAttack = true;
        }

        private IEnumerator IdleBeforePatrol()
        {
            _canPatrol = false;
            yield return new WaitForSeconds(_idleDurationBeforePatrol);
            _canPatrol = true;
        }
    }
}