using Microsoft.EntityFrameworkCore;
using SKitLs.Data.Core.Core;

namespace SKitLs.Data.Core.IO.EfDbContext
{
    public class DbWriter<T, TId> : IDataWriter<T> where T : ModelDso<TId> where TId : notnull, IEquatable<TId>, IComparable<TId>
    {
        public static string SourceName { get; set; } = "DataBase Connection";
        public string GetSourceName() => SourceName;

        public DbContext Context { get; set; }

        public DbWriter(DbContext context)
        {
            Context = context;
        }

        public bool WriteData<TData>(TData item) where TData : class
        {
            if (typeof(TData) == typeof(T))
                return WriteData((item as T)!);
            else
                throw new NotSupportedException();
        }

        public async Task<bool> WriteDataAsync<TData>(TData item, CancellationTokenSource? cts) where TData : class
        {
            if (typeof(TData) == typeof(T))
                return await WriteDataAsync((item as T)!, cts);
            else
                throw new NotSupportedException();
        }

        public bool WriteData(T item)
        {
            try
            {
                var dbSet = Context.Set<T>();
                var existingEntity = dbSet.Find(item.GetId());
                if (existingEntity is null)
                {
                    dbSet.Add(item);
                }
                else
                {
                    Context.Entry(existingEntity).CurrentValues.SetValues(item);
                }
                Context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> WriteDataAsync(T item, CancellationTokenSource? cts)
        {
            cts ??= new();
            try
            {
                var dbSet = Context.Set<T>();
                var existingEntity = dbSet.Find(item.GetId());
                if (existingEntity is null)
                {
                    dbSet.Add(item);
                }
                else
                {
                    Context.Entry(existingEntity).CurrentValues.SetValues(item);
                }
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                cts.Cancel();
                return false;
            }
        }

        public bool WriteData<TData>(IEnumerable<TData> items) where TData : class
        {
            if (typeof(TData) == typeof(T) || items.FirstOrDefault()?.GetType() == typeof(T))
                return WriteData(items.Select(x => (x as T)!));
            else
                throw new NotSupportedException();
        }

        public async Task<bool> WriteDataAsync<TData>(IEnumerable<TData> items, CancellationTokenSource? cts) where TData : class
        {
            if (typeof(TData) == typeof(T))
                return await WriteDataAsync(items.Select(x => (x as T)!), cts);
            else
                throw new NotSupportedException();
        }

        public bool WriteData(IEnumerable<T> items)
        {
            try
            {
                var dbSet = Context.Set<T>();
                foreach (var item in items)
                {
                    var existingEntity = dbSet.Find(item.GetId());
                    if (existingEntity is null)
                    {
                        dbSet.Add(item);
                    }
                    else
                    {
                        Context.Entry(existingEntity).CurrentValues.SetValues(item);
                    }
                }
                Context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> WriteDataAsync(IEnumerable<T> items, CancellationTokenSource? cts)
        {
            cts ??= new();
            try
            {
                var dbSet = Context.Set<T>();
                foreach (var item in items)
                {
                    var existingEntity = dbSet.Find(item.GetId());
                    if (existingEntity is null)
                    {
                        dbSet.Add(item);
                    }
                    else
                    {
                        Context.Entry(existingEntity).CurrentValues.SetValues(item);
                    }
                }
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                cts.Cancel();
                return false;
            }
        }
    }
}