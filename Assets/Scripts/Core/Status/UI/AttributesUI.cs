
using TMPro;
using UnityEngine;
using SLOTC.Utils;
using UnityEngine.InputSystem;

namespace SLOTC.Core.Stats.UI
{
    public class AttributesUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _strValueTxt;
        [SerializeField] TextMeshProUGUI _intValueTxt;
        [SerializeField] TextMeshProUGUI _dexValueTxt;
        [SerializeField] TextMeshProUGUI _vitValueTxt;
        [SerializeField] TextMeshProUGUI _lukValueTxt;
        [SerializeField] TextMeshProUGUI _pointsValueTxt;

        private Attributes _attributes;

        private void Awake()
        {
            _attributes = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<Attributes>();
        }

        private void OnEnable()
        {
            _attributes.OnAttributesChanged.AddListener(UpdateValues);
            UpdateValues();
        }

        private void OnDisable()
        {
            _attributes.OnAttributesChanged.RemoveListener(UpdateValues);
        }

        private void Update()
        {
            _pointsValueTxt.text = _attributes.AttributePoints.ToString();
        }

        public void AddPointToSTR()
        {
            _attributes.AddPoints(AttributeType.Strength, 1);
        }

        public void AddPointToINT()
        {
            _attributes.AddPoints(AttributeType.Intelligence, 1);
        }

        public void AddPointToDEX()
        {
            _attributes.AddPoints(AttributeType.Dexterity, 1);
        }

        public void AddPointToVIT()
        {
            _attributes.AddPoints(AttributeType.Vitality, 1);
        }

        public void AddPointToLUK()
        {
            _attributes.AddPoints(AttributeType.Luck, 1);
        }

        public void UpdateValues()
        {
            //if (!isActiveAndEnabled) return; // because unity event calls this even when disabled

            _strValueTxt.text = _attributes.GetAttribute(AttributeType.Strength).ToString();
            _intValueTxt.text = _attributes.GetAttribute(AttributeType.Intelligence).ToString();
            _dexValueTxt.text = _attributes.GetAttribute(AttributeType.Dexterity).ToString();
            _vitValueTxt.text = _attributes.GetAttribute(AttributeType.Vitality).ToString();
            _lukValueTxt.text = _attributes.GetAttribute(AttributeType.Luck).ToString();
        }
    }
}