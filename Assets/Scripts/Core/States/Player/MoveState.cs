using UnityEngine;
using SLOTC.Core.Combat;
using SLOTC.Core.Input;
using SLOTC.Core.Movement.Player;
using Animancer;

namespace SLOTC.Core.States.Player
{
    public class MoveState : MoveableState
    {
        private readonly PlayerInput _playerInput;
        private readonly AnimancerComponent _animancer;
        private readonly MixerTransition2D _moveAnim;
        private readonly TargetLocker _targetLocker;
        private readonly float _blendSpeed;

        //private MixerState<Vector2> _curAnimState;

        public override bool CanExit { get; set; }

        public MoveState(PlayerMover playerMover, PlayerInput playerInput, TargetLocker targetLocker, AnimancerComponent animancer, MixerTransition2D moveAnim, float blendSpeed, float moveSpeed, float rotationSpeed)
            : base(playerMover, moveSpeed, rotationSpeed)
        {
            _playerInput = playerInput;
            _targetLocker = targetLocker;
            _animancer = animancer;
            _moveAnim = moveAnim;
            _blendSpeed = blendSpeed;
        }


        public override string GetID()
        {
            return GetType().ToString();
        }

        public override void OnEnter()
        {
            CanExit = true;
            //if (_curAnimState == null || !_curAnimState.IsPlaying)
            //_curAnimState = (MixerState<Vector2>)_animancer.Play(_moveAnim);
            _animancer.Play(_moveAnim);
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
            if(!_targetLocker.HasTarget)
                FreeLook(deltaTime);
            else
                TargetLocked(deltaTime);
        }

        private void FreeLook(float deltaTime)
        {
            FreeLookMove(_playerInput.Axis, deltaTime);
            _moveAnim.State.Parameter = Vector2.MoveTowards(_moveAnim.State.Parameter, new Vector2(0.0f, 1.0f), _blendSpeed * Time.deltaTime);
        }

        private void TargetLocked(float deltaTime)
        {
            Vector2 inputAxis = _playerInput.Axis;
            TargetLockedMove(inputAxis, deltaTime);
            _moveAnim.State.Parameter = Vector2.MoveTowards(_moveAnim.State.Parameter, inputAxis, _blendSpeed * Time.deltaTime);
        }
    }
}