using UnityEngine;
using UnityEngine.InputSystem;

namespace SLOTC.Core.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] float _forceSmoothDampTime = 0.3f;
        [SerializeField] float _gravityMultiplier = 1.0f;

        public CharacterController _controller;

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

            if (velocity.y < 0.0f && _controller.isGrounded)
                velocity.y = Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
            else
                velocity += Physics.gravity * _gravityMultiplier * Time.deltaTime;

            _force = Vector3.SmoothDamp(_force, Vector3.zero, ref _dampingVelocity, _forceSmoothDampTime);
            _controller.Move((velocity + _force) * Time.deltaTime);
        }

        public void AddForce(Vector3 force)
        {
            _force += force;
        }
    }
}