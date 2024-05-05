using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SLOTC.Core.Input
{
    public class PlayerInput : MonoBehaviour, PlayerInputActions.IGameplayActions, PlayerInputActions.IUIActions
    {
        private PlayerInputActions _actions;

        public event Action<InputAction.CallbackContext> OnMoveEvent;
        public event Action<InputAction.CallbackContext> OnJumpEvent;
        public event Action<InputAction.CallbackContext> OnAttackEvent;
        public event Action<InputAction.CallbackContext> OnTargetEvent;
        public event Action<InputAction.CallbackContext> OnDodgeEvent;
        public event Action<InputAction.CallbackContext> OnCharacterMenuEvent;
        public event Action<InputAction.CallbackContext> OnActionBarSlot1Event;
        public event Action<InputAction.CallbackContext> OnActionBarSlot2Event;
        public event Action<InputAction.CallbackContext> OnActionBarSlot3Event;
        public event Action<InputAction.CallbackContext> OnActionBarSlot4Event;
        public event Action<InputAction.CallbackContext> OnActionBarSlot5Event;
        public event Action<InputAction.CallbackContext> OnActionBarSlot6Event;
        public event Action<InputAction.CallbackContext> OnActionBarSlot7Event;
        public event Action<InputAction.CallbackContext> OnActionBarSlot8Event;
        public event Action<InputAction.CallbackContext> OnActionBarSlot9Event;
        public event Action<InputAction.CallbackContext> OnActionBarSlot10Event;

        public Vector2 Axis {  get; private set; }

        private void Awake()
        {
            _actions = new PlayerInputActions();
            _actions.Gameplay.SetCallbacks(this);
            _actions.UI.SetCallbacks(this);
        }

        private void OnEnable()
        {
            EnableGameplayActions();
            EnableUIActions();
        }

        private void OnDisable()
        {
            DisableGameplayActions();
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

        public void EnableGameplayActions()
        {
            _actions.Gameplay.Enable();
        }

        public void DisableGameplayActions()
        {
            _actions.Gameplay.Disable();
        }

        public void EnableUIActions()
        {
            _actions.UI.Enable();
        }

        public void DisableUIActions()
        {
            _actions.UI.Disable();
        }

        public void OnCharacterMenu(InputAction.CallbackContext context)
        {
            OnCharacterMenuEvent?.Invoke(context);
        }

        public void OnActionBarSlot1(InputAction.CallbackContext context)
        {
            OnActionBarSlot1Event?.Invoke(context);
        }

        public void OnActionBarSlot2(InputAction.CallbackContext context)
        {
            OnActionBarSlot2Event?.Invoke(context);
        }

        public void OnActionBarSlot3(InputAction.CallbackContext context)
        {
            OnActionBarSlot3Event?.Invoke(context);
        }

        public void OnActionBarSlot4(InputAction.CallbackContext context)
        {
            OnActionBarSlot4Event?.Invoke(context);
        }

        public void OnActionBarSlot5(InputAction.CallbackContext context)
        {
            OnActionBarSlot5Event?.Invoke(context);
        }

        public void OnActionBarSlot6(InputAction.CallbackContext context)
        {
            OnActionBarSlot6Event?.Invoke(context);
        }

        public void OnActionBarSlot7(InputAction.CallbackContext context)
        {
            OnActionBarSlot7Event?.Invoke(context);
        }

        public void OnActionBarSlot8(InputAction.CallbackContext context)
        {
            OnActionBarSlot8Event?.Invoke(context);
        }

        public void OnActionBarSlot9(InputAction.CallbackContext context)
        {
            OnActionBarSlot9Event?.Invoke(context);
        }

        public void OnActionBarSlot10(InputAction.CallbackContext context)
        {
            OnActionBarSlot10Event?.Invoke(context);
        }
    }
}