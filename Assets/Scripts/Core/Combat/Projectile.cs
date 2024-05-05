using System;
using System.Linq;
using UnityEngine;


namespace SLOTC.Core.Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float _speed;
        [SerializeField] float _lifetime;
        [SerializeField] GameObject _impact;
        //[SerializeField] ParticleSystem _detachFX;

        private Rigidbody _rb;
        private Action<Collision> _onHit;

        public event Action<Collision> OnHit
        {
            add
            {
                if (_onHit == null || !_onHit.GetInvocationList().Contains(value))
                {
                    _onHit += value;
                }
            }
            remove
            {
                _onHit -= value;
            }
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _rb.AddForce(transform.forward * _speed, ForceMode.Impulse);
            Destroy(gameObject, _lifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            //if (_detachFX)
            //    _detachFX.transform.parent = null;
            if (_impact)
                Instantiate(_impact, transform.position, transform.rotation);

            _onHit?.Invoke(collision);
            Destroy(gameObject);
        }
    }

}
