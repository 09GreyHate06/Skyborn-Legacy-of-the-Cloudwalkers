using UnityEngine;
using System;
using SLOTC.Core.Movement;

namespace SLOTC.Core.Combat
{
    [RequireComponent(typeof(IForceReceiver))]
    public class Knockbackable : MonoBehaviour
    {
        [SerializeField, Tooltip("forceDir * (forceMagnitude - forceResist)")] float _forceResistance;
        //[SerializeField] float _minForceMagnitudeToGetKnockdown;
        [SerializeField] float _minForceMagnitudeToGetStagger;

        public enum KnockbackType
        {
            ForceOnly,
            Stagger,
            //Knockdown,
        }

        private IForceReceiver _forceReceiver;

        //public event Action<Vector3 /*dir*/> OnKnockback;
        public event Action<KnockbackType> OnKnockback;

        private void Awake()
        {
            _forceReceiver = GetComponent<IForceReceiver>();
        }

        public void Knockback(Vector3 forceDir, float forceMagnitude)
        {
            float magnitude = Mathf.Max(0.0f, forceMagnitude - _forceResistance);
            Vector3 force = forceDir * magnitude;
            _forceReceiver.AddForce(force);
            //OnKnockback?.Invoke(transform.InverseTransformDirection(forceWithRespectToCaller.normalized));

            KnockbackType knockbackType = KnockbackType.ForceOnly;
            if (magnitude >= _minForceMagnitudeToGetStagger)
                knockbackType = KnockbackType.Stagger;

            //if (magnitude >= _minForceMagnitudeToGetKnockdown)
            //    knockbackType = KnockbackType.Knockdown;

            OnKnockback?.Invoke(knockbackType);
        }


    }
}