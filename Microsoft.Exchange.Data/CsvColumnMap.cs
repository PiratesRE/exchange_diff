using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	internal struct CsvColumnMap
	{
		public CsvColumnMap(IList<string> columnNames)
		{
			this = new CsvColumnMap(columnNames, true);
		}

		public CsvColumnMap(IList<string> columnNames, bool throwOnDuplicateColumns)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>(columnNames.Count, StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < columnNames.Count; i++)
			{
				string text = columnNames[i];
				if (!string.IsNullOrEmpty(text))
				{
					if (dictionary.ContainsKey(text))
					{
						if (throwOnDuplicateColumns)
						{
							throw new CsvDuplicatedColumnException(text);
						}
					}
					else
					{
						dictionary[text] = i;
					}
				}
			}
			this.columnNames = columnNames;
			this.columnIndex = dictionary;
		}

		public int Count
		{
			get
			{
				return this.columnNames.Count;
			}
		}

		public string this[int columnIndex]
		{
			get
			{
				return this.columnNames[columnIndex];
			}
		}

		public int this[string columnName]
		{
			get
			{
				return this.columnIndex[columnName];
			}
		}

		public bool Contains(string columnName)
		{
			return this.columnIndex.ContainsKey(columnName);
		}

		public bool TryGetIndex(string columnName, out int columnIndex)
		{
			return this.columnIndex.TryGetValue(columnName, out columnIndex);
		}

		private IList<string> columnNames;

		private IDictionary<string, int> columnIndex;
	}
}
