using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Movement.Player;

namespace SLOTC.Core.Player.States
{
    public abstract class MoveableState : IState
    {
        protected readonly PlayerMover _playerMover;
        protected readonly float _moveSpeed;
        protected readonly float _rotationSpeed;

        public abstract bool CanExit { get; set; }

        public MoveableState(PlayerMover playerMover, float moveSpeed, float rotationSpeed)
        {
            _playerMover = playerMover;
            _moveSpeed = moveSpeed;
            _rotationSpeed = rotationSpeed;
        }

        public abstract string GetID();

        public abstract void OnEnter();

        public abstract void OnExit();

        public abstract void OnUpdate(float deltaTime);

        protected void FreeLookMove(Vector2 inputAxis, float deltaTime)
        {
            Vector3 velocity = Vector3.zero;
            if (inputAxis.sqrMagnitude > float.Epsilon)
            {
                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0.0f;
                forward = forward.normalized;
                Vector3 right = Camera.main.transform.right;
                Vector3 heading = (right * inputAxis.x + forward * inputAxis.y).normalized;

                Quaternion headingQuat = Quaternion.LookRotation(heading, Vector3.up);
                _playerMover.transform.rotation = Quaternion.RotateTowards(_playerMover.transform.rotation, headingQuat, _rotationSpeed * deltaTime);

                velocity = _moveSpeed * /*inputMagnitude **/ heading;
            }

            velocity.y = _playerMover.velocity.y;
            _playerMover.velocity = velocity;
        }

        protected void TargetLockedMove(Vector2 inputAxis, float deltaTime)
        {
            Vector3 velocity = Vector3.zero;
            if(inputAxis.sqrMagnitude > float.Epsilon)
            {
                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0.0f;
                forward = forward.normalized;

                Quaternion headingQuat = Quaternion.LookRotation(forward, Vector3.up);
                _playerMover.transform.rotation = Quaternion.RotateTowards(_playerMover.transform.rotation, headingQuat, _rotationSpeed * deltaTime);

                Vector3 right = Vector3.Cross(Vector3.up, forward);
                Vector3 heading = (right * inputAxis.x + forward * inputAxis.y).normalized;

                velocity = _moveSpeed * /*inputMagnitude **/ heading;
            }

            velocity.y = _playerMover.velocity.y;
            _playerMover.velocity = velocity;
        }
    }
}