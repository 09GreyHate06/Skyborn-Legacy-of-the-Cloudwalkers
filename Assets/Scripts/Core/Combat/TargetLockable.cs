using UnityEngine;
using System;

namespace SLOTC.Core.Combat
{
    public class TargetLockable : MonoBehaviour
    {
        public event Action<TargetLockable> OnDestroyed;

        private void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }
    }
}