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
        [SerializeField] float _timeLeavedGroundBeforeFallState = 1.0f;
        [SerializeField] TargetLocker _targetLocker;

        [Space(10)]
        [Header("Animation Settings")]
        [SerializeField] float _toIdleAnimTransitonDuration = 0.25f;
        [SerializeField] float _toFreeLookAnimTransitonDuration = 0.25f;
        [SerializeField] float _toTargetLockedAnimTransitonDuration = 0.25f;
        [SerializeField] float _toJumpAnimTransitonDuration = 0.25f;
        [SerializeField] float _toFallingAnimTransitonDuration = 0.25f;
        [SerializeField] float _idleDampTime = 0.05f;
        [SerializeField] float _freeLookBlendTreeDampTime = 0.05f;
        [SerializeField] float _targetLockedBlendTreeDampTime = 0.05f;

        private StateMachine _stateMachine;
        private PlayerInput _playerInput;
        private PlayerMover _playerMover;

        private bool _jumpBtnPressed;
        private bool _attackBtnPressed;
        private bool _moveBtnPressed;
        private bool _isAttacking = false;
        private bool _attackAnimFinished = true;

        private bool _leavedGroundFlag;
        private float _timeSinceLeaveGround;

        private void Awake()
        {
            _playerInput = FindObjectOfType<PlayerInput>();
            _stateMachine = new StateMachine();
        }

        void Start()
        {
            Animator animator = GetComponent<Animator>();
            _playerMover = GetComponent<PlayerMover>();

            IdleState idleState = new IdleState(_playerMover, animator, _toIdleAnimTransitonDuration, _idleDampTime);
            FreeLookMoveState freeLookMoveState = new FreeLookMoveState(_playerMover, _playerInput, animator, 
                _toFreeLookAnimTransitonDuration, _freeLookBlendTreeDampTime, _moveSpeed, _rotationSpeed);
            TargetLockedMoveState targetLockMoveState = new TargetLockedMoveState(_playerMover, _playerInput, animator, 
                _toTargetLockedAnimTransitonDuration, _targetLockedBlendTreeDampTime, _moveSpeed, _rotationSpeed);
            JumpState jumpState = new JumpState(_playerMover, animator, _toJumpAnimTransitonDuration, _jumpForce);
            FallingState fallingState = new FallingState(animator, _toFallingAnimTransitonDuration);
            AttackState attackState = new AttackState(_playerMover, _playerInput, animator, _combo, _comboGraceTime, _rotationSpeed);

            attackState.OnAttackFinished += () => { _isAttacking = false; };
            attackState.OnAnimationFinished += () => { _attackAnimFinished = true; };

            void AT(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

            // to IdleState
            AT(freeLookMoveState, idleState, () => { return _playerMover.IsGrounded && !_moveBtnPressed; });
            AT(targetLockMoveState, idleState, () => { return _playerMover.IsGrounded && !_moveBtnPressed; });
            AT(jumpState, idleState, () => { return _playerMover.IsGrounded; });
            AT(fallingState, idleState, () => { return _playerMover.IsGrounded; });
            AT(attackState, idleState, () => { return _playerMover.IsGrounded && _attackAnimFinished; });

            // to FreeLookMoveState
            bool toFLMoveBase() { return _playerMover.IsGrounded && !_targetLocker.HasTarget && _moveBtnPressed; }
            AT(idleState, freeLookMoveState, toFLMoveBase);
            AT(targetLockMoveState, freeLookMoveState, toFLMoveBase);
            AT(attackState, freeLookMoveState, () => { return toFLMoveBase() && !_isAttacking; });

            // to TargetLockMoveState
            bool toTLMoveBase() { return _playerMover.IsGrounded && _targetLocker.HasTarget && _moveBtnPressed; }
            AT(idleState, targetLockMoveState, toTLMoveBase);
            AT(freeLookMoveState, targetLockMoveState, toTLMoveBase);
            AT(attackState, freeLookMoveState, () => { return toTLMoveBase() && !_isAttacking; });

            // to jumpState
            bool toJumpStateBase() { return _playerMover.IsGrounded && _jumpBtnPressed; }
            AT(idleState, jumpState, toJumpStateBase);
            AT(freeLookMoveState, jumpState, toJumpStateBase);
            AT(targetLockMoveState, jumpState, toJumpStateBase);
            AT(attackState, jumpState, () => { return toJumpStateBase() && !_isAttacking; });

            // to FallingState 
            // todo: walking when falling and rotating
            bool isFallingBase() { return _timeSinceLeaveGround >= _timeLeavedGroundBeforeFallState; }
            AT(idleState, fallingState, isFallingBase);
            AT(freeLookMoveState, fallingState, isFallingBase);
            AT(targetLockMoveState, fallingState, isFallingBase);
            AT(jumpState, fallingState, isFallingBase);
            AT(attackState, fallingState, () => { return isFallingBase() && !_isAttacking; });

            // to AttackState
            _stateMachine.AddAnyTransition(attackState, true, () => 
            { 
                bool result = _attackBtnPressed && !_isAttacking;

                if (result)
                {
                    _isAttacking = true;
                    _attackAnimFinished = false;
                }

                return result;
            });

            _stateMachine.SetState(freeLookMoveState);
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
            if (!_playerMover.IsGrounded)
                _timeSinceLeaveGround += Time.deltaTime;
            else 
                _timeSinceLeaveGround = 0.0f;
            
            _stateMachine.OnUpdate(Time.deltaTime);
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            _moveBtnPressed = context.performed;
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            _jumpBtnPressed = context.performed;
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            _attackBtnPressed = context.performed;
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
        }

        private void OnApplicationFocus(bool focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}