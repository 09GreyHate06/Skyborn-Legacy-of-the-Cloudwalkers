using UnityEditor;
using UnityEngine;

namespace SLOTC.Core.Dialogue.Editor
{
    [CustomEditor(typeof(Dialogue)), CanEditMultipleObjects]
    public class DialogueEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            if (GUILayout.Button("Create Node"))
            {
                Dialogue dialogue = (Dialogue)target;

                Undo.RecordObject(dialogue, "Created Dialogue Node");
                DialogueNode newNode = dialogue.CreateNode(null);
                Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");

                EditorUtility.SetDirty(dialogue);
            }

            EditorGUI.EndChangeCheck();
        }
    }
}