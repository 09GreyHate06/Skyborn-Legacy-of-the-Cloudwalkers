
using UnityEngine;
using UnityEngine.EventSystems;

namespace SLOTC.Utils.UI.Tooltip
{
    public abstract class TooltipSpawner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] GameObject _tooltipPrefab;

        private GameObject _tooltip;

        public abstract void UpdateTooltip(GameObject tooltip);
        public abstract bool CanCreateTooltip();

        private void OnDestroy()
        {
            ClearTooltip();
        }

        private void OnDisable()
        {
            ClearTooltip();
        }

        private void ClearTooltip()
        {
            if (_tooltip)
            {
                Destroy(_tooltip.gameObject);
            }
        }

        private void PositionTooltip()
        {
            Canvas.ForceUpdateCanvases();

            Vector3[] tooltipCorners = new Vector3[4];
            _tooltip.GetComponent<RectTransform>().GetWorldCorners(tooltipCorners);
            Vector3[] slotCorners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(slotCorners);

            bool below = transform.position.y > Screen.height / 2;
            bool right = transform.position.x < Screen.width / 2;

            int slotCorner = GetCornerIndex(below, right);
            int tooltipCorner = GetCornerIndex(!below, !right);

            _tooltip.transform.position = slotCorners[slotCorner] - tooltipCorners[tooltipCorner] + _tooltip.transform.position;
        }

        private int GetCornerIndex(bool below, bool right)
        {
            if (below && !right) return 0;
            else if (!below && !right) return 1;
            else if (!below && right) return 2;
            else return 3;

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Canvas parentCanvas = GetComponentInParent<Canvas>();

            if (_tooltip && !CanCreateTooltip())
            {
                ClearTooltip();
            }

            if (!_tooltip && CanCreateTooltip())
            {
                _tooltip = Instantiate(_tooltipPrefab, parentCanvas.transform);
            }

            if (_tooltip)
            {
                UpdateTooltip(_tooltip);
                PositionTooltip();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ClearTooltip();
        }
    }
}