using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Player.States;
using UnityEngine.InputSystem;
using SLOTC.Core.Combat;
using System;

namespace SLOTC.Core.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Locomotion Settings")]
        [SerializeField] float _moveSpeed = 3.0f;
        [SerializeField] float _rotationSpeed = 50.0f;
        [SerializeField] float _jumpForce = 5.0f;
        [SerializeField] float _fallingThreshold = -1.5f;
        [SerializeField] TargetLocker _targetLocker;

        [Space(10)]
        [Header("Animation Settings")]
        [SerializeField] float _toFreeLookAnimTransitonDuration = 0.1f;
        [SerializeField] float _toTargetLockedAnimTransitonDuration = 0.1f;
        [SerializeField] float _toJumpAnimTransitonDuration = 0.1f;
        [SerializeField] float _toFallingAnimTransitonDuration = 0.5f;
        [SerializeField] float _freeLookBlendTreeDampTime = 0.05f;
        [SerializeField] float _targetLockedBlendTreeDampTime = 0.05f;

        private StateMachine _stateMachine;
        private PlayerInput _playerInput;
        private PlayerMover _playerMover;

        private bool _jumpBtnPressed;

        private void Awake()
        {
            _playerInput = FindObjectOfType<PlayerInput>();
            _stateMachine = new StateMachine();
        }

        void Start()
        {
            Animator animator = GetComponent<Animator>();
            _playerMover = GetComponent<PlayerMover>();

            FreeLookState freeLookState = new FreeLookState(_playerMover, animator, _toFreeLookAnimTransitonDuration, _freeLookBlendTreeDampTime, _moveSpeed, _rotationSpeed);
            TargetLockedState targetLockState = new TargetLockedState(_playerMover, animator, _toTargetLockedAnimTransitonDuration, _targetLockedBlendTreeDampTime, _moveSpeed, _rotationSpeed);
            JumpState jumpState = new JumpState(_playerMover, animator, _toJumpAnimTransitonDuration, _jumpForce);
            FallingState fallingState = new FallingState(animator, _toFallingAnimTransitonDuration);

            // to FreeLookState
            _stateMachine.AddTransition(jumpState, freeLookState, () => { return _playerMover.IsGrounded; });
            _stateMachine.AddTransition(fallingState, freeLookState, () => { return _playerMover.IsGrounded; });
            _stateMachine.AddTransition(targetLockState, freeLookState, () => { return !_targetLocker.HasTarget; });

            //// to TargetLockState
            _stateMachine.AddTransition(freeLookState, targetLockState, () => { return _targetLocker.HasTarget; });

            //to JumpState
            _stateMachine.AddTransition(freeLookState, jumpState, () => { return _jumpBtnPressed && _playerMover.IsGrounded; });
            _stateMachine.AddTransition(targetLockState, jumpState, () => { return _jumpBtnPressed && _playerMover.IsGrounded; });


            // to FallingState
            Func<bool> isFalling = () => { return _playerMover.velocity.y < _fallingThreshold && !_playerMover.IsGrounded; };
            _stateMachine.AddTransition(freeLookState, fallingState, () => { return isFalling(); });
            _stateMachine.AddTransition(targetLockState, fallingState, () => { return isFalling(); });
            _stateMachine.AddTransition(jumpState, fallingState, () => { return isFalling(); });
            

            _stateMachine.SetState(freeLookState);
        }

        private void OnEnable()
        {
            _playerInput.OnMoveEvent += OnMove;
            _playerInput.OnJumpEvent += OnJump;
            _playerInput.OnAttackEvent += OnAttack;
            _playerInput.OnTargetEvent += OnTarget;
        }

        private void OnDisable()
        {
            _playerInput.OnMoveEvent -= OnMove;
            _playerInput.OnJumpEvent -= OnJump;
            _playerInput.OnAttackEvent -= OnAttack;
            _playerInput.OnTargetEvent -= OnTarget;
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

        private void OnMove(InputAction.CallbackContext context)
        {
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            _jumpBtnPressed = context.performed;
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
        }

        private void OnTarget(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;


            if (_targetLocker.HasTarget)
                _targetLocker.Cancel();
            else
                _targetLocker.SelectTarget();
        }

        private void OnApplicationFocus(bool focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}