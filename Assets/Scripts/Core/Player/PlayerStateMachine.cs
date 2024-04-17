using UnityEngine;
using SLOTC.Utils.StateMachine;
using SLOTC.Core.Player.States;

using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;

namespace SLOTC.Core.Player
{
    public class PlayerStateMachine : MonoBehaviour
    {
        [SerializeField] float _moveSpeed = 3.0f;
        [SerializeField] float _rotationSpeed = 50.0f;
        [SerializeField] float _jumpForce = 5.0f;

        private StateMachine _stateMachine;
        private PlayerInput _playerInput;

        private bool _isMoving;

        private void Awake()
        {
            _stateMachine = new StateMachine();
            _playerInput = FindObjectOfType<PlayerInput>();
        }

        void Start()
        {
            _playerInput.OnMoveEvent += OnMove;

            Animator animator = GetComponent<Animator>();
            PlayerMover playerMover = GetComponent<PlayerMover>();

            EmptyState emptyState = new EmptyState();
            MoveState moveState = new MoveState(playerMover, animator, _moveSpeed, _rotationSpeed, 0.05f);

            _stateMachine.AddTransition(emptyState, moveState, () => { return _isMoving; });
            _stateMachine.AddTransition(moveState, emptyState, () => { return !_isMoving; });

            _stateMachine.SetState(emptyState);
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
            _isMoving = context.performed;
        }
    }
}