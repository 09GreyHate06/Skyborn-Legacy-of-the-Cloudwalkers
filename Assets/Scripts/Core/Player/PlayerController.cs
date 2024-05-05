using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Combat;
using SLOTC.Core.Movement.Player;
using SLOTC.Core.Player.States;
using SLOTC.Core.Input;
using System;
using Animancer;
using SLOTC.Core.Combat.Animation;
using System.Collections;
using SLOTC.Core.Inventory;

namespace SLOTC.Core.Player
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
        [EventNames(CombatAnimationEventNames.DoAction, CombatAnimationEventNames.Exit)]
        [SerializeField] ClipTransition _castAnim;
        [SerializeField] float _animBlendSpeed;


        private StateMachine _stateMachine;
        private PlayerInput _playerInput;

        private bool _jumpBtnPressed;
        private bool _attackBtnPressed;
        private bool _moveBtnPressed;
        private bool _dodgeBtnPressed;

        private bool _attackAnimationEnded = true;
        private bool _dodgeAnimationEnded = true;
        private bool _castAnimationEnded = true;
        private bool _staggerAnimationEnded = true;
        private bool _shouldStagger;
        private bool _canDodge = true;
        private bool _useItem;

        private float _timeSinceLeaveGround;

        private UseActionItemState _useActionItemState;
        //public MartialArt _tempMA;
        //public bool _temp;

        private void Awake()
        {
            _playerInput = FindObjectOfType<PlayerInput>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
            _useActionItemState = new UseActionItemState(_playerMover, _animancer, _castAnim, GetComponent<ActionItemBar>());
            //MartialArtState martialArtState = new MartialArtState(_playerMover, _playerInput, _animancer, _weaponHandler, _rotationSpeed);
            //martialArtState.SetMartialArt(_tempMA);


            attackState.OnAnimationEnded += () => _attackAnimationEnded = true;
            dodgeState.OnAnimationEnded += () => _dodgeAnimationEnded = true;
            staggeredState.OnAnimationEnded += () => _staggerAnimationEnded = true;
            _useActionItemState.OnAnimationEnded += () => _castAnimationEnded = true;
            
            void AT(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

            // to IdleState
            {
                AT(moveState, idleState, () => _playerMover.IsGrounded && !_moveBtnPressed);
                AT(jumpState, idleState, () => _playerMover.IsGrounded);
                AT(fallingState, idleState, () => _playerMover.IsGrounded);
                AT(attackState, idleState, () => _playerMover.IsGrounded && _attackAnimationEnded);
                AT(dodgeState, idleState, () => _playerMover.IsGrounded && _dodgeAnimationEnded);
                AT(staggeredState, idleState, () => _playerMover.IsGrounded && _staggerAnimationEnded);
                AT(_useActionItemState, idleState, () => _playerMover.IsGrounded && _castAnimationEnded);
            }


            // to MoveState
            bool ToMoveBase() { return _playerMover.IsGrounded && _moveBtnPressed; }
            {
                AT(idleState, moveState, ToMoveBase);
                AT(attackState, moveState, ToMoveBase);
                AT(dodgeState, moveState, ToMoveBase);
                AT(staggeredState, moveState, ToMoveBase);
                AT(_useActionItemState, moveState, ToMoveBase);
            }

            // to jumpState
            bool ToJumpStateBase() { return _playerMover.IsGrounded && _jumpBtnPressed; }
            {
                AT(idleState, jumpState, ToJumpStateBase);
                AT(moveState, jumpState, ToJumpStateBase);
                AT(attackState, jumpState, ToJumpStateBase);
                AT(dodgeState, jumpState, ToJumpStateBase);
                AT(staggeredState, jumpState, ToJumpStateBase);
                AT(_useActionItemState, jumpState, ToJumpStateBase);
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
                AT(_useActionItemState, fallingState, ToFallingStateBase);
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

            _stateMachine.AddAnyTransition(_useActionItemState, false, () => _useItem);

            //_stateMachine.AddAnyTransition(martialArtState, true, () => _temp && (martialArtState.CanExit || _stateMachine.CurrentState.GetID() != martialArtState.GetID()));

            _stateMachine.SetState(idleState);

            _stateMachine.OnBeforeChangeState += (IState nextState) =>
            {
                _shouldStagger = false;
                _useItem = false;

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
                else if (_stateMachine.CurrentState.GetID() == _useActionItemState.GetID())
                {
                    _castAnimationEnded = true;
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
                else if (nextState.GetID() == staggeredState.GetID())
                {
                    _staggerAnimationEnded = false;
                }
                else if (nextState.GetID() == _useActionItemState.GetID())
                {
                    _castAnimationEnded = false;
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
            _playerInput.OnActionBarSlot1Event += OnActionBarSlot1;
            _playerInput.OnActionBarSlot2Event += OnActionBarSlot2;
            _playerInput.OnActionBarSlot3Event += OnActionBarSlot3;
            _playerInput.OnActionBarSlot4Event += OnActionBarSlot4;
            _playerInput.OnActionBarSlot5Event += OnActionBarSlot5;
            _playerInput.OnActionBarSlot6Event += OnActionBarSlot6;
            _playerInput.OnActionBarSlot7Event += OnActionBarSlot7;
            _playerInput.OnActionBarSlot8Event += OnActionBarSlot8;
            _playerInput.OnActionBarSlot9Event += OnActionBarSlot9;
            _playerInput.OnActionBarSlot10Event += OnActionBarSlot10;
        }

        private void OnDisable()
        {
            _playerInput.OnMoveEvent -= OnMove;
            _playerInput.OnJumpEvent -= OnJump;
            _playerInput.OnAttackEvent -= OnAttack;
            _playerInput.OnTargetEvent -= OnTarget;
            _playerInput.OnDodgeEvent -= OnDodge;
            _playerInput.OnActionBarSlot1Event -= OnActionBarSlot1;
            _playerInput.OnActionBarSlot2Event -= OnActionBarSlot2;
            _playerInput.OnActionBarSlot3Event -= OnActionBarSlot3;
            _playerInput.OnActionBarSlot4Event -= OnActionBarSlot4;
            _playerInput.OnActionBarSlot5Event -= OnActionBarSlot5;
            _playerInput.OnActionBarSlot6Event -= OnActionBarSlot6;
            _playerInput.OnActionBarSlot7Event -= OnActionBarSlot7;
            _playerInput.OnActionBarSlot8Event -= OnActionBarSlot8;
            _playerInput.OnActionBarSlot9Event -= OnActionBarSlot9;
            _playerInput.OnActionBarSlot10Event -= OnActionBarSlot10;
        }

        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            
            Awake();
            Start();
        }


        void Update()
        {
            //if (UnityEngine.InputSystem.Keyboard.current.tKey.wasPressedThisFrame)
            //{
            //    _temp = true;
            //}
            //else
            //{
            //    _temp = false;
            //}

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

        private IEnumerator DodgeCooldown()
        {
            _canDodge = false;
            yield return new WaitForSeconds(_dodgeCooldown);
            _canDodge = true;
        }

        private void OnActionBarSlot1(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _useActionItemState.SetActionBarSlotIndex(0);
                _useItem = true;
            }
        }

        private void OnActionBarSlot2(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _useActionItemState.SetActionBarSlotIndex(1);
                _useItem = true;
            }
        }

        private void OnActionBarSlot3(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed)
            { 
                _useActionItemState.SetActionBarSlotIndex(2);
                _useItem = true;
            }
        }

        private void OnActionBarSlot4(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _useActionItemState.SetActionBarSlotIndex(3);
                _useItem = true;
            }
        }

        private void OnActionBarSlot5(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _useActionItemState.SetActionBarSlotIndex(4);
                _useItem = true;
            }
        }

        private void OnActionBarSlot6(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _useActionItemState.SetActionBarSlotIndex(5);
                _useItem = true;
            }
        }

        private void OnActionBarSlot7(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _useActionItemState.SetActionBarSlotIndex(6);
                _useItem = true;
            }
        }

        private void OnActionBarSlot8(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _useActionItemState.SetActionBarSlotIndex(7);
                _useItem = true;
            }
        }

        private void OnActionBarSlot9(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed)
            { 
                _useActionItemState.SetActionBarSlotIndex(8);
                _useItem = true;
            }
        }

        private void OnActionBarSlot10(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _useActionItemState.SetActionBarSlotIndex(9);
                _useItem = true;
            }
        }
    }
}