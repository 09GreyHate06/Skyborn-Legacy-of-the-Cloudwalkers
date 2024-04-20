using Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace SLOTC.Core.Combat
{
    public class TargetLocker : MonoBehaviour
    {
        [SerializeField] Animator _cmStateMachine;
        [SerializeField] CinemachineVirtualCamera _cmTargetLock;
        [SerializeField, Tooltip("Viewcone max angle in degrees")] float _maxAngle = 60.0f;

        private List<TargetLockable> _targets = new List<TargetLockable>();

        private int _freeLookStateHash = Animator.StringToHash("FreeLookState");
        private int _targetLockedStateHash = Animator.StringToHash("TargetLockedState");
        
        public TargetLockable CurrentTarget { get; private set; }
        public bool HasTarget { get { return CurrentTarget != null; } }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<TargetLockable>(out TargetLockable target))
                return;

            _targets.Add(target);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<TargetLockable>(out TargetLockable target))
                return;

            RemoveTarget(target);
        }

        public bool SelectTarget()
        {
            if (_targets.Count == 0)
                return false;

            TargetLockable closestTarget = null;
            float closestAngle = _maxAngle;
            float closestDistance = Mathf.Infinity;

            foreach(TargetLockable target in _targets)
            {
                Vector3 toTarget = target.transform.position - transform.position;
                float distance = toTarget.magnitude;
                float angle = Vector3.Angle(Camera.main.transform.forward, toTarget);

                if (angle > _maxAngle)
                    continue;

                if(distance + angle < closestDistance + closestAngle)
                {
                    closestDistance = distance;
                    closestAngle = angle;
                    closestTarget = target;
                }
            }

            if (closestTarget == null)
                return false;

            CurrentTarget = closestTarget;
            _cmTargetLock.LookAt = CurrentTarget.transform;
            _cmStateMachine.Play(_targetLockedStateHash);
            return true;
        }

        public void Cancel()
        {
            if (CurrentTarget == null)
                return;

            _cmTargetLock.LookAt = null;
            CurrentTarget = null;
            _cmStateMachine.Play(_freeLookStateHash);
        }

        private void RemoveTarget(TargetLockable target)
        {
            if(CurrentTarget == target)
            {
                Cancel();
            }

            target.OnDestroyed -= RemoveTarget;
            _targets.Remove(target);
        }
    }
}