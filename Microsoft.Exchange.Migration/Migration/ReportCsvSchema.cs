using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Migration
{
	internal abstract class ReportCsvSchema : CsvSchema
	{
		public ReportCsvSchema(int maximumRowCount, Dictionary<string, ProviderPropertyDefinition> requiredColumns, Dictionary<string, ProviderPropertyDefinition> optionalColumns, IEnumerable<string> prohibitedColumns) : base(maximumRowCount, requiredColumns, optionalColumns, prohibitedColumns)
		{
		}

		public abstract void WriteHeader(StreamWriter streamWriter);

		public abstract void WriteRow(StreamWriter streamWriter, MigrationJobItem jobItem);
	}
}
