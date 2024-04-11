using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace SLOTC.Core.Dialogue.Editor
{
    public class DialogueEditorWindow : EditorWindow
    {
        private Dialogue _selectedDialogue;
        private Vector2 _scrollViewPos;
        private const float _scrollViewSize = 4000.0f;

        // editor window automatically serialized properties
        [NonSerialized] DialogueNode _draggingNode;
        [NonSerialized] Vector2 _draggingNodeOffset;
        [NonSerialized] DialogueNode _nodeToBeCreatedParent;
        [NonSerialized] DialogueNode _nodeToDelete;
        [NonSerialized] DialogueNode _nodeToLink;

        [NonSerialized] bool _draggingScrollView;
        [NonSerialized] Vector2 _draggingScrollViewOffset;

        [NonSerialized] GUIStyle[] _nodeStyles;

        [NonSerialized] Texture2D _backgroundTex;
        [NonSerialized] Texture2D _arrowTex;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow<DialogueEditorWindow>(false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instancename, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instancename) as Dialogue;

            if (dialogue)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            _nodeStyles = new GUIStyle[Enum.GetNames(typeof(SpeakerType)).Length];
            for (int i = 0; i < _nodeStyles.Length; i++)
            {
                _nodeStyles[i] = new GUIStyle();
                _nodeStyles[i].normal.background = EditorGUIUtility.Load("node" + i) as Texture2D;
                _nodeStyles[i].padding = new RectOffset(20, 20, 20, 20);
                _nodeStyles[i].border = new RectOffset(12, 12, 12, 12);
            }

            _backgroundTex = Resources.Load("background") as Texture2D;
            _arrowTex = Resources.Load("arrow") as Texture2D;
        }

        private void OnDisable()
        {
        }

        private void OnSelectionChange()
        {
            Dialogue dialogue = Selection.activeObject as Dialogue;
            if (dialogue)
            {
                _selectedDialogue = dialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (!_selectedDialogue) return;

            ProcessEvents();

            _scrollViewPos = EditorGUILayout.BeginScrollView(_scrollViewPos);
            Rect scrollViewRect = GUILayoutUtility.GetRect(_scrollViewSize, _scrollViewSize);

            Rect texCoords = new Rect(0, 0, _scrollViewSize / _backgroundTex.width, _scrollViewSize / _backgroundTex.height);
            GUI.DrawTextureWithTexCoords(scrollViewRect, _backgroundTex, texCoords);

            foreach (DialogueNode node in _selectedDialogue.Nodes)
            {
                DrawLinks(node);
            }
            foreach (DialogueNode node in _selectedDialogue.Nodes)
            {
                DrawNode(node);
            }

            EditorGUILayout.EndScrollView();

            if (_nodeToBeCreatedParent != null)
            {
                // record parent for link
                Undo.RecordObjects(new UnityEngine.Object[] { _selectedDialogue, _nodeToBeCreatedParent }, "Created Dialogue Node");
                DialogueNode newNode = _selectedDialogue.CreateNode(_nodeToBeCreatedParent);
                Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");

                // undo. func doesn't set sub asset dirty
                EditorUtility.SetDirty(_nodeToBeCreatedParent);

                _nodeToBeCreatedParent = null;
            }

            if (_nodeToDelete != null)
            {
                var parents = _selectedDialogue.GetParentsOfNode(_nodeToDelete);

                var rec = parents.ToList<UnityEngine.Object>();
                rec.Add(_selectedDialogue);

                Undo.RecordObjects(rec.ToArray(), "Deleted Dialogue Node");
                _selectedDialogue.DeleteNode(_nodeToDelete);

                // undo. func doesn't set sub asset dirty
                foreach (DialogueNode node in parents)
                {
                    EditorUtility.SetDirty(node);
                }

                Undo.DestroyObjectImmediate(_nodeToDelete);

                _nodeToDelete = null;
            }
        }

        public void ProcessEvents()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollViewPos);
                    if (_draggingNode != null)
                    {
                        _draggingNodeOffset = Event.current.mousePosition - _draggingNode.Rect.position;
                        Selection.activeObject = _draggingNode;
                    }
                    else
                    {
                        _draggingScrollView = true;
                        _draggingScrollViewOffset = Event.current.mousePosition + _scrollViewPos;
                        Selection.activeObject = _selectedDialogue;
                    }
                    break;

                case EventType.MouseDrag:
                    if (_draggingNode != null)
                    {
                        Undo.RecordObject(_draggingNode, "Updated Dialogue Node Position");
                        _draggingNode.Rect.position = Event.current.mousePosition - _draggingNodeOffset;
                        // undo. func doesn't set sub asset dirty
                        EditorUtility.SetDirty(_draggingNode);
                        GUI.changed = true;
                    }
                    else if (_draggingScrollView)
                    {
                        _scrollViewPos = -Event.current.mousePosition + _draggingScrollViewOffset;
                        GUI.changed = true;
                    }
                    break;

                case EventType.MouseUp:
                    _draggingNode = null;
                    _draggingScrollView = false;
                    break;
            }
        }

        private void DrawLinks(DialogueNode node)
        {
            Vector3 startPos = node.Rect.center + (Vector2.right * node.Rect.size.x / 2);
            foreach (DialogueNode childNode in _selectedDialogue.GetChildrenOfNode(node))
            {
                Vector3 endPos = childNode.Rect.center - (Vector2.right * childNode.Rect.size.x / 2);
                Vector3 tangent = 0.8f * (endPos - startPos);
                tangent.y = 0;
                Handles.DrawBezier(startPos, endPos, startPos + tangent, endPos - tangent, Color.white, null, 4.0f);
                Rect arrowRect = new Rect(endPos, new Vector2(10.0f, 10.0f));
                arrowRect.position -= arrowRect.size / 2;
                GUI.DrawTexture(arrowRect, _arrowTex);
            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.Rect, _nodeStyles[(int)node.Speaker]);

            EditorGUILayout.LabelField(node.name);

            EditorGUI.BeginChangeCheck();
            string newText = EditorGUILayout.TextField(node.Text);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(node, "Updated Dialogue Text");
                node.SetText(newText);
                // undo. func doesn't set sub asset dirty
                EditorUtility.SetDirty(node);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                _nodeToBeCreatedParent = node;
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("x"))
            {
                _nodeToDelete = node;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (_nodeToLink == null)
            {
                if (GUILayout.Button("link"))
                {
                    _nodeToLink = node;
                }
            }
            else if (_nodeToLink == node)
            {
                if (GUILayout.Button("cancel"))
                {
                    _nodeToLink = null;
                }
            }
            else if (_nodeToLink.Children.Contains(node.name))
            {
                if (GUILayout.Button("-link"))
                {
                    Undo.RecordObject(_nodeToLink, "Unlink Dialogue Node");
                    _nodeToLink.RemoveChild(node.name);
                    // undo. func doesn't set sub asset dirty
                    EditorUtility.SetDirty(_nodeToLink);
                    _nodeToLink = null;
                }
            }
            else
            {
                if (GUILayout.Button("+link"))
                {
                    Undo.RecordObject(_nodeToLink, "Link Dialogue Node");
                    _nodeToLink.AddChild(node.name);
                    // undo. func doesn't set sub asset dirty
                    EditorUtility.SetDirty(_nodeToLink);
                    _nodeToLink = null;
                }
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            foreach (DialogueNode node in _selectedDialogue.Nodes.Reverse())
            {
                if (node.Rect.Contains(point))
                    return node;
            }

            return null;
        }
    }
}