using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SLOTC.Core.Player
{
    public class PlayerInput : MonoBehaviour, PlayerInputActions.IGameplayActions
    {
        private PlayerInputActions _actions;

        public event Action<InputAction.CallbackContext> OnMoveEvent;
        public event Action<InputAction.CallbackContext> OnJumpEvent;
        public event Action<InputAction.CallbackContext> OnAttackEvent;
        public event Action<InputAction.CallbackContext> OnTargetEvent;
        public event Action<InputAction.CallbackContext> OnDodgeEvent;

        public Vector2 Axis {  get; private set; }

        private void Awake()
        {
            _actions = new PlayerInputActions();
            _actions.Gameplay.SetCallbacks(this);
        }

        private void OnEnable()
        {
            _actions.Gameplay.Enable();
        }

        private void OnDisable()
        {
            _actions.Gameplay.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Axis = context.ReadValue<Vector2>();
            OnMoveEvent?.Invoke(context);
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            OnJumpEvent?.Invoke(context);
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            OnAttackEvent?.Invoke(context);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
        }

        public void OnTarget(InputAction.CallbackContext context)
        {
            OnTargetEvent?.Invoke(context);
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            OnDodgeEvent?.Invoke(context);
        }
    }
}