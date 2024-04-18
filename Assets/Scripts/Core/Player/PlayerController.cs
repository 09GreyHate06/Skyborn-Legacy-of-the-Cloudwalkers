using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Player.States;
using UnityEngine.InputSystem;

namespace SLOTC.Core.Player
{
    [RequireComponent(typeof(PlayerMover))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float _moveSpeed = 3.0f;
        [SerializeField] float _rotationSpeed = 50.0f;
        [SerializeField] float _jumpForce = 5.0f;
        [SerializeField] float _fallingThreshold = -1.5f;

        private StateMachine _stateMachine;
        private PlayerInput _playerInput;
        private PlayerMover _playerMover;

        private bool _moveBtnPressed;
        private bool _jumpBtnPressed;

        private void Awake()
        {
            _stateMachine = new StateMachine();
            _playerInput = FindObjectOfType<PlayerInput>();
        }

        void Start()
        {
            Animator animator = GetComponent<Animator>();
            _playerMover = GetComponent<PlayerMover>();

            IdleState idleState = new IdleState(_playerMover, animator, 0.1f);
            MoveState moveState = new MoveState(_playerMover, animator, 0.1f, 0.05f, _moveSpeed, _rotationSpeed);
            JumpState jumpState = new JumpState(_playerMover, animator, 0.1f, _jumpForce);
            FallingState fallingState = new FallingState(animator, 0.5f);

            // to IdleState
            _stateMachine.AddTransition(moveState, idleState, () => { return !_moveBtnPressed && _playerMover.IsGrounded; });
            _stateMachine.AddTransition(jumpState, idleState, () => { return _playerMover.IsGrounded; });
            _stateMachine.AddTransition(fallingState, idleState, () => { return _playerMover.IsGrounded; });

            // to MoveState
            _stateMachine.AddTransition(idleState, moveState, () => { return _moveBtnPressed && _playerMover.IsGrounded; });


            //to JumpState
            _stateMachine.AddTransition(idleState, jumpState, () => { return _jumpBtnPressed && _playerMover.IsGrounded; });
            _stateMachine.AddTransition(moveState, jumpState, () => { return _jumpBtnPressed && _playerMover.IsGrounded; });


            // to FallingState
            _stateMachine.AddTransition(idleState, fallingState, () => { return IsFalling(); });
            _stateMachine.AddTransition(moveState, fallingState, () => { return IsFalling(); });
            _stateMachine.AddTransition(jumpState, fallingState, () => { return IsFalling(); });


            _stateMachine.SetState(idleState);
        }

        private void OnEnable()
        {
            _playerInput.OnMoveEvent += OnMove;
            _playerInput.OnJumpEvent += OnJump;
        }

        private void OnDisable()
        {
            _playerInput.OnMoveEvent -= OnMove;
            _playerInput.OnJumpEvent -= OnJump;
        }

        private void OnValidate()
        {
            Awake();
            Start();
        }


        void Update()
        {
            _stateMachine.OnUpdate(Time.deltaTime);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveBtnPressed = context.performed;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            _jumpBtnPressed = context.performed;
        }

        private bool IsFalling()
        {
            return _playerMover.velocity.y < _fallingThreshold && !_playerMover.IsGrounded;
        }
    }
}