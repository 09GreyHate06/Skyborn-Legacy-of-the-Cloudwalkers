using SLOTC.Core.Saving;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SLOTC.Core.Stats
{
    [Serializable]
    public struct Attribute
    {
        public AttributeType type;
        public int value;
    }

    public class Attributes : MonoBehaviour, ISaveable
    {
        [SerializeField] Attribute[] _attributes = new Attribute[5];
        [SerializeField] int _attributePointsGainPerLvl;
        [SerializeField] UnityEvent _onAttributesChanged;

        private Experience _exp;

        public UnityEvent OnAttributesChanged { get { return _onAttributesChanged; } }
        public int AttributePoints { get; private set; } = 0;

        private void Awake()
        {
            TryGetComponent<Experience>(out _exp);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_attributes.Length != 5)
                _attributes = new Attribute[5];

            for (int i = 0; i < _attributes.Length; i++)
            {
                _attributes[i].type = (AttributeType)i;
            }

            _onAttributesChanged?.Invoke();
        }
#endif

        private void OnEnable()
        {
            if (_exp)
                _exp.OnLevelUp += OnLevelUp;
        }

        private void OnDisable()
        {
            if (_exp)
                _exp.OnLevelUp -= OnLevelUp;
        }

        public int GetAttribute(AttributeType attributeType)
        {
            return _attributes.First(x => x.type == attributeType).value;
        }

        public void AddPoints(AttributeType type, int points)
        {
            if (points > AttributePoints) return;

            for (int i = 0; i < _attributes.Length; i++)
            {
                if (_attributes[i].type != type) continue;

                _attributes[i].value += points;
                AttributePoints -= points;
            }

            _onAttributesChanged?.Invoke();
        }

        private void OnLevelUp(int curLevel)
        {
            AttributePoints += _attributePointsGainPerLvl;
        }

        public object CaptureState()
        {
            object[] state = new object[2];
            state[0] = _attributes;
            state[1] = AttributePoints;

            return state;
        }

        public void RestoreState(object state)
        {
            object[] state_ = (object[])state;
            _attributes = (Attribute[])state_[0];
            AttributePoints = (int)state_[1];
            _onAttributesChanged?.Invoke();
        }
    }
}
