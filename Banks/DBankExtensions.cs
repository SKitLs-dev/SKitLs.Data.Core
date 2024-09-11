using SKitLs.Data.IO;
using SKitLs.Data.IO.Json;

namespace SKitLs.Data.Core.Banks
{
    /// <summary>
    /// Provides extension methods for configuring data banks with specific input/output (IO) mechanisms.
    /// </summary>
    /// <remarks>
    /// This class is marked as <see cref="ObsoleteAttribute"/> and is currently in the Beta stage. The extension methods allow
    /// data banks to be configured with JSON-based IO mechanisms, including both single-file and split-file approaches.
    /// </remarks>
    /// <seealso cref="IDataBank{TId, TData}"/>
    [Obsolete("Beta")]
    public static class DBankExtensions
    {
        /// <summary>
        /// Configures a data bank to use JSON-based input/output (IO) in a single JSON file.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier in the data bank.</typeparam>
        /// <typeparam name="TData">The type of data stored in the data bank.</typeparam>
        /// <param name="bank">The data bank to configure.</param>
        /// <returns>The configured data bank with JSON-based reader/writer.</returns>
        /// <remarks>
        /// The method sets up a JSON reader and writer that operate on a single JSON file located in the manager's data folder.
        /// The file is named after the <see cref="IDataBank.HoldingType"/> of the bank.
        /// </remarks>
        /// <seealso cref="JsonReader{TData}"/>
        /// <seealso cref="JsonWriter{TData, TId}"/>
        public static IDataBank<TId, TData> UseJsonIO<TId, TData>(this IDataBank<TId, TData> bank) where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
        {
            var dataFolder = bank.Manager.DataFolderPath;
            var dataFile = Path.Combine(dataFolder, $"{bank.HoldingType.Name.ToLower()}.json");
            var reader = new JsonReader<TData>(dataFile);
            var writer = new JsonWriter<TData, TId>(dataFile);
            bank.UpdateReader(reader);
            bank.UpdateWriter(writer);
            return bank;
        }

        /// <summary>
        /// Configures a data bank to use JSON-based input/output (IO) with split files, where each record is stored in a separate file.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier in the data bank.</typeparam>
        /// <typeparam name="TData">The type of data stored in the data bank.</typeparam>
        /// <param name="bank">The data bank to configure.</param>
        /// <returns>The configured data bank with split JSON-based reader/writer.</returns>
        /// <remarks>
        /// The method sets up a JSON reader and writer that operate on multiple JSON files, each representing a single data entity.
        /// These files are stored in a subfolder named after the <see cref="IDataBank.HoldingType"/> of the bank.
        /// </remarks>
        /// <seealso cref="JsonSplitReader{TData}"/>
        /// <seealso cref="JsonSplitWriter{TData, TId}"/>
        public static IDataBank<TId, TData> UseSplitJsonIO<TId, TData>(this IDataBank<TId, TData> bank) where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
        {
            var dataSubFolder = Path.Combine(bank.Manager.DataFolderPath, bank.HoldingType.Name.ToLower());
            if (!Directory.Exists(dataSubFolder))
                Directory.CreateDirectory(dataSubFolder);

            var reader = new JsonSplitReader<TData>(dataSubFolder);
            var writer = new JsonSplitWriter<TData, TId>(dataSubFolder);
            bank.UpdateReader(reader);
            bank.UpdateWriter(writer);
            return bank;
        }
    }
}