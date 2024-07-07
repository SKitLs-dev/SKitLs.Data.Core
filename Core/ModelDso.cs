namespace SKitLs.Data.Core.Core
{
    public abstract class ModelDso<TId> : IEquatable<ModelDso<TId>> where TId : notnull, IEquatable<TId>, IComparable<TId>
    {
        public event EventHandler? OnSaveRequested;

        public bool IsEnabled { get; private set; } = true;
        public void Enable() => IsEnabled = true;
        public void Disable() => IsEnabled = false;

        public abstract TId GetId();
        public abstract void SetId(TId id);

        public void Save() => OnSaveRequested?.Invoke(this, EventArgs.Empty);

        public override int GetHashCode() => GetId().GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is ModelDso<TId> modelDso)
                return Equals(modelDso);
            return false;
        }

        public bool Equals(ModelDso<TId>? other)
        {
            if (other?.GetId().Equals(GetId()) ?? false)
                return true;
            return false;
        }
    }
}