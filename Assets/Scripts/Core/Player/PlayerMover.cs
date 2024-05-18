using SLOTC.Utils;
using SLOTC.Core.Saving;
using UnityEngine;

namespace SLOTC.Core.Movement.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMover : MonoBehaviour, IForceReceiver, ISaveable
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

            _force = Vector3.SmoothDamp(_force, Vector3.zero, ref _dampingVelocity, _forceSmoothDampTime);
            _controller.Move((velocity + _force) * Time.deltaTime);
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

        public object CaptureState()
        {
            SerializableVector3[] v = new SerializableVector3[]
            {
                new SerializableVector3(transform.position),
                new SerializableVector3(transform.localEulerAngles),
            };
            return v;
        }

        public void RestoreState(object state)
        {
            SerializableVector3[] v = (SerializableVector3[])state;

            transform.position = v[0].ToVector3();
            transform.localEulerAngles = v[1].ToVector3();
        }
    }
}