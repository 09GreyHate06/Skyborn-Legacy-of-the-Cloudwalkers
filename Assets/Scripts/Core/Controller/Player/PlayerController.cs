using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.States.Player;
using SLOTC.Core.Combat;
using SLOTC.Core.Movement.Player;
using SLOTC.Core.Input;
using System;
using Animancer;
using SLOTC.Core.Combat.Animation;
using System.Collections;

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
        [SerializeField] AnimancerComponent _animancer;
        [SerializeField] MixerTransition2D _moveAnim;
        [EventNames(CombatAnimationEventNames.DodgeEnded, CombatAnimationEventNames.Exit)]
        [SerializeField] MixerTransition2D _dodgeAnim;
        [SerializeField] ClipTransition _jumpAnim;
        [SerializeField] ClipTransition _fallingAnim;
        [EventNames(CombatAnimationEventNames.StaggerEnded, CombatAnimationEventNames.Exit)]
        [SerializeField] ClipTransition _staggerAnim;
        [SerializeField] float _animBlendSpeed;

        private StateMachine _stateMachine;
        private PlayerInput _playerInput;

        private bool _jumpBtnPressed;
        private bool _attackBtnPressed;
        private bool _moveBtnPressed;
        private bool _dodgeBtnPressed;

        private bool _attackAnimationEnded = true;
        private bool _dodgeAnimationEnded = true;
        private bool _staggerAnimationEnded = true;
        private bool _shouldStagger;
        private bool _canDodge = true;

        private float _timeSinceLeaveGround;

        private void Awake()
        {
            _playerInput = FindObjectOfType<PlayerInput>();
        }

        void Start()
        {
            _stateMachine = new StateMachine();
            _animancer.States.GetOrCreate(_moveAnim);
            IdleState idleState = new IdleState(_playerMover, _animancer, _moveAnim, _animBlendSpeed);
            MoveState moveState = new MoveState(_playerMover, _playerInput, _targetLocker, _animancer, _moveAnim, _animBlendSpeed, _moveSpeed, _rotationSpeed);
            JumpState jumpState = new JumpState(_playerMover, _playerInput, _targetLocker, _animancer, _jumpAnim, _moveSpeed, _rotationSpeed, _jumpForce);
            FallingState fallingState = new FallingState(_playerMover, _playerInput, _targetLocker, _animancer, _fallingAnim, _moveSpeed, _rotationSpeed);
            AttackState attackState = new AttackState(_playerMover, _playerInput, _animancer, _weaponHandler, _combo, _comboGraceTime, _rotationSpeed);
            DodgeState dodgeState = new DodgeState(_playerMover, _playerInput, _targetLocker, _animancer, _dodgeAnim, _animBlendSpeed, _dodgeForce);
            StaggeredState staggeredState = new StaggeredState(_playerMover, _animancer, _staggerAnim);

            attackState.OnAnimationEnded += () => _attackAnimationEnded = true;
            dodgeState.OnAnimationEnded += () => _dodgeAnimationEnded = true;
            staggeredState.OnAnimationEnded += () => _staggerAnimationEnded = true;
            
            void AT(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

            // to IdleState
            {
                AT(moveState, idleState, () => _playerMover.IsGrounded && !_moveBtnPressed);
                AT(jumpState, idleState, () => _playerMover.IsGrounded);
                AT(fallingState, idleState, () => _playerMover.IsGrounded);
                AT(attackState, idleState, () => _playerMover.IsGrounded && _attackAnimationEnded);
                AT(dodgeState, idleState, () => _playerMover.IsGrounded && _dodgeAnimationEnded);
                AT(staggeredState, idleState, () => _playerMover.IsGrounded && _staggerAnimationEnded);
            }


            // to MoveState
            bool ToMoveBase() { return _playerMover.IsGrounded && _moveBtnPressed; }
            {
                AT(idleState, moveState, ToMoveBase);
                AT(attackState, moveState, ToMoveBase);
                AT(dodgeState, moveState, ToMoveBase);
                AT(staggeredState, moveState, ToMoveBase);
            }

            // to jumpState
            bool ToJumpStateBase() { return _playerMover.IsGrounded && _jumpBtnPressed; }
            {
                AT(idleState, jumpState, ToJumpStateBase);
                AT(moveState, jumpState, ToJumpStateBase);
                AT(attackState, jumpState, ToJumpStateBase);
                AT(dodgeState, jumpState, ToJumpStateBase);
                AT(staggeredState, jumpState, ToJumpStateBase);
            }
            
            
            // to FallingState 
            bool ToFallingStateBase() { return _timeSinceLeaveGround >= _timeLeavedGroundBeforeFallState && _playerMover.velocity.y < -1.5f; }
            {
                AT(idleState, fallingState, ToFallingStateBase);
                AT(moveState, fallingState, ToFallingStateBase);
                AT(jumpState, fallingState, ToFallingStateBase);
                AT(attackState, fallingState, ToFallingStateBase);
                AT(dodgeState, fallingState, ToFallingStateBase);
                AT(staggeredState, fallingState, ToFallingStateBase);
            }
            
            
            // to AttackState
            _stateMachine.AddAnyTransition(attackState, true, () => 
            {
                if (!_attackBtnPressed || (_stateMachine.CurrentState.GetID() == attackState.GetID() && !_stateMachine.CurrentState.CanExit)
                    || (!_stateMachine.CurrentState.CanExit && _stateMachine.CurrentState.GetID() == staggeredState.GetID()))
                    return false;

                _stateMachine.CurrentState.CanExit = true;
                return true;
            });
            
            // to DodgeState
            _stateMachine.AddAnyTransition(dodgeState, false, () => 
            {
                if (!_canDodge || !_dodgeBtnPressed || (!_stateMachine.CurrentState.CanExit && _stateMachine.CurrentState.GetID() == staggeredState.GetID()))
                    return false;

                _stateMachine.CurrentState.CanExit = true;
                return true;
            });

            // to StaggeredState
            {
                GetComponent<Knockbackable>().OnKnockback += (Knockbackable.KnockbackType knockbackType) =>
                {
                    if (knockbackType == Knockbackable.KnockbackType.Stagger && (_stateMachine.CurrentState.GetID() != attackState.GetID() || _stateMachine.CurrentState.CanExit))
                        _shouldStagger = true;
                };


                _stateMachine.AddAnyTransition(staggeredState, false, () => _shouldStagger);
            }

            _stateMachine.SetState(idleState);

            _stateMachine.OnBeforeChangeState += (IState nextState) =>
            {
                _shouldStagger = false;

                if (_stateMachine.CurrentState.GetID() == attackState.GetID())
                {
                    _attackAnimationEnded = true;
                }
                else if(_stateMachine.CurrentState.GetID() == dodgeState.GetID())
                {
                    _dodgeAnimationEnded = true;
                }
                else if (_stateMachine.CurrentState.GetID() == staggeredState.GetID())
                {
                    _staggerAnimationEnded = true;
                }

                if (nextState.GetID() == attackState.GetID())
                {
                    _attackAnimationEnded = false;
                }
                else if (nextState.GetID() == dodgeState.GetID())
                {
                    StartCoroutine(DodgeCooldown());
                    _dodgeAnimationEnded = false;
                }
                else if (_stateMachine.CurrentState.GetID() == staggeredState.GetID())
                {
                    _staggerAnimationEnded = false;
                }
            };
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
/*
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
        }*/

        private IEnumerator DodgeCooldown()
        {
            _canDodge = false;
            yield return new WaitForSeconds(_dodgeCooldown);
            _canDodge = true;
        }

        private void OnApplicationFocus(bool focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}