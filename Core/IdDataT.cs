namespace SKitLs.Data.Core.Core
{
    public class IdData<TId, TData>(TData data) where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
    {
        public TId Id { get; init; } = data.GetId();
        public TData Value { get; init; } = data ?? throw new ArgumentNullException(nameof(data));

        public bool IsActive { get; private set; } = true;
        public void Enable() => IsActive = true;
        public void Disable() => IsActive = false;

        public override string ToString() => $"{Id} - {Value}";
    }
}