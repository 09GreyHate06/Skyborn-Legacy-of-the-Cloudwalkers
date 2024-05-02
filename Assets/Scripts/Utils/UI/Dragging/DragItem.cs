using UnityEngine;
using UnityEngine.EventSystems;

namespace SLOTC.Utils.UI.Dragging
{
    public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler where T : class
    {
        private Vector3 _startPos;
        private Transform _ogParent;
        private IDragSource<T> _source;
        private Canvas _parentCanvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _parentCanvas = GetComponentInParent<Canvas>();
            _source = GetComponentInParent<IDragSource<T>>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPos = transform.position;
            _ogParent = transform.parent;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0.6f;
            transform.SetParent(_parentCanvas.transform, true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _parentCanvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.position = _startPos;
            _canvasGroup.alpha = 1.0f;
            transform.SetParent(_ogParent, true);

            IDragDestination<T> destination;
            if (eventData.pointerEnter)
            {
                destination = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();
            }
            else
            {
                destination = _ogParent.GetComponent<IDragDestination<T>>();
            }

            if (destination != null)
            {
                DropItemIntoSlot(destination);
            }

            _canvasGroup.blocksRaycasts = true;
        }

        private void DropItemIntoSlot(IDragDestination<T> destination)
        {
            if (object.ReferenceEquals(destination, _source)) return;

            var destinationSlot = destination as IDragSlot<T>;
            var sourceSlot = _source as IDragSlot<T>;

            if (destinationSlot == null || sourceSlot == null || destinationSlot.GetItem() == null ||
                object.ReferenceEquals(destinationSlot.GetItem(), sourceSlot.GetItem()))
            {
                AttempSimpleTransfer(destination);
                return;
            }

            AttempSwap(destinationSlot, sourceSlot);
        }

        private bool AttempSimpleTransfer(IDragDestination<T> destination)
        {
            var item = _source.GetItem();
            int qty = _source.GetQty();

            int acceptable = destination.MaxAcceptable(item);
            int toTransfer = Mathf.Min(acceptable, qty);

            if (toTransfer > 0)
            {
                _source.RemoveItems(toTransfer);
                destination.AddItems(item, toTransfer);
                return true;
            }

            return false;
        }

        private void AttempSwap(IDragSlot<T> destinationSlot, IDragSlot<T> sourceSlot)
        {
            var removedSourceQty = sourceSlot.GetQty();
            var removedSourceItem = sourceSlot.GetItem();
            var removedDestinationQty = destinationSlot.GetQty();
            var removedDestinationItem = destinationSlot.GetItem();

            sourceSlot.RemoveItems(removedSourceQty);
            destinationSlot.RemoveItems(removedDestinationQty);

            int sourceTakeBackQty = CalculateTakeBack(removedSourceItem, removedSourceQty, sourceSlot, destinationSlot);
            int destinationTakeBackQty = CalculateTakeBack(removedDestinationItem, removedDestinationQty, destinationSlot, sourceSlot);

            if (sourceTakeBackQty > 0)
            {
                sourceSlot.AddItems(removedSourceItem, sourceTakeBackQty);
                removedSourceQty -= sourceTakeBackQty;
            }
            if (destinationTakeBackQty > 0)
            {
                destinationSlot.AddItems(removedDestinationItem, destinationTakeBackQty);
                removedDestinationQty -= destinationTakeBackQty;
            }


            if (sourceSlot.MaxAcceptable(removedDestinationItem) < removedDestinationQty ||
                destinationSlot.MaxAcceptable(removedSourceItem) < removedSourceQty)
            {
                destinationSlot.AddItems(removedDestinationItem, removedDestinationQty);
                sourceSlot.AddItems(removedSourceItem, removedSourceQty);
                return;
            }

            if (removedDestinationQty > 0)
            {
                sourceSlot.AddItems(removedDestinationItem, removedDestinationQty);
            }
            if (removedSourceQty > 0)
            {
                destinationSlot.AddItems(removedSourceItem, removedSourceQty);
            }
        }

        private int CalculateTakeBack(T removedItem, int removedQty, IDragSlot<T> removeSource, IDragSlot<T> destination)
        {
            int takeBackQty = 0;
            var destinationMaxAcceptable = destination.MaxAcceptable(removedItem);

            if (destinationMaxAcceptable < removedQty)
            {
                takeBackQty = removedQty - destinationMaxAcceptable;
                var sourceTakeBackAcceptable = removeSource.MaxAcceptable(removedItem);

                if (sourceTakeBackAcceptable < takeBackQty)
                {
                    return 0;
                }
            }

            return takeBackQty;
        }
    }
}