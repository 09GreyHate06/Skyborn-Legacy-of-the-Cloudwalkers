using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SLOTC.Core.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        private Dictionary<string, object> _gameStates = new Dictionary<string, object>();

        public void StoreGameData()
        {
            CaptureState(_gameStates);
        }

        public void LoadGameData()
        {
            RestoreState(_gameStates);
        }

        /// <summary>
        /// Save game states to a file.
        /// </summary>
        /// <param name="fileName"></param>
        public void SerializeGameData(string filename)
        {
            CaptureState(_gameStates);
            _gameStates["LastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
            SaveFile(filename, _gameStates);
        }

        /// <summary>
        /// Load game states from a file.
        /// </summary>
        /// <param name="fileName"></param>
        public IEnumerator DeserializeGameData(string filename)
        {
            _gameStates = LoadFile(filename);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;

            if (_gameStates.ContainsKey("LastSceneBuildIndex"))
            {
                buildIndex = (int)_gameStates["LastSceneBuildIndex"];
            }

            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreState(_gameStates);
        }

        public bool SaveExists(string filename)
        {
            string filename_ = GetPathFromSaveFile(filename);
            return File.Exists(filename_);
        }

        public bool DeleteSaveFile(string filename)
        {
            string filename_ = GetPathFromSaveFile(filename);

            if (!File.Exists(filename_)) return false;

            File.Delete(filename_);
            return true;
        }

        private void CaptureState(Dictionary<string, object> states)
        {
            foreach(SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                states[saveable.UniqueIdentifier] = saveable.CaptureState();
            }
        }

        private void RestoreState(Dictionary<string, object> states)
        {
            foreach(SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.UniqueIdentifier;
                if (!states.ContainsKey(id)) 
                    continue;

                saveable.RestoreState(states[id]);
            }
        }

        private void SaveFile(string filename, object states)
        {
            string filename_ = GetPathFromSaveFile(filename);

            Debug.Log("Saving to " + filename_);
            using (FileStream stream = File.Open(filename_, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, states);
            }
        }

        private Dictionary<string, object> LoadFile(string filename)
        {
            string filename_ = GetPathFromSaveFile(filename);

            if (!File.Exists(filename_))
            {
                return new Dictionary<string, object>();
            }

            Debug.Log("Loading from " + filename_);
            using (FileStream stream = File.Open(filename_, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private string GetPathFromSaveFile(string filename)
        {
            return Path.Combine(Application.persistentDataPath, filename + ".sav");
        }
    }
}
