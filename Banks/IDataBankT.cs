using System.Collections;
using System.Data;
using SKitLs.Data.Core.Core;
using SKitLs.Data.Core.IO;

namespace SKitLs.Data.Core.Banks
{
    public interface IDataBank<TId, TData> : IDataBank, IEnumerable<IdData<TId, TData>> where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
    {
        public event DataBankCollectionUpdatedHandler<TId, TData>? OnBankDataAdded;
        public event DataBankCollectionUpdatedHandler<TId, TData>? OnBankDataRemoved;

        public TData BuildNewData();

        public void AddNSave(TData data);
        public void AddNSave(IEnumerable<TData> dataCollection);

        public void DropNSave(TData data);

        public int LoadData<TIn>(List<TIn> data);

        public IReadOnlyList<TData> GetReadonlyData();

        public IdData<TId, TData> GetData(TId id);
        public IdData<TId, TData> GetData(Func<TData, bool> predicate);

        public TData GetValue(TId id);
        public TData GetValue(Func<TData, bool> predicate);

        public bool Drop(Func<TData, bool> predicate);
        public int DropAll(Func<TData, bool> predicate);
    }
}