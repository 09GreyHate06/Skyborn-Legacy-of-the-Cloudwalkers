using System.Collections;
using UnityEngine;

namespace SLOTC.Core.Saving
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] KeyCode _saveKey = KeyCode.S;
        [SerializeField] KeyCode _loadKey = KeyCode.L;
        [SerializeField] KeyCode _deleteKey = KeyCode.D;

        private SavingSystem _savingSystem;

        private void Awake()
        {
            _savingSystem = GetComponent<SavingSystem>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(_saveKey))
            {
                _savingSystem.SerializeGameData("test");
            }
            else if (Input.GetKeyDown(_loadKey))
            {
                StartCoroutine(_savingSystem.DeserializeGameData("test"));
            }
            else if (Input.GetKeyDown(_deleteKey))
            {
                _savingSystem.DeleteSaveFile("test");
            }
        }
    }
}