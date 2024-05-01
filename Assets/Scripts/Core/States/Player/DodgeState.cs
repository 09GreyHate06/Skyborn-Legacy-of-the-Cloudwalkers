using UnityEngine;
using SLOTC.Utils.StateMachine;
using System;
using SLOTC.Core.Combat;
using SLOTC.Core.Combat.Animation;
using SLOTC.Core.Input;
using SLOTC.Core.Movement.Player;
using Animancer;

namespace SLOTC.Core.States.Player
{
    public class DodgeState : IState
    {
        private readonly PlayerMover _playerMover;
        private readonly PlayerInput _playerInput;
        private readonly TargetLocker _targetLocker;
        private readonly AnimancerComponent _animancer;
        private readonly MixerTransition2D _dodgeAnim;
        private readonly AnimancerEvent.Sequence _dodgeAnimEvents;
        private readonly float _blendSpeed;
        private readonly float _dodgeForce;

        private Vector2 _inputXYParam;

        public bool CanExit { get; set; } = false;
        public event Action OnAnimationEnded;

        public DodgeState(PlayerMover playerMover, PlayerInput playerInput, TargetLocker targetLocker, AnimancerComponent animancer, MixerTransition2D dodgeAnim, float blendSpeed, float dodgeForce)
        {
            _playerMover = playerMover;
            _playerInput = playerInput;
            _targetLocker = targetLocker;
            _animancer = animancer;
            _dodgeAnim = dodgeAnim;
            _blendSpeed = blendSpeed;
            _dodgeForce = dodgeForce;

            _dodgeAnimEvents = new AnimancerEvent.Sequence(_dodgeAnim.Events);
            _dodgeAnimEvents.SetCallback(CombatAnimationEventNames.DodgeEnded, () => CanExit = true);
            _dodgeAnimEvents.SetCallback(CombatAnimationEventNames.Exit, () => OnAnimationEnded?.Invoke());
            _dodgeAnimEvents.OnEnd = null;
        }

        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            CanExit = false; 

            _playerMover.velocity.x = 0.0f;
            _playerMover.velocity.z = 0.0f;


            AnimancerState state = _animancer.Play(_dodgeAnim);
            state.Events = _dodgeAnimEvents;


            if (!_targetLocker.HasTarget || _playerInput.Axis.sqrMagnitude <= float.Epsilon)
            {
                _inputXYParam = new Vector2(0.0f, 1.0f);
                _playerMover.AddForce(_playerMover.transform.forward * _dodgeForce);
            }
            else
            {
                _inputXYParam = _playerInput.Axis;
                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0.0f;
                forward = forward.normalized;
                Vector3 right = Vector3.Cross(Vector3.up, forward);
                Vector3 heading = (right * _inputXYParam.x + forward * _inputXYParam.y).normalized;
                _playerMover.AddForce(heading * _dodgeForce);
            }
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
            _dodgeAnim.State.Parameter = Vector2.MoveTowards(_dodgeAnim.State.Parameter, _inputXYParam, _blendSpeed * Time.deltaTime);
        }
    }
}