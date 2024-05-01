using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Movement.Player;
using Animancer;

namespace SLOTC.Core.States.Player
{
    public class IdleState : IState
    {
        private readonly PlayerMover _playerMover;
        private readonly AnimancerComponent _animancer;
        private readonly MixerTransition2D _moveAnim;
        private readonly float _blendSpeed;

        //private MixerState<Vector2> _curAnimState;

        public bool CanExit { get; set; }

        public IdleState(PlayerMover playerMover, AnimancerComponent animancer, MixerTransition2D moveAnim, float blendSpeed)
        {
            _playerMover = playerMover;
            _animancer = animancer;
            _moveAnim = moveAnim;
            _blendSpeed = blendSpeed;
        }


        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            CanExit = true;

            _playerMover.velocity = Vector3.zero;

            //if (_curAnimState == null || !_curAnimState.IsPlaying)
            //    _curAnimState = (MixerState<Vector2>)_animancer.Play(_moveAnim);

            _animancer.Play(_moveAnim);
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
            _moveAnim.State.Parameter = Vector2.MoveTowards(_moveAnim.State.Parameter, Vector2.zero, _blendSpeed * Time.deltaTime);
        }
    }
}