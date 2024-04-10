using SLOTC.Core.Inventory;
using SLOTC.Core.Saving;
using SLOTC.Core.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDebug : MonoBehaviour
{
    [SerializeField] EquipableItem _weapon;
    [SerializeField] EquipableItem _accessory;
    [SerializeField] EquipableItem _pendant;
    [SerializeField] EquipableItem _head;
    [SerializeField] EquipableItem _body;
    [SerializeField] EquipableItem _shoes;

    [SerializeField] GameObject _player;

    [SerializeField] KeyCode _gainExpKey = KeyCode.X;
    [SerializeField] KeyCode _addEquipment1 = KeyCode.Alpha1;
    [SerializeField] KeyCode _addEquipment2 = KeyCode.Alpha2;
    [SerializeField] KeyCode _addEquipment3 = KeyCode.Alpha3;
    [SerializeField] KeyCode _addEquipment4 = KeyCode.Alpha4;
    [SerializeField] KeyCode _addEquipment5 = KeyCode.Alpha5;
    [SerializeField] KeyCode _addEquipment6 = KeyCode.Alpha6;
    [SerializeField] KeyCode _removeEquipment1 = KeyCode.Keypad1;
    [SerializeField] KeyCode _removeEquipment2 = KeyCode.Keypad2;
    [SerializeField] KeyCode _removeEquipment3 = KeyCode.Keypad3;
    [SerializeField] KeyCode _removeEquipment4 = KeyCode.Keypad4;
    [SerializeField] KeyCode _removeEquipment5 = KeyCode.Keypad5;
    [SerializeField] KeyCode _removeEquipment6 = KeyCode.Keypad6;

    [SerializeField] List<StatModifier[]> _statModifiers = new List<StatModifier[]>(9);

    private readonly object[] _statModifiersKey = new object[9];

    private Experience _exp;
    private Equipment _equipment;

    void Awake()
    {
        _exp = _player.GetComponent<Experience>();
        _equipment = _player.GetComponent<Equipment>();

        for(int i = 0; i < _statModifiersKey.Length; i++)
        {
            _statModifiersKey[i] = new object();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(_gainExpKey))
        {
            _exp.GainExperience(100);
        }

        if (Input.GetKeyDown(_addEquipment1))
        {
            _equipment.Equip(_weapon);
        }
        if (Input.GetKeyDown(_addEquipment2))
        {
            _equipment.Equip(_accessory);
        }
        if (Input.GetKeyDown(_addEquipment3))
        {
            _equipment.Equip(_pendant);
        }
        if (Input.GetKeyDown(_addEquipment4))
        {
            _equipment.Equip(_head);
        }
        if (Input.GetKeyDown(_addEquipment5))
        {
            _equipment.Equip(_body);
        }
        if (Input.GetKeyDown(_addEquipment6))
        {
            _equipment.Equip(_shoes);
        }

        if (Input.GetKeyDown(_removeEquipment1))
        {
            _equipment.Unequip(EquipLocation.Weapon);
        }
        if (Input.GetKeyDown(_removeEquipment2))
        {
            _equipment.Unequip(EquipLocation.Accessory);
        }
        if (Input.GetKeyDown(_removeEquipment3))
        {
            _equipment.Unequip(EquipLocation.Pendant);
        }
        if (Input.GetKeyDown(_removeEquipment4))
        {
            _equipment.Unequip(EquipLocation.Head);
        }
        if (Input.GetKeyDown(_removeEquipment5))
        {
            _equipment.Unequip(EquipLocation.Body);
        }
        if (Input.GetKeyDown(_removeEquipment6))
        {
            _equipment.Unequip(EquipLocation.Shoes);
        }
    }

}
