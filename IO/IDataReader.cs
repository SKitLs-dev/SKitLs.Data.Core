namespace SKitLs.Data.Core.IO
{
    public interface IDataReader
    {
        public string GetSourceName();

        public IEnumerable<TData> ReadData<TData>() where TData : class;

        public Task<IEnumerable<TData>> ReadDataAsync<TData>(CancellationTokenSource? cts) where TData : class;
    }
}