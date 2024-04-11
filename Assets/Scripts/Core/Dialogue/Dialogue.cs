
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SLOTC.Core.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] List<DialogueNode> _nodes = new List<DialogueNode>();

#if UNITY_EDITOR
        /// <summary>
        /// editor only
        /// </summary>
        [SerializeField] Vector2 _nodeCreationPositionOffset = new Vector2(250, 0);
#endif

        // dictionary is not serializable
        private Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

        public IEnumerable<DialogueNode> Nodes { get { return _nodes; } }
        public DialogueNode RootNode { get { return _nodes[0]; } }

        private void Awake()
        {
            UpdateLookup();
        }

        private void OnValidate()
        {
            UpdateLookup();
        }

        public IEnumerable<DialogueNode> GetChildrenOfNode(DialogueNode parentNode)
        {
            foreach (string childNodeID in parentNode.Children)
            {
                if (_nodeLookup.ContainsKey(childNodeID))
                    yield return _nodeLookup[childNodeID];

                //foreach (DialogueNode node in Nodes)
                //{
                //    if (node.name != childNodeID) continue;
                //
                //    yield return node;
                //    break;
                //}
            }
        }

        public IEnumerable<DialogueNode> GetChildrenOfNode(DialogueNode parentNode, SpeakerType speaker)
        {
            foreach (string childNodeID in parentNode.Children)
            {
                if (_nodeLookup.ContainsKey(childNodeID) && _nodeLookup[childNodeID].Speaker == speaker)
                    yield return _nodeLookup[childNodeID];

                //foreach (DialogueNode node in Nodes)
                //{
                //    if (node.name != childNodeID || node.Speaker != speaker) continue;
                //
                //    yield return node;
                //    break;
                //}
            }
        }

        public IEnumerable<DialogueNode> GetParentsOfNode(DialogueNode childNode)
        {
            foreach (DialogueNode node in Nodes)
            {
                if (!node.Children.Contains(childNode.name)) continue;

                yield return node;
            }
        }

        private void UpdateLookup()
        {
            _nodeLookup.Clear();

            foreach (DialogueNode node in Nodes)
            {
                _nodeLookup[node.name] = node;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// editor only
        /// </summary>
        public DialogueNode CreateNode(DialogueNode parent)
        {
            DialogueNode node = CreateInstance<DialogueNode>();
            node.name = Guid.NewGuid().ToString();
            if (parent != null)
            {
                parent.AddChild(node.name);
                node.Rect.position = parent.Rect.position + _nodeCreationPositionOffset;
            }
            _nodes.Add(node);

            UpdateLookup();

            return node;
        }

        /// <summary>
        /// editor only
        /// returns: parent nodes
        /// </summary>
        public void DeleteNode(DialogueNode nodeToDelete)
        {

            _nodes.Remove(nodeToDelete);
            foreach (DialogueNode node in Nodes)
            {
                node.RemoveChild(nodeToDelete.name);
            }
            UpdateLookup();
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this))) return;

            foreach (DialogueNode node in Nodes)
            {
                // todo why node is null when undoing in window
                if (!node || !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(node))) continue;

                AssetDatabase.AddObjectToAsset(node, this);
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}