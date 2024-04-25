namespace SLOTC.Core.Stats
{
    public interface IStatModifier
    {
        public string GetID();
        public StatModifier[] GetStatModifiers();
    }
}
