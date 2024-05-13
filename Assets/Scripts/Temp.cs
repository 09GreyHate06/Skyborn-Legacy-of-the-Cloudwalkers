using SLOTC.Core.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Temp : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // temp
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            FindObjectOfType<SavingSystem>().SerializeGameData("test");
        }
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            StartCoroutine(FindObjectOfType<SavingSystem>().DeserializeGameData("test"));
        }
    }
}
