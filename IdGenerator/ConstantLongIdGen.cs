using SKitLs.Data.Core.Banks;
using SKitLs.Data.Core.Core;
using SKitLs.Utils.Extensions.Lists;

namespace SKitLs.Data.Core.IdGenerator
{
    public class ConstantLongIdGen : IIdGenerator<long>
    {
        private Random Random { get; set; }

        private long DefaultId { get; set; }

        private long StartId { get; set; }

        public long GetDefaultId() => DefaultId;
        public bool IsDefaultID(long id) => id == DefaultId;

        public ConstantLongIdGen(long defaultId = -1, int startId = 1, int? seed = null)
        {
            DefaultId = defaultId;
            StartId = startId;
            Random = seed is not null ? new Random(seed.Value) : new Random();
        }

        public long GenerateId() => Random.NextInt64();

        // TODO !
        public long GenerateIdFor<TData>(IDataBank<long, TData> bank) where TData : ModelDso<long>
        {
            return bank.GetReadonlyData().Select(x => x.GetId()).FirstAvailableValue((int)StartId);
        }
    }
}