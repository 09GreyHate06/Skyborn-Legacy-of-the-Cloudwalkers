using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SLOTC.Core.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        // clear to refresh
        [SerializeField] string _uniqueIdentifier = "";
        private static Dictionary<string, SaveableEntity> s_globalLookup = new Dictionary<string, SaveableEntity>();

        public string UniqueIdentifier { get { return _uniqueIdentifier; } }

        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();

            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }

            return state;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, SaveableEntity> stateDict = new Dictionary<string, SaveableEntity>();

            foreach(ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if (stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDict[typeString]);
                }
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty uiProperty = serializedObject.FindProperty("_uniqueIdentifier");

            if(string.IsNullOrEmpty(uiProperty.stringValue) || !IsUnique(uiProperty.stringValue))
            {
                uiProperty.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            s_globalLookup[uiProperty.stringValue] = this;
        }
#endif

        private bool IsUnique(string value)
        {
            if(!s_globalLookup.ContainsKey(value) || s_globalLookup[value] == this) return true;

            if (s_globalLookup[value] == null)
            {
                s_globalLookup.Remove(value);
                return true;
            }

            if (s_globalLookup[value].UniqueIdentifier != value)
            {
                s_globalLookup.Remove(value);
                return true;
            }

            return false;
        }
    }
}
