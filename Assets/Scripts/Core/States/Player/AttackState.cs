using UnityEngine;
using SLOTC.Core.Combat;
using System;
using SLOTC.Core.Combat.Animation;
using SLOTC.Core.Input;
using SLOTC.Core.Movement.Player;

namespace SLOTC.Core.States.Player
{
    public class AttackState : MoveableState
    {
        public enum EventType
        {
            Enter,
            AttackEnded,
            AnimationEnded,
            Exit
        }

        private readonly PlayerInput _playerInput;
        private readonly Animator _animator;
        private readonly float _transitionDuration;
        private readonly CombatAnimationEvent _combatAnimationEvent;
        private readonly SingleAttack[] _combo;
        private readonly float _comboGraceTime;
        private int _comboCounter;
        private float _lastAttackTime = float.MinValue;


        private SingleAttack _activeAttack;

        public Action<EventType> OnEvent;


        public AttackState(PlayerMover playerMover, PlayerInput playerInput, Animator animator, float transitionDuration, SingleAttack[] combo, float comboGraceTime, float rotationSpeed)
            : base(playerMover, 0.0f, rotationSpeed)
        {
            _playerInput = playerInput;
            _animator = animator;
            _transitionDuration = transitionDuration;
            _combo = combo;
            _comboGraceTime = comboGraceTime;

            _combatAnimationEvent = _animator.GetComponent<CombatAnimationEvent>();
        }

        public override string GetID() 
        {
            return GetType().ToString();
        }

        public override void OnEnter()
        {
            _combatAnimationEvent.Listeners += OnCombatAnimEvent;
            //if(GetNormalizedTime(_animator, "ComboAttack") < -0.1f)
            //{
            //    Debug.Log("from idle");
            //}

            _playerMover.velocity.x = 0.0f;
            _playerMover.velocity.z = 0.0f;
            float timeSinceLastAttack = Time.realtimeSinceStartup - _lastAttackTime;
            if (timeSinceLastAttack > _comboGraceTime)
                _comboCounter = 0;

            int comboIndex = _comboCounter++ % _combo.Length;
            _activeAttack = _combo[comboIndex];
            _animator.CrossFadeInFixedTime(_activeAttack.AnimNameHash, 0.25f, 0);
            OnEvent?.Invoke(EventType.Enter);
        }

        public override void OnExit()
        {
            _combatAnimationEvent.Listeners -= OnCombatAnimEvent;
            _activeAttack = null;
            OnEvent?.Invoke(EventType.Exit);
        }

        public override void OnUpdate(float deltaTime)
        {
            Vector2 inputAxis = _playerInput.Axis;
            FreeLookMove(inputAxis, inputAxis.magnitude, deltaTime);
        }

        private void ApplyForce()
        {
            Vector3 right = _playerMover.transform.right * _activeAttack.Force.x;
            Vector3 up = _playerMover.transform.up * _activeAttack.Force.y;
            Vector3 forward = _playerMover.transform.forward * _activeAttack.Force.z;
            _playerMover.AddForce(right + up + forward);
        }

        private void OnCombatAnimEvent(CombatAnimationEvent.Type type)
        {
            switch (type)
            {
                case CombatAnimationEvent.Type.ApplyForce:
                    ApplyForce();
                    break;

                case CombatAnimationEvent.Type.ActivateWeapon:
                    break;

                case CombatAnimationEvent.Type.DeactivateWeapon:
                    _lastAttackTime = Time.realtimeSinceStartup;
                    OnEvent?.Invoke(EventType.AttackEnded);
                    break;

                case CombatAnimationEvent.Type.AnimationEnded:
                    OnEvent?.Invoke(EventType.AnimationEnded);
                    break;
            }
        }
    }
}