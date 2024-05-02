
namespace SLOTC.Utils.UI.Dragging
{
    public interface IDragSource<T> where T : class
    {
        public T GetItem();
        public int GetQty();
        public void RemoveItems(int qty);
    }
}