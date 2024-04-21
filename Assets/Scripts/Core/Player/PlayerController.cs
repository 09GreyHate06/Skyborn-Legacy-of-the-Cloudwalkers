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
        [Header("Temp Variables")]
        [SerializeField] Attack[] _combo;
        [SerializeField] float _comboGraceTime;

        [Space(10)]
        [Header("Locomotion Settings")]
        [SerializeField] float _moveSpeed = 3.0f;
        [SerializeField] float _rotationSpeed = 50.0f;
        [SerializeField] float _jumpForce = 5.0f;
        [SerializeField] float _fallingThreshold = -1.5f;
        [SerializeField] TargetLocker _targetLocker;

        [Space(10)]
        [Header("Animation Settings")]
        [SerializeField] float _toFreeLookAnimTransitonDuration = 0.25f;
        [SerializeField] float _toTargetLockedAnimTransitonDuration = 0.25f;
        [SerializeField] float _toJumpAnimTransitonDuration = 0.25f;
        [SerializeField] float _toGuardAnimTransionDuration = 0.25f;
        [SerializeField] float _toFallingAnimTransitonDuration = 0.25f;
        [SerializeField] float _freeLookBlendTreeDampTime = 0.05f;
        [SerializeField] float _targetLockedBlendTreeDampTime = 0.05f;

        private StateMachine _stateMachine;
        private PlayerInput _playerInput;
        private PlayerMover _playerMover;

        private bool _jumpBtnPressed;
        private bool _isAttackBtnPressed;
        private bool _guardBtnPressed;
        private bool _isAttacking = false;

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
            AttackState attackState = new AttackState(_playerMover, animator, _combo, _comboGraceTime);
            GuardState guardState = new GuardState(_playerMover, animator, _toGuardAnimTransionDuration);

            attackState.OnAttackFinished += () => { _isAttacking = false; };

            // to FreeLook
            _stateMachine.AddTransition(jumpState, freeLookState, () => { return _playerMover.IsGrounded; });
            _stateMachine.AddTransition(fallingState, freeLookState, () => { return _playerMover.IsGrounded; });
            _stateMachine.AddTransition(attackState, freeLookState, () => { return _playerMover.IsGrounded && !_isAttacking; });
            _stateMachine.AddTransition(guardState, freeLookState, () => { return _playerMover.IsGrounded && !_guardBtnPressed; });
            _stateMachine.AddTransition(targetLockState, freeLookState, () => { return _playerMover.IsGrounded && !_targetLocker.HasTarget; });

            // to TargetLockState
            _stateMachine.AddTransition(freeLookState, targetLockState, () => { return _playerMover.IsGrounded && _targetLocker.HasTarget; });

            // to JumpState
            _stateMachine.AddTransition(freeLookState, jumpState, () => { return _playerMover.IsGrounded && _jumpBtnPressed; });
            _stateMachine.AddTransition(targetLockState, jumpState, () => { return _playerMover.IsGrounded && _jumpBtnPressed; });
            _stateMachine.AddTransition(fallingState, jumpState, () => { return _playerMover.IsGrounded && _jumpBtnPressed; });
            _stateMachine.AddTransition(attackState, jumpState, () => { return _playerMover.IsGrounded && _jumpBtnPressed && !_isAttacking; });
            _stateMachine.AddTransition(guardState, jumpState, () => { return _playerMover.IsGrounded && _jumpBtnPressed && !_guardBtnPressed; });

            // to FallingState
            Func<bool> isFalling = () => { return _playerMover.velocity.y < _fallingThreshold && !_playerMover.IsGrounded; };
            _stateMachine.AddTransition(freeLookState, fallingState, () => isFalling());
            _stateMachine.AddTransition(targetLockState, fallingState, () => isFalling());
            _stateMachine.AddTransition(jumpState, fallingState, () => isFalling());
            _stateMachine.AddTransition(attackState, fallingState, () => isFalling() && !_isAttacking);
            _stateMachine.AddTransition(guardState, fallingState, () => isFalling());

            // to AttackState
            _stateMachine.AddAnyTransition(attackState, true, () => { return _isAttackBtnPressed; });

            // to Guard
            _stateMachine.AddTransition(freeLookState, guardState, () => { return _guardBtnPressed && _playerMover.IsGrounded; });
            _stateMachine.AddTransition(targetLockState, guardState, () => { return _guardBtnPressed && _playerMover.IsGrounded && _targetLocker.HasTarget; });
            _stateMachine.AddTransition(attackState, guardState, () => { return _guardBtnPressed && _playerMover.IsGrounded && !_isAttacking; });

            _stateMachine.SetState(freeLookState);
        }

        private void OnEnable()
        {
            _playerInput.OnMoveEvent += OnMove;
            _playerInput.OnJumpEvent += OnJump;
            _playerInput.OnAttackEvent += OnAttack;
            _playerInput.OnTargetEvent += OnTarget;
            _playerInput.OnGuardEvent += OnGuard;
        }

        private void OnDisable()
        {
            _playerInput.OnMoveEvent -= OnMove;
            _playerInput.OnJumpEvent -= OnJump;
            _playerInput.OnAttackEvent -= OnAttack;
            _playerInput.OnTargetEvent -= OnTarget;
            _playerInput.OnGuardEvent -= OnGuard;
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
            _isAttackBtnPressed = context.performed;
            if (_isAttackBtnPressed)
                _isAttacking = true;
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

        private void OnGuard(InputAction.CallbackContext context)
        {
            _guardBtnPressed = context.performed;
        }

        private void OnApplicationFocus(bool focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}