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
        [SerializeField] PlayerMover _playerMover;
        [SerializeField] TargetLocker _targetLocker;
        [SerializeField] float _moveSpeed = 6.0f;
        [SerializeField] float _rotationSpeed = 500.0f;
        [SerializeField] float _jumpForce = 6.0f;
        [SerializeField] float _timeLeavedGroundBeforeFallState = 0.5f;
        [SerializeField] float _dodgeForce = 50.0f;
        [SerializeField] float _dodgeCooldown = 1.0f;

        [Space(10)]
        [Header("Animation Settings")]
        [SerializeField] Animator _animator;
        [SerializeField] float _toIdleAnimTransitionDuration = 0.25f;
        [SerializeField] float _idleAnimDampTime = 0.05f;
        [SerializeField] float _toMoveAnimTransitionDuration = 0.25f;
        [SerializeField] float _moveAnimDampTime = 0.05f;
        [SerializeField] float _toJumpAnimTransitionDuration = 0.25f;
        [SerializeField] float _toFallingAnimTransitionDuration = 0.25f;
        [SerializeField] float _toDodgeAnimTransitionDuration = 0.25f;
        [SerializeField] float _dodgeAnimNormExitTime = 1.0f;
        [SerializeField] float _dodgeAnimDampTime = 0.05f;

        private StateMachine _stateMachine;
        private PlayerInput _playerInput;

        private bool _jumpBtnPressed;
        private bool _attackBtnPressed;
        private bool _moveBtnPressed;
        private bool _dodgeBtnPressed;

        private bool _isAttacking;
        private bool _attackAnimationEnded = true;
        private bool _isDodging;
        private bool _dodgeAnimationEnded = true;

        private float _timeSinceLeaveGround;
        private float _timeSinceLastDodge;

        private void Awake()
        {
            _playerInput = FindObjectOfType<PlayerInput>();
            _stateMachine = new StateMachine();
        }

        void Start()
        {
            IdleState idleState = new IdleState(_playerMover, _animator, _toIdleAnimTransitionDuration, _idleAnimDampTime);
            MoveState moveState = new MoveState(_playerMover, _playerInput, _targetLocker, _animator, _toMoveAnimTransitionDuration, _moveAnimDampTime, _moveSpeed, _rotationSpeed);
            JumpState jumpState = new JumpState(_playerMover, _playerInput, _targetLocker, _animator, _toJumpAnimTransitionDuration, _moveSpeed, _rotationSpeed, _jumpForce);
            FallingState fallingState = new FallingState(_playerMover, _playerInput, _targetLocker, _animator, _toFallingAnimTransitionDuration, _moveSpeed, _rotationSpeed);
            AttackState attackState = new AttackState(_playerMover, _playerInput, _animator, _combo, _comboGraceTime, _rotationSpeed);
            DodgeState dodgeState = new DodgeState(_playerMover, _playerInput, _targetLocker, _animator, _dodgeAnimNormExitTime, 
                _toDodgeAnimTransitionDuration, _dodgeAnimDampTime, _dodgeForce);

            attackState.OnEvent += OnAttackStateEvent;
            dodgeState.OnEvent += OnDodgeStateEvent;

            void AT(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

            // to IdleState
            AT(moveState, idleState, () => { return _playerMover.IsGrounded && !_moveBtnPressed; });
            AT(jumpState, idleState, () => { return _playerMover.IsGrounded; });
            AT(fallingState, idleState, () => { return _playerMover.IsGrounded; });
            AT(attackState, idleState, () => { return _playerMover.IsGrounded && _attackAnimationEnded; });
            AT(dodgeState, idleState, () => { return _playerMover.IsGrounded && _dodgeAnimationEnded; });

            // to MoveState
            bool toMoveBase() { return _playerMover.IsGrounded && _moveBtnPressed; }
            AT(idleState, moveState, toMoveBase);
            AT(attackState, moveState, () => { return toMoveBase() && !_isAttacking; });
            AT(dodgeState, moveState, () => { return toMoveBase() && !_isDodging; });

            // to jumpState
            bool toJumpStateBase() { return _playerMover.IsGrounded && _jumpBtnPressed; }
            AT(idleState, jumpState, toJumpStateBase);
            AT(moveState, jumpState, toJumpStateBase);
            AT(attackState, jumpState, () => { return toJumpStateBase() && !_isAttacking; });
            AT(dodgeState, jumpState, () => { return toJumpStateBase() && !_isDodging; });

            // to FallingState 
            bool isFallingBase() { return _timeSinceLeaveGround >= _timeLeavedGroundBeforeFallState; }
            AT(idleState, fallingState, isFallingBase);
            AT(moveState, fallingState, isFallingBase);
            AT(jumpState, fallingState, isFallingBase);
            AT(attackState, fallingState, () => { return isFallingBase() && !_isAttacking; });
            AT(dodgeState, fallingState, () => { return isFallingBase() && !_isDodging; });

            // to AttackState
            _stateMachine.AddAnyTransition(attackState, true, () => { return _attackBtnPressed && !_isAttacking; });

            // to DodgeState
            _stateMachine.AddAnyTransition(dodgeState, false, () => { return _dodgeBtnPressed && _timeSinceLastDodge >= _dodgeCooldown && !_isDodging; });

            _stateMachine.SetState(idleState);
        }

        private void OnEnable()
        {
            _playerInput.OnMoveEvent += OnMove;
            _playerInput.OnJumpEvent += OnJump;
            _playerInput.OnAttackEvent += OnAttack;
            _playerInput.OnTargetEvent += OnTarget;
            _playerInput.OnDodgeEvent += OnDodge;
        }

        private void OnDisable()
        {
            _playerInput.OnMoveEvent -= OnMove;
            _playerInput.OnJumpEvent -= OnJump;
            _playerInput.OnAttackEvent -= OnAttack;
            _playerInput.OnTargetEvent -= OnTarget;
            _playerInput.OnDodgeEvent -= OnDodge;
        }

        private void OnValidate()
        {
            if (!Application.isPlaying) return;

            Awake();
            Start();
        }


        void Update()
        {
            if (!_playerMover.IsGrounded)
                _timeSinceLeaveGround += Time.deltaTime;
            else
                _timeSinceLeaveGround = 0.0f;

            if (!_isDodging)
                _timeSinceLastDodge += Time.deltaTime;
            else
                _timeSinceLastDodge = 0.0f;

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

        private void OnDodge(InputAction.CallbackContext context)
        {
            _dodgeBtnPressed = context.performed;
        }

        private void OnAttackStateEvent(AttackState.EventType type)
        {
            switch(type)
            {
                case AttackState.EventType.Enter:
                    _isAttacking = true;
                    _attackAnimationEnded = false;
                    break;

                case AttackState.EventType.AttackEnded:
                    _isAttacking = false;
                    break;

                case AttackState.EventType.AnimationEnded:
                    _attackAnimationEnded = true;
                    break;

                case AttackState.EventType.Exit:
                    _isAttacking = false;
                    _attackAnimationEnded = true;
                    break;
            }
        }

        private void OnDodgeStateEvent(DodgeState.EventType type)
        {
            switch (type)
            {
                case DodgeState.EventType.Enter:
                    _isDodging = true;
                    _dodgeAnimationEnded = false;
                    break;

                case DodgeState.EventType.DodgeEnded:
                    _isDodging = false;
                    break;

                case DodgeState.EventType.AnimationEnded:
                    _dodgeAnimationEnded = true;
                    break;

                case DodgeState.EventType.Exit:
                    _isDodging = false;
                    _dodgeAnimationEnded = true;
                    break;
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}