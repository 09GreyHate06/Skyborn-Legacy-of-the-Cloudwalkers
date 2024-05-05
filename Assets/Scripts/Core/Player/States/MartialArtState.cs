using UnityEngine;
using SLOTC.Core.Combat;
using System;
using SLOTC.Core.Input;
using SLOTC.Core.Movement.Player;
using Animancer;
using SLOTC.Core.Combat.Animation;

namespace SLOTC.Core.Player.States
{
    public class MartialArtState : MoveableState
    {
        private readonly PlayerInput _playerInput;
        private readonly AnimancerComponent _animancer;
        private readonly WeaponHandler _weaponHandler;
        private MartialArt _activeMartialArt;
        private AnimancerEvent.Sequence _martialArtAnimEvents;
        private int _attackInstanceCounter;

        public override bool CanExit { get; set; }

        public event Action OnAnimationEnded;


        public MartialArtState(PlayerMover playerMover, PlayerInput playerInput, AnimancerComponent animancer, WeaponHandler weaponHandler, float rotationSpeed)
            : base(playerMover, 0.0f, rotationSpeed)
        {
            _playerInput = playerInput;
            _animancer = animancer;
            _weaponHandler = weaponHandler;
        }

        public void SetMartialArt(MartialArt martialArt)
        {
            _activeMartialArt = martialArt;
            _martialArtAnimEvents = new AnimancerEvent.Sequence(_activeMartialArt.MartialArtAnim.Events);
            for(int i = 0; i < _activeMartialArt.AttackInstances.Length; i++)
            {
                int index = i;
                _martialArtAnimEvents.SetCallback(CombatAnimationEventNames.ApplyForce + index.ToString(), ApplyForce);
                _martialArtAnimEvents.SetCallback(CombatAnimationEventNames.ActivateWeapon + index.ToString(), ActivateWeapon);
                _martialArtAnimEvents.SetCallback(CombatAnimationEventNames.DeactivateWeapon + index.ToString(), DeactivateWeapon);
            }

            _martialArtAnimEvents.SetCallback(CombatAnimationEventNames.Exit, AnimationEnded);
            _martialArtAnimEvents.OnEnd = null;
        }

        public override string GetID()
        {
            return GetType().ToString();
        }

        public override void OnEnter()
        {
            CanExit = false;
            _playerMover.velocity.x = 0.0f;
            _playerMover.velocity.z = 0.0f;
            _attackInstanceCounter = 0;
            AnimancerState state = _animancer.Play(_activeMartialArt.MartialArtAnim);
            state.Events = _martialArtAnimEvents;
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
            FreeLookMove(_playerInput.Axis, deltaTime);
        }

        private void ApplyForce()
        {
            Attack curAttackInstance = _activeMartialArt.AttackInstances[_attackInstanceCounter];
            Vector3 right = _playerMover.transform.right * curAttackInstance.Force.x;
            Vector3 up = _playerMover.transform.up * curAttackInstance.Force.y;
            Vector3 forward = _playerMover.transform.forward * curAttackInstance.Force.z;
            _playerMover.AddForce(right + up + forward);
        }

        private void ActivateWeapon()
        {
            _weaponHandler.Activate(_activeMartialArt.AttackInstances[_attackInstanceCounter]);
        }

        private void DeactivateWeapon()
        {
            _weaponHandler.Deactivate();
            ++_attackInstanceCounter;
            if (_attackInstanceCounter >= _activeMartialArt.AttackInstances.Length)
                CanExit = true;
        }

        private void AnimationEnded()
        {
            OnAnimationEnded?.Invoke();
        }
    }
}