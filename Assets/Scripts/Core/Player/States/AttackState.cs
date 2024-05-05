using UnityEngine;
using SLOTC.Core.Combat;
using System;
using SLOTC.Core.Input;
using SLOTC.Core.Movement.Player;
using Animancer;
using SLOTC.Core.Combat.Animation;

namespace SLOTC.Core.Player.States
{
    public class AttackState : MoveableState
    {
        private readonly PlayerInput _playerInput;
        private readonly AnimancerComponent _animancer;
        private readonly WeaponHandler _weaponHandler;
        private readonly SingleAttack[] _attacks;
        private readonly AnimancerEvent.Sequence[] _attacksAnimEvents;
        private readonly float _comboGraceTime;
        private int _comboCounter;
        private float _lastAttackTime = float.MinValue;

        private AudioSource _audioSource;
        private AudioClip _attackSFX;
        private SingleAttack _activeAttack;

        public override bool CanExit { get; set; }

        public event Action OnAnimationEnded;


        public AttackState(PlayerMover playerMover, PlayerInput playerInput, AnimancerComponent animancer, AudioSource audioSource, AudioClip attackSFX, WeaponHandler weaponHandler, SingleAttack[] attacks, float comboGraceTime, float rotationSpeed)
            : base(playerMover, 0.0f, rotationSpeed)
        {
            _playerInput = playerInput;
            _animancer = animancer;
            _audioSource = audioSource;
            _attackSFX = attackSFX;
            _weaponHandler = weaponHandler;
            _attacks = attacks;
            _comboGraceTime = comboGraceTime;

            _attacksAnimEvents = new AnimancerEvent.Sequence[attacks.Length];
            for (int i = 0; i < attacks.Length; i++)
            {
                var events = new AnimancerEvent.Sequence(_attacks[i].AttackAnim.Events);
                events.AddCallback(CombatAnimationEventNames.ApplyForce, ApplyForce);
                events.AddCallback(CombatAnimationEventNames.ActivateWeapon, ActivateWeapon);
                events.AddCallback(CombatAnimationEventNames.DeactivateWeapon, DeactivateWeapon);
                events.AddCallback(CombatAnimationEventNames.Exit, AnimationEnded);
                events.OnEnd = null;
                _attacksAnimEvents[i] = events;
            }
        }

        public override string GetID() 
        {
            return GetType().ToString();
        }

        public override void OnEnter()
        {
            //if(GetNormalizedTime(_animator, "ComboAttack") < -0.1f)
            //{
            //    Debug.Log("from idle");
            //}
            CanExit = false;

            _playerMover.velocity.x = 0.0f;
            _playerMover.velocity.z = 0.0f;
            float timeSinceLastAttack = Time.realtimeSinceStartup - _lastAttackTime;
            if (timeSinceLastAttack > _comboGraceTime)
                _comboCounter = 0;

            int comboIndex = _comboCounter++ % _attacks.Length;
            _activeAttack = _attacks[comboIndex];
            AnimancerState state = _animancer.Play(_activeAttack.AttackAnim);
            state.Events = _attacksAnimEvents[comboIndex];
        }

        public override void OnExit()
        {
            _activeAttack = null;
            //_audioSource.Stop();
        }

        public override void OnUpdate(float deltaTime)
        {
            FreeLookMove(_playerInput.Axis, deltaTime);
        }

        private void ApplyForce()
        {
            Vector3 right = _playerMover.transform.right * _activeAttack.Force.x;
            Vector3 up = _playerMover.transform.up * _activeAttack.Force.y;
            Vector3 forward = _playerMover.transform.forward * _activeAttack.Force.z;
            _playerMover.AddForce(right + up + forward);
        }

        private void ActivateWeapon()
        {
            _weaponHandler.Activate(_activeAttack);
            _audioSource.clip = _attackSFX;
            _audioSource.loop = false;
            _audioSource.Play();
        }

        private void DeactivateWeapon()
        {
            _lastAttackTime = Time.realtimeSinceStartup;
            _weaponHandler.Deactivate();
            CanExit = true;
        }

        private void AnimationEnded()
        {
            OnAnimationEnded?.Invoke();
        }
    }
}