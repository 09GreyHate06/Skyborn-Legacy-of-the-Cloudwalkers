using UnityEngine;
using System;

namespace SLOTC.Core.Combat.Animation
{
    [RequireComponent(typeof(Animator))]
    public class CombatAnimationEvent : MonoBehaviour
    {
        public enum Type
        {
            DodgeEnded = 0,
            ApplyForce,
            ActivateWeapon, // or fire projectile
            DeactivateWeapon,
            AnimationEnded,
            //ExitTime,
        }

        public event Action<Type> Listeners;

        private void OnCombatAnimationEvent(Type type)
        {
            Listeners?.Invoke(type);
        }
    }
}