/*
using E9COH.Core.Player;
using E9COH.Core.Saving;
using E9COH.Core.Scene;
using SLOTC.Core.Controller.Player;
using UnityEditor;
using UnityEngine;

namespace SLOTC
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField] KeyCode _openCloseMenuPanel = KeyCode.I;
        [SerializeField] GameObject _menuPanel;
        [SerializeField] GameObject _attributesPanel;
        [SerializeField] GameObject _statusPanel;
        [SerializeField] GameObject _martialArtsPanel;
        [SerializeField] GameObject _equipmentsPanel;
        [SerializeField] GameObject _inventoryPanel;

        private PlayerController _playerController;
        private CameraController _cameraController;

        private void Awake()
        {
            _playerController = GameObject.FindObjectOfType<PlayerController>();
            _cameraController = GameObject.FindObjectOfType<CameraController>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(_openCloseMenuPanel))
            {
                _menuPanel.SetActive(!_menuPanel.activeInHierarchy);
                if (!_menuPanel.activeInHierarchy)
                {
                    CloseAllPanel();
                }

                _playerController.enabled = !_menuPanel.activeInHierarchy;
                _cameraController.enabled = !_menuPanel.activeInHierarchy;
                Cursor.visible = !_cameraController.enabled;
                Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }

        public void OpenCloseAttributesPanel(bool open)
        {
            _attributesPanel.SetActive(open);
            if (!open)
                _attributesPanel.transform.localPosition = Vector3.zero;
            else
                _attributesPanel.transform.SetAsLastSibling();
        }

        public void OpenCloseStatusPanel(bool open)
        {
            _statusPanel.SetActive(open);
            if (!open)
                _statusPanel.transform.localPosition = Vector3.zero;
            else
                _statusPanel.transform.SetAsLastSibling();
        }

        public void OpenCloseMartialArtsPanel(bool open)
        {
            _martialArtsPanel.SetActive(open);
            if (!open)
                _martialArtsPanel.transform.localPosition = Vector3.zero;
            else
                _martialArtsPanel.transform.SetAsLastSibling();
        }

        public void OpenCloseEquipmentsPanel(bool open)
        {
            _equipmentsPanel.SetActive(open);
            if (!open)
                _equipmentsPanel.transform.localPosition = Vector3.zero;
            else
                _equipmentsPanel.transform.SetAsLastSibling();
        }

        public void OpenCloseInventoryPanel(bool open)
        {
            _inventoryPanel.SetActive(open);
            if (!open)
                _inventoryPanel.transform.localPosition = Vector3.zero;
            else
                _inventoryPanel.transform.SetAsLastSibling();
        }

        public void CloseAllPanel()
        {
            OpenCloseAttributesPanel(false);
            OpenCloseStatusPanel(false);
            OpenCloseMartialArtsPanel(false);
            OpenCloseEquipmentsPanel(false);
            OpenCloseInventoryPanel(false);
            _menuPanel.SetActive(false);
            _menuPanel.transform.localPosition = Vector3.zero;

            _playerController.enabled = true;
            _cameraController.enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Save()
        {
            GameObject.FindObjectOfType<SavingWrapper>().Save();
        }

        public void Load()
        {
            GameObject.FindObjectOfType<SavingWrapper>().Load();
        }
    }
}*/