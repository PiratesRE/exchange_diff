using System;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal interface IColumnDefinition
	{
		string ColumnName { get; }

		Type ColumnType { get; }

		void InsertColumnValueInDataRow(DataRow dataRow, CsvRow row);
	}
}
