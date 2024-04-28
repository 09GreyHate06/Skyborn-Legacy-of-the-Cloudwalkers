using UnityEngine;
using UnityEngine.AI;

namespace SLOTC.Core.Movement.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CharacterController))]
    public class EnemyMover : MonoBehaviour, IForceReceiver
    {
        [SerializeField] float _forceSmoothDampTime = 0.1f;
        [SerializeField] float _gravityMultiplier = 1.0f;
        [field: SerializeField] public bool UseGravity { get; set; } = true;

        private NavMeshAgent _navMeshAgent;
        private CharacterController _controller;

        private Vector3 _force;
        private Vector3 _dampingVelocity;
        private float _yVelocity;

        public Vector3 Destination
        {
            get
            {
                return _navMeshAgent.destination;
            }
            set
            {
                if (_navMeshAgent.enabled)
                    _navMeshAgent.destination = value;
            }
        }

        public bool HasPath { get { return _navMeshAgent.enabled && _navMeshAgent.hasPath; } }
        public float AngularSpeed { get { return _navMeshAgent.angularSpeed; } }

        public bool IsGrounded { get { return _controller.isGrounded; } }

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (UseGravity)
                ApplyGravity();

            //if (_navMeshAgent.isOnOffMeshLink)
            //{
            //    _navMeshAgent.enabled = true;
            //    _controller.enabled = false;
            //    return;
            //}
            //else
            //{
            //    _controller.enabled = true;
            //}

            _navMeshAgent.enabled = _force.sqrMagnitude < 0.2f * 0.2f;
            Vector3 velocity = Vector3.zero;
            if (_navMeshAgent.enabled && _navMeshAgent.hasPath)
                velocity = _navMeshAgent.desiredVelocity;

            _force = Vector3.SmoothDamp(_force, Vector3.zero, ref _dampingVelocity, _forceSmoothDampTime);
            _controller.Move((velocity + (Vector3.up * _yVelocity) + _force) * Time.deltaTime);
        }

        public void ResetPath()
        {
            if (_navMeshAgent.enabled)
                _navMeshAgent.ResetPath();
        }

        public void AddForce(Vector3 force)
        {
            _force += force;
        }

        private void ApplyGravity()
        {
            if (_yVelocity < 0.0f && _controller.isGrounded)
                _yVelocity = -1.5f;
            else
                _yVelocity += UnityEngine.Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
        }
    }
}