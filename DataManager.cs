using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml.Export.ToDataTable;
using SKitLs.Data.Core.Banks;
using SKitLs.Data.Core.IdGenerator;
using SKitLs.Data.Core.IO.EfDbContext;
using SKitLs.Data.IO;
using SKitLs.Data.IO.Json;
using SKitLs.Data.IO.Shortcuts;
using System.IO;

namespace SKitLs.Data.Core
{
    /// <summary>
    /// Implementation of <see cref="IDataManager"/> that manages various data banks.
    /// </summary>
    public class DataManager : IDataManager
    {
        /// <inheritdoc/>
        public string DataFolderPath { get; init; }

        private List<DataBankInfo> Notations { get; set; } = [];

        /// <inheritdoc/>
        public IReadOnlyCollection<DataBankInfo> GetNotations() => Notations;

        private List<IDataBank> Banks { get; set; } = [];

        /// <inheritdoc/>
        public IEnumerable<IDataBank> GetBanks() => Banks;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataManager"/> class, setting up the specified data folder.
        /// </summary>
        /// <param name="dataFolderPath">The path to the folder where data will be managed and stored.</param>
        public DataManager(string dataFolderPath = "Resources/Data")
        {
            if (!Directory.Exists(dataFolderPath))
            {
                // TODO Logs
                Console.WriteLine($"{nameof(DataManager)}: {dataFolderPath} not found. Creating...");
                Directory.CreateDirectory(dataFolderPath);
            }

            DataFolderPath = dataFolderPath;
            InitializeDeclarations();
        }

        /// <inheritdoc/>
        /// <exception cref="NullReferenceException">Thrown when a data bank of the specified type is not found.</exception>
        public IDataBank ResolveBank(Type bankType) => Banks.Where(x => x.HoldingType == bankType).FirstOrDefault() ?? throw new NullReferenceException(); //NotDefinedException(bankType);

        /// <inheritdoc/>
        /// <exception cref="NullReferenceException">Thrown when a data bank of the specified type is not found.</exception>
        public IDataBank<TId, TData> ResolveBank<TId, TData>() where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
            => (IDataBank<TId, TData>?)Banks.Where(x => x.HoldingType == typeof(TData)).FirstOrDefault() ?? throw new NullReferenceException(); // NotDefinedException(typeof(TData));

        /// <inheritdoc/>
        public void Declare<TId, TData>(IDataBank<TId, TData> bank) where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
        {
            bank.Manager = this;
            var bankInfo = DataBankInfo.OfDataBank(bank);
            bank.OnBankDataUpdated += (cnt) =>
            {
                bankInfo.Count = bank.Count;
            };

            Banks.Add(bank);
            Notations.Add(bankInfo);
        }

        /// <summary>
        /// Initializes data bank declarations. This method is called during the class initialization to set up and declare the necessary data banks.
        /// </summary>
        /// <remarks>
        /// This method can be overridden in derived classes to customize the declaration process for specific data banks.
        /// </remarks>
        public virtual void InitializeDeclarations() { }

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

        #region DBanks Shortcuts
        /// <summary>
        /// Creates a data bank of type <typeparamref name="TData"/> with default JSON-based Reader/Writer in the manager's folder (<see cref="DataFolderPath"/>).
        /// The bank uses the provided <paramref name="idGenerator"/> to generate unique identifiers.
        /// <para/>
        /// The <c>NewInstanceGenerator</c> is set to use the default constructor of <typeparamref name="TData"/> (requires a parameterless constructor).
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TData">The type of data stored in the bank, which must implement <see cref="ModelDso{TId}"/>.</typeparam>
        /// <param name="idGenerator">The generator used for creating unique IDs.</param>
        /// <param name="dbName">The name of the data bank.</param>
        /// <param name="dbDescription">An optional description for the data bank.</param>
        /// <param name="dropStrategy">The strategy for handling data drops, default is <see cref="DropStrategy.Disable"/>.</param>
        /// <returns>An instance of <see cref="IDataBank{TId, TData}"/> initialized with JSON reader/writer.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="dbName"/> is null or whitespace.</exception>
        public IDataBank<TId, TData> JsonBank<TId, TData>(IIdGenerator<TId> idGenerator, string dbName, string? dbDescription = null, DropStrategy dropStrategy = DropStrategy.Disable) where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(dbName, nameof(dbName));

            var path = Path.Combine(DataFolderPath, HotIO.FitJsonPath(dbName));
            var bank = new DataBank<TId, TData>(dbName, dbDescription, dropStrategy)
            {
                Reader = new JsonReader<TData>(path) { CreateNewFile = true },
                Writer = new JsonWriter<TData, TId>(path),
                IdGenerator = idGenerator,
                NewInstanceGenerator = Activator.CreateInstance<TData>,
            };
            Declare(bank);
            return bank;
        }

        /// <summary>
        /// Creates a data bank of type <typeparamref name="TData"/> with default Entity Framework-based Reader/Writer in the manager's folder (<see cref="DataFolderPath"/>).
        /// The bank uses the provided <paramref name="idGenerator"/> to generate unique identifiers.
        /// The <c>NewInstanceGenerator</c> is set to use the default constructor of <typeparamref name="TData"/> (requires a parameterless constructor).
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TData">The type of data stored in the bank, which must implement <see cref="ModelDso{TId}"/>.</typeparam>
        /// <param name="context">The <see cref="DbContext"/> used for database interactions.</param>
        /// <param name="idGenerator">The generator used for creating unique IDs.</param>
        /// <param name="dbName">The name of the data bank.</param>
        /// <param name="dbDescription">An optional description for the data bank.</param>
        /// <param name="dropStrategy">The strategy for handling data drops, default is <see cref="DropStrategy.Disable"/>.</param>
        /// <returns>An instance of <see cref="IDataBank{TId, TData}"/> initialized with Entity Framework reader/writer.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="dbName"/> is null or whitespace.</exception>
        public IDataBank<TId, TData> EfBank<TId, TData>(DbContext context, IIdGenerator<TId> idGenerator, string dbName, string? dbDescription = null, DropStrategy dropStrategy = DropStrategy.Disable) where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(dbName, nameof(dbName));

            var bank = new DataBank<TId, TData>(dbName, dbDescription, dropStrategy)
            {
                Reader = new DbReader<TData>(context),
                Writer = new DbWriter<TData, TId>(context),
                IdGenerator = idGenerator,
                NewInstanceGenerator = Activator.CreateInstance<TData>,
            };
            Declare(bank);
            return bank;
        }
        #endregion
    }
}