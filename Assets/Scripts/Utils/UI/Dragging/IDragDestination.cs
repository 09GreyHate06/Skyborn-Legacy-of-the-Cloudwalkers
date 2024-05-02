
namespace SLOTC.Utils.UI.Dragging
{
    public interface IDragDestination<T> where T : class
    {
        public int MaxAcceptable(T item);
        public void AddItems(T item, int qty);
    }
}