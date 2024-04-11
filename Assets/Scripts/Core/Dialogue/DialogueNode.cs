using System.Collections.Generic;
using UnityEngine;

namespace SLOTC.Core.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] SpeakerType _speaker;
        [SerializeField] string _text;
        [SerializeField] List<string> _children = new List<string>();


        public SpeakerType Speaker { get { return _speaker; } }
        public IEnumerable<string> Children { get { return _children; } }
        public string Text { get { return _text; } }

#if UNITY_EDITOR
        /// <summary>
        /// editor only
        /// </summary>
        public Rect Rect = new Rect(0, 0, 200, 100);

        /// <summary>
        /// editor only
        /// </summary>
        public void AddChild(string name)
        {
            _children.Add(name);
        }

        /// <summary>
        /// editor only
        /// </summary>
        public void RemoveChild(string name)
        {
            _children.Remove(name);
        }

        /// <summary>
        /// editor only
        /// </summary>
        public void SetText(string text)
        {
            _text = text;
        }
#endif
    }
}

