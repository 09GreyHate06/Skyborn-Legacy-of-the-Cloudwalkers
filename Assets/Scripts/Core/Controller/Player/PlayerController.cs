using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.States.Player;
using SLOTC.Core.Combat;
using SLOTC.Core.Movement.Player;
using SLOTC.Core.Input;
using System;

namespace SLOTC.Core.Controller.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Combat Settings")]
        [SerializeField] WeaponHandler _weaponHandler;
        [SerializeField] SingleAttack[] _combo;
        [SerializeField] float _comboGraceTime;
        [SerializeField] float _dodgeForce = 50.0f;
        [SerializeField] float _dodgeCooldown = 1.0f;

        [Space(10)]
        [Header("Locomotion Settings")]
        [SerializeField] PlayerMover _playerMover;
        [SerializeField] TargetLocker _targetLocker;
        [SerializeField] float _moveSpeed = 6.0f;
        [SerializeField] float _rotationSpeed = 500.0f;
        [SerializeField] float _jumpForce = 6.0f;
        [SerializeField] float _timeLeavedGroundBeforeFallState = 0.5f;

        [Space(10)]
        [Header("Animation Settings")]
        [SerializeField] Animator _animator;
        [SerializeField] float _toIdleAnimTransitionDuration = 0.25f;
        [SerializeField] float _toMoveAnimTransitionDuration = 0.25f;
        [SerializeField] float _toJumpAnimTransitionDuration = 0.25f;
        [SerializeField] float _toFallingAnimTransitionDuration = 0.25f;
        [SerializeField] float _toAttackAnimTransitionDuration = 0.25f;
        [SerializeField] float _toDodgeAnimTransitionDuration = 0.25f;
        [SerializeField] float _toStaggerAnimTransitionDuration = 0.25f;
        [SerializeField] float _animParamDampTime = 0.05f;

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
        private bool _isStaggering;
        private bool _shouldStagger;

        private float _timeSinceLeaveGround;
        private float _timeSinceLastDodge;

        private void Awake()
        {
            _playerInput = FindObjectOfType<PlayerInput>();
        }

        void Start()
        {
            _stateMachine = new StateMachine();
            IdleState idleState = new IdleState(_playerMover, _animator, _toIdleAnimTransitionDuration, _animParamDampTime);
            MoveState moveState = new MoveState(_playerMover, _playerInput, _targetLocker, _animator, _toMoveAnimTransitionDuration, _animParamDampTime, _moveSpeed, _rotationSpeed);
            JumpState jumpState = new JumpState(_playerMover, _playerInput, _targetLocker, _animator, _toJumpAnimTransitionDuration, _moveSpeed, _rotationSpeed, _jumpForce);
            FallingState fallingState = new FallingState(_playerMover, _playerInput, _targetLocker, _animator, _toFallingAnimTransitionDuration, _moveSpeed, _rotationSpeed);
            AttackState attackState = new AttackState(_playerMover, _playerInput, _animator, _toAttackAnimTransitionDuration, _weaponHandler, _combo, _comboGraceTime, _rotationSpeed);
            DodgeState dodgeState = new DodgeState(_playerMover, _playerInput, _targetLocker, _animator, _toDodgeAnimTransitionDuration, _animParamDampTime, _dodgeForce);

            attackState.OnEvent += OnAttackStateEvent;
            dodgeState.OnEvent += OnDodgeStateEvent;

            void AT(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

            // to IdleState
            {
                AT(moveState, idleState, () => _playerMover.IsGrounded && !_moveBtnPressed);
                AT(jumpState, idleState, () => _playerMover.IsGrounded);
                AT(fallingState, idleState, () => _playerMover.IsGrounded);
                AT(attackState, idleState, () => _playerMover.IsGrounded && _attackAnimationEnded);
                AT(dodgeState, idleState, () => _playerMover.IsGrounded && _dodgeAnimationEnded);
            }


            // to MoveState
            bool ToMoveBase() { return _playerMover.IsGrounded && _moveBtnPressed; }
            {
                AT(idleState, moveState, ToMoveBase);
                AT(attackState, moveState, () => ToMoveBase() && !_isAttacking);
                AT(dodgeState, moveState, () => ToMoveBase() && !_isDodging);
            }


            // to jumpState
            bool ToJumpStateBase() { return _playerMover.IsGrounded && _jumpBtnPressed; }
            {
                AT(idleState, jumpState, ToJumpStateBase);
                AT(moveState, jumpState, ToJumpStateBase);
                AT(attackState, jumpState, () => ToJumpStateBase() && !_isAttacking);
                AT(dodgeState, jumpState, () => ToJumpStateBase() && !_isDodging);
            }


            // to FallingState 
            bool ToFallingStateBase() { return _timeSinceLeaveGround >= _timeLeavedGroundBeforeFallState && _playerMover.velocity.y < -1.5f; }
            {
                AT(idleState, fallingState, ToFallingStateBase);
                AT(moveState, fallingState, ToFallingStateBase);
                AT(jumpState, fallingState, ToFallingStateBase);
                AT(attackState, fallingState, () => ToFallingStateBase() && !_isAttacking);
                AT(dodgeState, fallingState, () => ToFallingStateBase() && !_isDodging);
            }


            // to AttackState
            _stateMachine.AddAnyTransition(attackState, true, () => _attackBtnPressed && !_isAttacking && !_isStaggering);

            // to DodgeState
            _stateMachine.AddAnyTransition(dodgeState, false, () => _dodgeBtnPressed && _timeSinceLastDodge >= _dodgeCooldown && !_isDodging && !_isStaggering);

            // StaggeredState
            if(TryGetComponent(out Knockbackable knockbackable))
            {
                knockbackable.OnKnockback += (Knockbackable.KnockbackType knockbackType) =>
                {
                    if (knockbackType == Knockbackable.KnockbackType.Stagger && !_isAttacking && !_isStaggering)
                        _shouldStagger = true;
                };

                StaggeredState staggeredState = new StaggeredState(_playerMover, _animator, _toStaggerAnimTransitionDuration);
                staggeredState.OnEvent += OnStaggerStateEvent;

                _stateMachine.AddAnyTransition(staggeredState, false, () => _shouldStagger);

                AT(staggeredState, idleState, () => _playerMover.IsGrounded && !_isStaggering);
                AT(staggeredState, moveState, () => ToMoveBase() && !_isStaggering);
                AT(staggeredState, jumpState, () => ToJumpStateBase() && !_isStaggering);
                AT(staggeredState, fallingState, () => ToFallingStateBase() && !_isStaggering);
            }

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
            _stateMachine.OnUpdate(Time.deltaTime);

            if (!_playerMover.IsGrounded)
                _timeSinceLeaveGround += Time.deltaTime;
            else
                _timeSinceLeaveGround = 0.0f;

            if (!_isDodging)
                _timeSinceLastDodge += Time.deltaTime;
            else
                _timeSinceLastDodge = 0.0f;

            _shouldStagger = false;
        }

        private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _moveBtnPressed = context.performed;
        }

        private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _jumpBtnPressed = context.performed;
        }

        private void OnAttack(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _attackBtnPressed = context.performed;
        }

        private void OnTarget(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;


            if (_targetLocker.HasTarget)
                _targetLocker.Cancel();
            else
                _targetLocker.SelectTarget();
        }

        private void OnDodge(UnityEngine.InputSystem.InputAction.CallbackContext context)
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

        private void OnStaggerStateEvent(StaggeredState.EventType type)
        {
            switch (type)
            {
                case StaggeredState.EventType.Enter:
                    _isStaggering = true;
                    break;
                case StaggeredState.EventType.StaggerEnded:
                    _isStaggering = false;
                    break;
                case StaggeredState.EventType.Exit:
                    _isStaggering = false;
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