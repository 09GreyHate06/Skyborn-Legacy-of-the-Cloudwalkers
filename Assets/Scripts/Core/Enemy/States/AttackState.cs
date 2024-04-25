
using UnityEngine.AI;
using SLOTC.Utils.StateMachine;
using UnityEngine;
using SLOTC.Core.Combat;
using SLOTC.Core.Player;
using static DuloGames.UI.UIPlayAudio;
using System;

namespace SLOTC.Core.Enemy.States
{
    public class AttackState : IState
    {
        public enum EventType
        {
            Enter,
            AttackEnded,
            AnimationEnded,
            Exit
        }

        private readonly int _idleAnimHash = Animator.StringToHash("Idle");

        private readonly NavMeshAgent _navMeshAgent;
        private readonly Animator _animator;
        private readonly Transform _target;
        private readonly Attack[] _combo;
        private readonly float _comboGraceTime; 
        private readonly float _animTransitionDuration;

        private Attack _activeAttack;
        private int _comboCounter;
        private float _lastAttackTime = float.MinValue;
        private bool _applyForceFlag;
        private bool _eventAttackEndedCalled = false;
        private bool _eventAnimationEndedCalled = false;

        public event Action<EventType> OnEvent;

        public AttackState(NavMeshAgent navMeshAgent, Animator animator, Transform target, Attack[] combo, float comboGraceTime)
        {
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            _target = target;
            _combo = combo;
            _comboGraceTime = comboGraceTime;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            _navMeshAgent.ResetPath();
            //if (!_playerMover.IsGrounded)
            //{
            //    _playerMover.velocity.y = 5.0f;
            //}

            float timeSinceLastAttack = Time.realtimeSinceStartup - _lastAttackTime;
            if (timeSinceLastAttack > _comboGraceTime)
                _comboCounter = 0;

            int comboIndex = _comboCounter++ % _combo.Length;
            _activeAttack = _combo[comboIndex];
            _animator.CrossFade(_activeAttack.AnimNameHash, _activeAttack.AnimNormalizedTransitionDuration, 0, _activeAttack.AnimNormalizedTransitionOffset);

            _applyForceFlag = true;
            _eventAttackEndedCalled = false;
            _eventAnimationEndedCalled = false;
            OnEvent?.Invoke(EventType.Enter);
        }

        public void OnExit()
        {
            _applyForceFlag = false;
            _eventAttackEndedCalled = true;
            _eventAnimationEndedCalled = true;
            _activeAttack = null;
            OnEvent?.Invoke(EventType.Exit);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_activeAttack == null)
                return;

            Vector3 toTarget = _target.position - _navMeshAgent.transform.position;
            toTarget.y = 0.0f;
            Quaternion headingQuat = Quaternion.LookRotation(toTarget, Vector3.up);
            _navMeshAgent.transform.rotation = Quaternion.RotateTowards(_navMeshAgent.transform.rotation, headingQuat, _navMeshAgent.angularSpeed * deltaTime);

            float n = GetNormalizedTime();

            //if (_applyForceFlag && n >= _activeAttack.AnimNormalizedTimeToApplyForce)
            //{
            //    Vector3 right = _playerMover.transform.right * _activeAttack.Force.x;
            //    Vector3 up = _playerMover.transform.up * _activeAttack.Force.y;
            //    Vector3 forward = _playerMover.transform.forward * _activeAttack.Force.z;
            //    _playerMover.AddForce(right + up + forward);
            //    _applyForceFlag = false;
            //}

            if (!_eventAttackEndedCalled && n >= _activeAttack.AnimNormalizedExitTime)
            {
                OnEvent?.Invoke(EventType.AttackEnded);
                _lastAttackTime = Time.realtimeSinceStartup;
                _eventAttackEndedCalled = true;
            }

            if (!_eventAnimationEndedCalled && n >= 1.0f - float.Epsilon)
            {
                OnEvent?.Invoke(EventType.AnimationEnded);
                _eventAnimationEndedCalled = true;
            }
        }

        protected float GetNormalizedTime()
        {
            AnimatorStateInfo currentInfo = _animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextInfo = _animator.GetNextAnimatorStateInfo(0);

            if (currentInfo.shortNameHash == _activeAttack.AnimNameHash)
                return currentInfo.normalizedTime;
            else if (nextInfo.shortNameHash == _activeAttack.AnimNameHash)
                return nextInfo.normalizedTime;
            else
                return 0.0f;
        }
    }
}