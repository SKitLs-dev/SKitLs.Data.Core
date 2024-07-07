using OfficeOpenXml;

namespace SKitLs.Data.Core.IO.Excel
{
    public abstract class ExcelReaderBase<T> : IDataReader<ExcelPartRow> where T : class
    {
        public static string DataSep { get; set; } = ";";
        public static string RowSep { get; set; } = "\n";
        public static string SourceName { get; set; } = "Excel File";
        public string GetSourceName() => SourceName;

        public string DataPath { get; set; }

        public string WorksheetName { get; set; }
        public int StartRow { get; set; } = 1;
        public int StartColumn { get; set; } = 1;
        public int EndColumn { get; set; } = 100;

        public int CollisionsEncounter { get; set; } = 0;
        public int EmptyRowsBreakHit { get; set; } = 3;

        public ExcelReaderBase(string dataPath, string worksheetName, int startRow = 1, int startColumn = 1, int endColumn = 100, int emptyRowsBreakHit = 3)
        {
            DataPath = dataPath ?? throw new ArgumentNullException(nameof(dataPath));
            WorksheetName = worksheetName ?? throw new ArgumentNullException(nameof(worksheetName));
            StartRow = startRow > 0 ? startRow : throw new ArgumentOutOfRangeException(nameof(startRow));
            StartColumn = startColumn > 0 ? startColumn : throw new ArgumentOutOfRangeException(nameof(startColumn));
            EndColumn = endColumn > 0 ? endColumn : throw new ArgumentOutOfRangeException(nameof(endColumn));
            EmptyRowsBreakHit = emptyRowsBreakHit > 0 ? emptyRowsBreakHit : throw new ArgumentOutOfRangeException(nameof(emptyRowsBreakHit));
        }
        public abstract T Convert(ExcelPartRow row);

        public virtual IEnumerable<TData> ReadData<TData>() where TData : class
        {
            if (typeof(TData) == typeof(string))
            {
                var read = ReadData();
                return read.Select(x => (x.Join(DataSep) as TData)!);
            }
            else if (typeof(TData) == typeof(T))
            {
                var read = ReadData();
                return read.Select(x => (Convert(x) as TData)!);
            }
            else
                throw new NotImplementedException();
        }

        public virtual async Task<IEnumerable<TData>> ReadDataAsync<TData>(CancellationTokenSource? cts = default) where TData : class
        {
            if (typeof(TData) == typeof(string))
            {
                var read = await ReadDataAsync(cts);
                return read.Select(x => (x.Join(DataSep) as TData)!);
            }
            else if (typeof(TData) == typeof(T))
            {
                var read = await ReadDataAsync();
                return read.Select(x => (Convert(x) as TData)!);
            }
            else
                throw new NotImplementedException();
        }

        public virtual IEnumerable<ExcelPartRow> ReadData()
        {
            var result = new List<ExcelPartRow>();
            var sourceFile = new FileInfo(DataPath);
            
            if (!File.Exists(sourceFile.FullName))
                throw new FileNotFoundException();

            using var package = new ExcelPackage(sourceFile);
            var worksheet = package.Workbook.Worksheets[WorksheetName];

            var emptyCounter = 0;
            for (int row_i = StartRow; row_i <= worksheet.Dimension.End.Row; row_i++)
            {
                var row = new ExcelPartRow(row_i, StartColumn, EndColumn);
                for (int j = StartColumn; j <= EndColumn; j++)
                {
                    row.Add(worksheet.Cells[row_i, j].Text);
                }

                if (row.IsEmpty())
                {
                    emptyCounter++;
                }
                else
                {
                    emptyCounter = 0;
                    result.Add(row);
                }
                if (emptyCounter > EmptyRowsBreakHit)
                    break;
            }
            return result;
        }

        public virtual async Task<IEnumerable<ExcelPartRow>> ReadDataAsync(CancellationTokenSource? cts = default)
        {
            cts ??= new();
            try
            {
                return await Task.Run(ReadData);
            }
            catch (Exception)
            {
                cts.Cancel();
                throw;
            }
        }
    }
}