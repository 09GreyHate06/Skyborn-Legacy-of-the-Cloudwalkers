using UnityEngine;

namespace SLOTC.Core.Movement.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMover : MonoBehaviour, IForceReceiver
    {
        [SerializeField] float _forceSmoothDampTime = 0.1f;
        [SerializeField] float _gravityMultiplier = 1.0f;
        [SerializeField] float _groundedYVelocity = -1.5f;

        [field: SerializeField] public bool UseGravity { get; set; } = true;


        private CharacterController _controller = null;
        private Vector3 _force;
        private Vector3 _dampingVelocity;

        public bool IsGrounded { get { return _controller.isGrounded; } }
        public Vector3 velocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            //if (Keyboard.current.spaceKey.wasPressedThisFrame)
            //{
            //    AddForce(new Vector3(0.0f, 0.0f, -20.0f));
            //}

            ApplyGravity();

            _controller.Move((velocity + _force) * Time.deltaTime);
            _force = Vector3.SmoothDamp(_force, Vector3.zero, ref _dampingVelocity, _forceSmoothDampTime);
        }

        public void AddForce(Vector3 force)
        {
            _force += force;
        }

        private void ApplyGravity()
        {
            if (!UseGravity)
                return;

            if (velocity.y < 0.0f && _controller.isGrounded)
                velocity.y = _groundedYVelocity;
            else
                velocity += Physics.gravity * _gravityMultiplier * Time.deltaTime;
        }
    }
}