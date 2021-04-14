using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	internal struct CsvRow
	{
		public CsvRow(int index, IList<string> data, CsvColumnMap columnMap)
		{
			if (data.Count != columnMap.Count)
			{
				throw new CsvWrongNumberOfColumnsException(index, columnMap.Count, data.Count);
			}
			this.index = index;
			this.data = data;
			this.columnMap = columnMap;
			this.errors = new Dictionary<string, PropertyValidationError>();
		}

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public IList<string> Data
		{
			get
			{
				return this.data;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.errors.Count == 0;
			}
		}

		public IDictionary<string, PropertyValidationError> Errors
		{
			get
			{
				return this.errors;
			}
		}

		public CsvColumnMap ColumnMap
		{
			get
			{
				return this.columnMap;
			}
		}

		public string this[string columnName]
		{
			get
			{
				int num = this.columnMap[columnName];
				return this.data[num];
			}
			set
			{
				int num = this.columnMap[columnName];
				this.data[num] = value;
			}
		}

		public bool TryGetColumnValue(string columnName, out string columnValue)
		{
			int num;
			if (this.columnMap.TryGetIndex(columnName, out num))
			{
				columnValue = this.data[num];
				return true;
			}
			columnValue = null;
			return false;
		}

		public IEnumerable<KeyValuePair<string, string>> GetExistingValues()
		{
			for (int columnIndex = 0; columnIndex < this.columnMap.Count; columnIndex++)
			{
				string columnData = this.data[columnIndex];
				if (!string.IsNullOrEmpty(columnData))
				{
					string columnName = this.columnMap[columnIndex];
					yield return new KeyValuePair<string, string>(columnName, columnData);
				}
			}
			yield break;
		}

		internal void SetError(string columnName, PropertyValidationError error)
		{
			this.errors[columnName] = error;
		}

		private int index;

		private IList<string> data;

		private CsvColumnMap columnMap;

		private IDictionary<string, PropertyValidationError> errors;
	}
}
