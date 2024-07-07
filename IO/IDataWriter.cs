namespace SKitLs.Data.Core.IO
{
    public interface IDataWriter
    {
        public string GetSourceName();

        public bool WriteData<TData>(TData item) where TData : class;

        public Task<bool> WriteDataAsync<TData>(TData item, CancellationTokenSource? cts) where TData : class;

        public bool WriteData<TData>(IEnumerable<TData> items) where TData : class;

        public Task<bool> WriteDataAsync<TData>(IEnumerable<TData> items, CancellationTokenSource? cts) where TData : class;
    }
}