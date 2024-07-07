using SKitLs.Data.Core.Core;

namespace SKitLs.Data.Core.Banks
{
    public struct DataBankInfo(long count, Type inType, string? name = null, string? description = null)
    {
        public long Count { get; set; } = count;

        public Type InType { get; set; } = inType;

        public string? Name { get; set; } = name;

        public string? Description { get; set; } = description;

        internal static DataBankInfo OfDataBank<TId, TData>(IDataBank<TId, TData> bank) where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
        {
            return new(bank.Count, typeof(TData), bank.Name, bank.Description);
        }
    }
}