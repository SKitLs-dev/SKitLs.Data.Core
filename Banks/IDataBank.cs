using SKitLs.Data.Core.Core;
using SKitLs.Data.Core.IO;
using IDataReader = SKitLs.Data.Core.IO.IDataReader;

namespace SKitLs.Data.Core.Banks
{
    public delegate void DataBankUpdatedHandler();
    public delegate void DataBankCollectionUpdatedHandler(int affectedRows);
    public delegate void DataBankCollectionUpdatedHandler<TId, TData>(IEnumerable<TData> updatedElements) where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>;

    public enum DropStrategy
    {
        Disable,
        Delete
    }

    public interface IDataBank
    {
        public event DataBankUpdatedHandler? OnBankInfoUpdated;
        public event DataBankCollectionUpdatedHandler? OnBankDataUpdated;

        public string Id { get; }

        public Type InType { get; }

        public string? Name { get; }

        public string? Description { get; }

        public long Count { get; }

        public DropStrategy DropStrategy { get; }

        public IDataReader? Reader { get; }

        public IDataWriter? Writer { get; }

        public void Initialize();
        public Task InitializeAsync();

        public object BuildNewObject();

        public void AddNSave<T>(T data) where T : class;
        public void AddNSave<T>(IEnumerable<T> dataCollection) where T : class;

        public void DropNSave(object data);

        public IReadOnlyList<object> GetReadonlyObjects();
    }
}