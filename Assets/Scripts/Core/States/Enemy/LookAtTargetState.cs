
using SLOTC.Core.Movement.Enemy;
using SLOTC.Utils.StateMachine;
using UnityEngine;

namespace SLOTC.Core.States.Enemy
{
    public class LookAtTargetState : IState
    {
        private readonly EnemyMover _enemyMover;
        private readonly Transform _target;

        public bool CanExit { get; set; }

        public LookAtTargetState(EnemyMover enemyMover, Transform target)
        {
            _enemyMover = enemyMover;
            _target = target;
        }


        public string GetID()
        {
            return GetType().ToString();
        }

        public void OnEnter()
        {
            CanExit = true;
            _enemyMover.ResetPath();
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
            Vector3 toTarget = _target.position - _enemyMover.transform.position;
            toTarget.y = 0.0f;
            Quaternion headingQuat = Quaternion.LookRotation(toTarget, Vector3.up);
            _enemyMover.transform.rotation = Quaternion.RotateTowards(_enemyMover.transform.rotation, headingQuat, _enemyMover.AngularSpeed * deltaTime);
        }
    }
}