using System;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class ColumnDefinition<T> : IColumnDefinition
	{
		public ColumnDefinition() : this(string.Empty)
		{
		}

		public ColumnDefinition(string columnName)
		{
			this.ColumnName = columnName;
			this.DataTableKeyColumnName = columnName;
			this.KnownStringType = KnownStringType.None;
		}

		public ColumnDefinition(string columnName, string dataTableKeyColumnName, KnownStringType knownStringType) : this(columnName)
		{
			this.DataTableKeyColumnName = dataTableKeyColumnName;
			this.KnownStringType = knownStringType;
		}

		public string ColumnName { get; private set; }

		public string DataTableKeyColumnName { get; private set; }

		public KnownStringType KnownStringType { get; private set; }

		public Type ColumnType
		{
			get
			{
				return typeof(T);
			}
		}

		public void InsertColumnValueInDataRow(DataRow dataRow, CsvRow row)
		{
			dataRow[this.ColumnName] = MigMonUtilities.GetColumnValue<T>(row, this.ColumnName);
		}

		public string GetConvertedRowString(CsvRow row)
		{
			string columnStringValue = MigMonUtilities.GetColumnStringValue(row, this.ColumnName);
			return KnownStringsHelper.ConvertStringValueByType(this.KnownStringType, columnStringValue);
		}
	}
}
