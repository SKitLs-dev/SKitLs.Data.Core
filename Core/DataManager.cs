using System.Runtime.InteropServices;
using SKitLs.Data.Core.Banks;

namespace SKitLs.Data.Core.Core
{
    public interface IDataManager
    {
        public List<DataBankInfo> GetNotations();

        public List<IDataBank> GetBanks();
        public IDataBank ResolveBank(Type bankType);
        public IDataBank ResolveBank<TData>();
        public IDataBank<TId, TData> ResolveBank<TId, TData>() where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>;

        public void Add<TId, TData>(IDataBank<TId, TData> bank) where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>;

        public void Initialize();
        public Task InitializeAsync();
    }

    public class DataManager : IDataManager
    {
        private List<IDataBank> Banks { get; set; } = [];
        private List<DataBankInfo> Notations { get; set; } = [];

        public List<IDataBank> GetBanks() => Banks;
        public IDataBank ResolveBank(Type bankType) => Banks.Find(x => x.InType == bankType) ?? throw new Exception();
        public IDataBank ResolveBank<TData>() => ResolveBank(typeof(TData));
        public IDataBank<TId, TData> ResolveBank<TId, TData>() where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
            => (IDataBank<TId, TData>?)Banks.Find(x => x.InType == typeof(TData)) ?? throw new Exception();

        public List<DataBankInfo> GetNotations() => Notations;

        public void Add<TId, TData>(IDataBank<TId, TData> bank) where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
        {
            var bankInfo = DataBankInfo.OfDataBank(bank);
            bank.OnBankDataUpdated += (cnt) => bankInfo.Count += cnt;

            Banks.Add(bank);
            Notations.Add(bankInfo);
        }

        public void Initialize()
        {
            foreach (var bank in GetBanks())
            {
                bank.Initialize();
            }
        }
        public async Task InitializeAsync()
        {
            foreach (var bank in GetBanks())
            {
                await bank.InitializeAsync();
            }
        }
    }
}