namespace SKitLs.Data.Core.IO
{
    public interface IDataReader<T> : IDataReader
    {
        public IEnumerable<T> ReadData();

        public Task<IEnumerable<T>> ReadDataAsync(CancellationTokenSource? cts);
    }
}