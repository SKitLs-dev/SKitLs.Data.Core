using SKitLs.Data.Core.Banks;
using SKitLs.Data.Core.Core;

namespace SKitLs.Data.Core.IdGenerator
{
    public interface IIdGenerator<TId> where TId : notnull, IEquatable<TId>, IComparable<TId>
    {
        public TId GetDefaultId();

        public bool IsDefaultID(TId id);

        public TId GenerateId();

        public TId GenerateIdFor<TData>(IDataBank<TId, TData> bank)where TData : ModelDso<TId>;
    }
}