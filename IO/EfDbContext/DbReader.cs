using Microsoft.EntityFrameworkCore;

namespace SKitLs.Data.Core.IO.EfDbContext
{
    public class DbReader<T> : IDataReader<T> where T : class
    {
        public static string SourceName { get; set; } = "DataBase Connection";
        public string GetSourceName() => SourceName;

        public DbContext Context { get; set; }

        public DbReader(DbContext context)
        {
            Context = context;
        }

        public IEnumerable<T> ReadData()
        {
            return Context.Set<T>();
        }

        public IEnumerable<TData> ReadData<TData>() where TData : class
        {
            if (typeof(TData) == typeof(T))
                return ReadData().Select(x => (x as TData)!);
            else
                throw new NotSupportedException();
        }

        public async Task<IEnumerable<T>> ReadDataAsync(CancellationTokenSource? cts = null)
        {
            cts ??= new CancellationTokenSource();
            try
            {
                return await Task.FromResult(ReadData());
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }

        public async Task<IEnumerable<TData>> ReadDataAsync<TData>(CancellationTokenSource? cts = null) where TData : class
        {
            cts ??= new CancellationTokenSource();
            try
            {
                return await Task.FromResult(ReadData<TData>());
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }
    }
}