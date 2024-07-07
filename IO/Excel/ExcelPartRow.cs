using SKitLs.Utils.Extensions.Lists;

namespace SKitLs.Data.Core.IO.Excel
{
    public class ExcelPartRow
    {
        public int RowIndex { get; set; }
        public int StartColumnIndex { get; set; }
        public int EndColumnIndex { get; set; }

        public List<string> Values { get; set; }

        public ExcelPartRow(int row, int startIndex, int endIndex)
        {
            RowIndex = row;
            StartColumnIndex = startIndex;
            EndColumnIndex = endIndex;
            Values = [];
        }

        public string this[int index]
        {
            get
            {
                var relativeIndex = index - StartColumnIndex;
                return relativeIndex >= 0 && relativeIndex < Values.Count ? Values[relativeIndex] : throw new IndexOutOfRangeException();
            }
        }

        public void Add(string value) => Values.Add(value);

        public int Count => Values.Count;

        public bool IsEmpty() => Values.Select(x => string.IsNullOrEmpty(x)).AllTrue();

        public string Join(string? sep = ";") => string.Join(sep, Values);
    }
}