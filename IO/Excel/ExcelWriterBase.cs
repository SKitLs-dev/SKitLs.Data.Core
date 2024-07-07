using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;

namespace SKitLs.Data.Core.IO.Excel
{
    public abstract class ExcelWriterBase<T> : IDataWriter<ExcelPartRow> where T : class
    {
        public static string SourceName { get; set; } = "Excel File";
        public string GetSourceName() => SourceName;

        public string DataPath { get; set; }

        public string WorksheetName { get; set; }
        public int StartRow { get; set; } = 1;
        public int StartColumn { get; set; } = 1;

        public ExcelWriterBase(string dataPath, string worksheetName, int startRow = 1, int startColumn = 1, int endColumn = 100, int emptyRowsBreakHit = 3)
        {
            DataPath = dataPath ?? throw new ArgumentNullException(nameof(dataPath));
            WorksheetName = worksheetName ?? throw new ArgumentNullException(nameof(worksheetName));
            StartRow = startRow > 0 ? startRow : throw new ArgumentOutOfRangeException(nameof(startRow));
            StartColumn = startColumn > 0 ? startColumn : throw new ArgumentOutOfRangeException(nameof(startColumn));
        }

        public abstract ExcelPartRow Convert(T data);

        public virtual bool WriteData<TData>(TData item) where TData : class
        {
            if (typeof(TData) == typeof(T))
                return WriteData(Convert((item as T)!));
            else if (typeof(TData) == typeof(ExcelPartRow))
                return WriteData((item as ExcelPartRow)!);
            else
                throw new NotSupportedException();
        }

        public virtual async Task<bool> WriteDataAsync<TData>(TData item, CancellationTokenSource? cts) where TData : class
        {
            cts ??= new();
            try
            {
                return await Task.Run(() =>
                {
                    return WriteData(item);
                });
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }

        public virtual bool WriteData(ExcelPartRow value)
        {
            var sourceFile = new FileInfo(DataPath);

            if (!File.Exists(sourceFile.FullName))
                throw new FileNotFoundException();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets[WorksheetName];

            try
            {
                for (int j = 0; j < value.Count; j++)
                {
                    worksheet.Cells[value.RowIndex, StartColumn + j].Value = value[j];
                }
                package.Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public virtual async Task<bool> WriteDataAsync(ExcelPartRow value, CancellationTokenSource? cts)
        {
            cts ??= new();
            try
            {
                return await Task.Run(() =>
                {
                    return WriteData(value);
                });
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }

        public virtual bool WriteData<TData>(IEnumerable<TData> items) where TData : class
        {
            if (typeof(TData) == typeof(T))
                return WriteData(items.Select(x => Convert((x as T)!)).ToList());
            else if (typeof(TData) == typeof(ExcelPartRow))
                return WriteData(items.Select(x => (x as ExcelPartRow)!).ToList());
            else
                throw new NotSupportedException();
        }

        public virtual async Task<bool> WriteDataAsync<TData>(IEnumerable<TData> items, CancellationTokenSource? cts) where TData : class
        {
            cts ??= new();
            try
            {
                return await Task.Run(() =>
                {
                    return WriteData(items);
                });
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }

        public virtual bool WriteData(IEnumerable<ExcelPartRow> values)
        {
            var sourceFile = new FileInfo(DataPath);

            if (!File.Exists(sourceFile.FullName))
                throw new FileNotFoundException();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets[WorksheetName];

            foreach (var part in values)
            {
                try
                {
                    for (int j = 0; j < part.Count; j++)
                    {
                        worksheet.Cells[part.RowIndex, StartColumn + j].Value = part[j];
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        public virtual async Task<bool> WriteDataAsync(IEnumerable<ExcelPartRow> values, CancellationTokenSource? cts)
        {
            cts ??= new();
            try
            {
                return await Task.Run(() =>
                {
                    return WriteData(values);
                });
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }

        private void WriteDataToWorksheet(ExcelPartRow dataRow, ExcelWorksheet worksheet)
        {
            for (int j = 0; j < dataRow.Count; j++)
            {
                worksheet.Cells[dataRow.RowIndex, StartColumn + j].Value = dataRow[j];
            }
        }
    }
}