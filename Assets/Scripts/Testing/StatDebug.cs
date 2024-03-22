using SLOTC.Core.Saving;
using SLOTC.Core.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDebug : MonoBehaviour
{
    [SerializeField] GameObject _player;

    [SerializeField] KeyCode _saveKey = KeyCode.S;
    [SerializeField] KeyCode _loadKey = KeyCode.L;
    [SerializeField] KeyCode _gainExpKey = KeyCode.X;
    [SerializeField] KeyCode _addMod0 = KeyCode.Alpha0;
    [SerializeField] KeyCode _addMod1 = KeyCode.Alpha1;
    [SerializeField] KeyCode _addMod2 = KeyCode.Alpha2;
    [SerializeField] KeyCode _addMod3 = KeyCode.Alpha3;
    [SerializeField] KeyCode _addMod4 = KeyCode.Alpha4;
    [SerializeField] KeyCode _addMod5 = KeyCode.Alpha5;
    [SerializeField] KeyCode _addMod6 = KeyCode.Alpha6;
    [SerializeField] KeyCode _addMod7 = KeyCode.Alpha7;
    [SerializeField] KeyCode _addMod8 = KeyCode.Alpha8;
    [SerializeField] KeyCode _addMod9 = KeyCode.Alpha9;
    [SerializeField] KeyCode _removeMod0 = KeyCode.Keypad0;
    [SerializeField] KeyCode _removeMod1 = KeyCode.Keypad1;
    [SerializeField] KeyCode _removeMod2 = KeyCode.Keypad2;
    [SerializeField] KeyCode _removeMod3 = KeyCode.Keypad3;
    [SerializeField] KeyCode _removeMod4 = KeyCode.Keypad4;
    [SerializeField] KeyCode _removeMod5 = KeyCode.Keypad5;
    [SerializeField] KeyCode _removeMod6 = KeyCode.Keypad6;
    [SerializeField] KeyCode _removeMod7 = KeyCode.Keypad7;
    [SerializeField] KeyCode _removeMod8 = KeyCode.Keypad8;
    [SerializeField] KeyCode _removeMod9 = KeyCode.Keypad9;

    [SerializeField] List<StatModifier[]> _statModifiers = new List<StatModifier[]>(9);

    private readonly object[] _statModifiersKey = new object[9];

    private Experience _exp;
    private Status _status;

    void Awake()
    {
        _exp = _player.GetComponent<Experience>();
        _status = _player.GetComponent<Status>();

        for(int i = 0; i < _statModifiersKey.Length; i++)
        {
            _statModifiersKey[i] = new object();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(_saveKey))
        {
            FindObjectOfType<SavingSystem>().SerializeGameData("test");
        }
        else if (Input.GetKeyDown(_loadKey))
        {
            StartCoroutine(FindObjectOfType<SavingSystem>().DeserializeGameData("test"));
        }

        if (Input.GetKeyDown(_gainExpKey))
        {
            _exp.GainExperience(100);
        }

        if(Input.GetKeyDown(_addMod0))
        {
            _status.AddModifiers(_statModifiersKey[0], _statModifiers[0]);
        }
        if (Input.GetKeyDown(_addMod1))
        {
            _status.AddModifiers(_statModifiersKey[1], _statModifiers[1]);
        }
        if (Input.GetKeyDown(_addMod2))
        {
            _status.AddModifiers(_statModifiersKey[2], _statModifiers[2]);
        }
        if (Input.GetKeyDown(_addMod3))
        {
            _status.AddModifiers(_statModifiersKey[3], _statModifiers[3]);
        }
        if (Input.GetKeyDown(_addMod4))
        {
            _status.AddModifiers(_statModifiersKey[4], _statModifiers[4]);
        }
        if (Input.GetKeyDown(_addMod5))
        {
            _status.AddModifiers(_statModifiersKey[5], _statModifiers[5]);
        }
        if (Input.GetKeyDown(_addMod6))
        {
            _status.AddModifiers(_statModifiersKey[6], _statModifiers[6]);
        }
        if (Input.GetKeyDown(_addMod7))
        {
            _status.AddModifiers(_statModifiersKey[7], _statModifiers[7]);
        }
        if (Input.GetKeyDown(_addMod8))
        {
            _status.AddModifiers(_statModifiersKey[8], _statModifiers[8]);
        }
        if (Input.GetKeyDown(_addMod9))
        {
            _status.AddModifiers(_statModifiersKey[9], _statModifiers[9]);
        }


        if (Input.GetKeyDown(_removeMod0))
        {
            _status.RemoveModifiers(_statModifiersKey[0]);
        }
        if (Input.GetKeyDown(_removeMod1))
        {
            _status.RemoveModifiers(_statModifiersKey[1]);
        }
        if (Input.GetKeyDown(_removeMod2))
        {
            _status.RemoveModifiers(_statModifiersKey[2]);
        }
        if (Input.GetKeyDown(_removeMod3))
        {
            _status.RemoveModifiers(_statModifiersKey[3]);
        }
        if (Input.GetKeyDown(_removeMod4))
        {
            _status.RemoveModifiers(_statModifiersKey[4]);
        }
        if (Input.GetKeyDown(_removeMod5))
        {
            _status.RemoveModifiers(_statModifiersKey[5]);
        }
        if (Input.GetKeyDown(_removeMod6))
        {
            _status.RemoveModifiers(_statModifiersKey[6]);
        }
        if (Input.GetKeyDown(_removeMod7))
        {
            _status.RemoveModifiers(_statModifiersKey[7]);
        }
        if (Input.GetKeyDown(_removeMod8))
        {
            _status.RemoveModifiers(_statModifiersKey[8]);
        }
        if (Input.GetKeyDown(_removeMod9))
        {
            _status.RemoveModifiers(_statModifiersKey[9]);
        }
    }

}
