using Cinemachine;
using SLOTC.Core.Input;
using UnityEngine;

namespace SLOTC.Core.UI
{
    public class CharacterUI : MonoBehaviour
    {
        [SerializeField] GameObject _characterUI;
        [SerializeField] GameObject _inventoryUI;

        private PlayerInput _playerInput;
        private CinemachineInputProvider _cmInputProvider;

        private void Awake()
        {
            _playerInput = FindObjectOfType<PlayerInput>();
            _cmInputProvider = Camera.main.GetComponentInChildren<CinemachineInputProvider>();
        }

        private void OnEnable()
        {
            _playerInput.OnCharacterMenuEvent += OnCharacterMenuEvent;
        }

        private void OnDisable()
        {
            _playerInput.OnCharacterMenuEvent -= OnCharacterMenuEvent;
        }

        public void OpenCloseCharacterUI(bool open)
        {
            _characterUI.SetActive(open);
            if (!open)
            {
                _characterUI.transform.localPosition = Vector3.zero;
                OpenCloseInventoryUI(false);
            }
            else
                _characterUI.transform.SetAsLastSibling();

            LockCursor(!open);
            if (open)
            {
                _playerInput.DisableGameplayActions();
                _cmInputProvider.enabled = false;
            }
            else
            {
                _cmInputProvider.enabled = true;
                _playerInput.EnableGameplayActions();
            }
        }

        public void OpenCloseInventoryUI(bool open)
        {
            _inventoryUI.SetActive(open);
            if (!open)
                _inventoryUI.transform.localPosition = Vector3.zero;
            else
                _inventoryUI.transform.SetAsLastSibling();
        }

        private void OnCharacterMenuEvent(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed)
                OpenCloseCharacterUI(!_characterUI.activeInHierarchy);
        }

        private void LockCursor(bool shouldLock)
        {
            Cursor.lockState = shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !shouldLock;
        }
    }
}