using SKitLs.Data.Core.Banks;
using SKitLs.Data.IO;

namespace SKitLs.Data.Core
{
    /// <summary>
    /// Implementation of <see cref="IDataManager"/> that manages various data banks.
    /// </summary>
    public class DataManager : IDataManager
    {
        private List<DataBankInfo> Notations { get; set; } = [];

        /// <inheritdoc/>
        public IReadOnlyCollection<DataBankInfo> GetNotations() => Notations;

        private List<IDataBank> Banks { get; set; } = [];

        /// <inheritdoc/>
        public IEnumerable<IDataBank> GetBanks() => Banks;

        /// <inheritdoc/>
        /// <exception cref="NotDefinedException">Thrown when a data bank of the specified type is not found.</exception>
        public IDataBank ResolveBank(Type bankType) => Banks.Where(x => x.HoldingType == bankType).FirstOrDefault() ?? throw new NullReferenceException(); //NotDefinedException(bankType);

        /// <inheritdoc/>
        /// <exception cref="NotDefinedException">Thrown when a data bank of the specified type is not found.</exception>
        public IDataBank<TId, TData> ResolveBank<TId, TData>() where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
            => (IDataBank<TId, TData>?)Banks.Where(x => x.HoldingType == typeof(TData)).FirstOrDefault() ?? throw new NullReferenceException(); // NotDefinedException(typeof(TData));

        /// <inheritdoc/>
        public void Declare<TId, TData>(IDataBank<TId, TData> bank) where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
        {
            var bankInfo = DataBankInfo.OfDataBank(bank);
            bank.OnBankDataUpdated += (cnt) =>
            {
                bankInfo.Count += cnt;
            };

            Banks.Add(bank);
            Notations.Add(bankInfo);
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            foreach (var bank in GetBanks())
            {
                bank.Initialize();
            }
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            foreach (var bank in GetBanks())
            {
                await bank.InitializeAsync();
            }
        }
    }
}