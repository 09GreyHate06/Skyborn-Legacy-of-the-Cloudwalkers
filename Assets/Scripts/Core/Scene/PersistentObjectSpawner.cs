using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLOTC.Core.Scene
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject _persistentObjectPrefab;

        private static bool s_hasSpawned = false;

        private void Awake()
        {
            if (s_hasSpawned) return;

            SpawnPersistentObjects();

            s_hasSpawned = true;
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = Instantiate(_persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }

}
