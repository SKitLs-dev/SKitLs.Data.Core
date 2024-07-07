namespace SKitLs.Data.Core.IO
{
    public interface IDataWriter<T> : IDataWriter where T : class
    {
        public bool WriteData(T item);

        public Task<bool> WriteDataAsync(T item, CancellationTokenSource? cts);

        public bool WriteData(IEnumerable<T> items);

        public Task<bool> WriteDataAsync(IEnumerable<T> items, CancellationTokenSource? cts);
    }
}