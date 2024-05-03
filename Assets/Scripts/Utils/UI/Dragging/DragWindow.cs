using UnityEngine;
using UnityEngine.EventSystems;

namespace SLOTC.Utils.UI
{
    public class DragWindow : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        [SerializeField] RectTransform _target;
        [SerializeField, Tooltip("For alpha")] CanvasGroup _targetCanvasGroup;

        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _target.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            _targetCanvasGroup.alpha = 0.6f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _target.SetAsLastSibling();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _targetCanvasGroup.alpha = 1.0f;
        }
    }
}
