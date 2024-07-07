using SKitLs.Data.Core.Core;
using SKitLs.Data.Core.IdGenerator;
using SKitLs.Data.Core.IO;
using System.Collections;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SKitLs.Data.Core.Banks
{
    public class DataBank<TId, TData> : IDataBank<TId, TData> where TId : notnull, IEquatable<TId>, IComparable<TId> where TData : ModelDso<TId>
    {
        public event DataBankUpdatedHandler? OnBankInfoUpdated;
        public event DataBankCollectionUpdatedHandler? OnBankDataUpdated;
        public event DataBankCollectionUpdatedHandler<TId, TData>? OnBankDataAdded;
        public event DataBankCollectionUpdatedHandler<TId, TData>? OnBankDataRemoved;

        public string Id => InType.Name.ToLower();

        public string? Name { get; set; }

        public string? Description { get; set; }

        public Type InType => typeof(TData);


        private SortedList<TId, TData> _data = [];

        public long Count => _data.Count;

        public DropStrategy DropStrategy { get; init; }

        public IIdGenerator<TId>? IdGenerator { get; init; }
        public IDataReader? Reader { get; init; }
        public IDataWriter? Writer { get; init; }

        public Func<TData>? NewInstanceGenerator { get; init; }

        public DataBank(string name, string? description, DropStrategy dropStrategy)
        {
            Name = name;
            Description = description ?? "No more info";
            DropStrategy = dropStrategy;
        }

        public DataBank(IDataReader<TData> reader, IDataWriter<TData> writer, string name, string? description = null, DropStrategy dropStrategy = DropStrategy.Disable, IIdGenerator<TId>? idGenerator = null) : this(name, description, dropStrategy)
        {
            Reader = reader;
            Writer = writer;
            IdGenerator = idGenerator;
        }

        public void Initialize()
        {
            (Reader?.ReadData<TData>().ToList() ?? throw new NullReferenceException()).ForEach(x => _data.Add(x.GetId(), x));
        }

        public async Task InitializeAsync()
        {
            if (Reader is not null)
            {
                var read = await Reader.ReadDataAsync<TData>(null);
                read.ToList().ForEach(x => _data.Add(x.GetId(), x));
            }
            else
                throw new NullReferenceException();
        }

        public TData BuildNewData()
        {
            var @new = NewInstanceGenerator?.Invoke() ?? throw new NotImplementedException();
            if (IdGenerator is null)
                throw new NullReferenceException();
            @new.SetId(IdGenerator.GetDefaultId());
            return @new;
        }

        public object BuildNewObject() => BuildNewData();

        public void AddNSave<T>(T data) where T : class
        {
            if (typeof(T) == typeof(TData))
                AddNSave((data as TData)!);
            else
                throw new NotSupportedException($"Cannot insert {typeof(T)} values to {typeof(TData)} bank.");
        }
        public void AddNSave<T>(IEnumerable<T> dataCollection) where T : class
        {
            if (typeof(T) == typeof(TData) || dataCollection.FirstOrDefault()?.GetType() == typeof(TData))
                AddNSave(dataCollection.Select(x => (x as TData)!));
            else
                throw new NotSupportedException($"Cannot insert {typeof(T)} values to {typeof(TData)} bank.");
        }

        public void AddNSave(TData data)
        {
            if (IdGenerator?.IsDefaultID(data.GetId()) ?? true)
            {
                data.OnSaveRequested += (s, e) =>
                {
                    if (s is TData data)
                        SaveObject(data);
                    else throw new Exception();
                };
                if (IdGenerator is not null)
                    data.SetId(IdGenerator.GenerateIdFor(this));
                _data.Add(data.GetId(), data);
            }
            SaveObject(data);
            OnBankDataUpdated?.Invoke(1);
            OnBankDataAdded?.Invoke([data]);
        }
        public void AddNSave(IEnumerable<TData> dataCollection)
        {
            foreach (var data in dataCollection)
            {
                if (IdGenerator?.IsDefaultID(data.GetId()) ?? true)
                {
                    data.OnSaveRequested += (s, e) =>
                    {
                        if (s is TData data)
                            SaveObject(data);
                        else throw new Exception();
                    };
                    if (IdGenerator is not null)
                        data.SetId(IdGenerator.GenerateIdFor(this));
                    _data.Add(data.GetId(), data);
                }
            }
            SaveObjects(dataCollection);
            OnBankDataUpdated?.Invoke(dataCollection.Count());
            OnBankDataAdded?.Invoke(dataCollection);
        }

        public void SaveObject(TData @object)
        {
            try
            {
                Writer?.WriteData(@object);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void SaveObjects(IEnumerable<TData> objects)
        {
            try
            {
                Writer?.WriteData(objects);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void DropNSave(object data)
        {
            if (data.GetType() == typeof(TData))
                DropNSave((data as TData)!);
            else
                throw new Exception();
        }

        public void DropNSave(TData data)
        {
            try
            {
                data.Disable();
                SaveObject(data);
                OnBankDataUpdated?.Invoke(1);
                OnBankDataRemoved?.Invoke([data]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public IReadOnlyList<TData> GetReadonlyData() => [.. _data.Values.Where(x => x.IsEnabled)];
        public IReadOnlyList<object> GetReadonlyObjects() => GetReadonlyData();

        public bool Drop(Func<TData, bool> predicate) => throw new NotImplementedException();

        public int DropAll(Func<TData, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IdData<TId, TData> GetData(TId id)
        {
            throw new NotImplementedException();
        }

        public IdData<TId, TData> GetData(Func<TData, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IdData<TId, TData>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public TData GetValue(TId id)
        {
            throw new NotImplementedException();
        }

        public TData GetValue(Func<TData, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public int LoadData<TIn>(List<TIn> data)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}