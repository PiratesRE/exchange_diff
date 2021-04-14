using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal abstract class SlowMRSDetectorBaseLogProcessor : BaseLogProcessor
	{
		protected SlowMRSDetectorBaseLogProcessor(string logDirectoryPath, string logFileTypeName, BaseMigMonCsvSchema csvSchemaInstance, string logFileSearchPattern = null) : base(logDirectoryPath, logFileTypeName, csvSchemaInstance, logFileSearchPattern)
		{
		}

		protected override bool TryAddSchemaSpecificDataRowValues(DataRow dataRow, CsvRow row)
		{
			List<ColumnDefinition<int>> optionalColumnsIds = this.CsvSchemaInstance.GetOptionalColumnsIds();
			optionalColumnsIds.ForEach(delegate(ColumnDefinition<int> oc)
			{
				this.TryAddStringValueByLookupId(oc, dataRow, row, null, true);
			});
			List<ColumnDefinition<int>> requiredColumnsIds = this.CsvSchemaInstance.GetRequiredColumnsIds();
			bool flag = true;
			foreach (ColumnDefinition<int> columnDefinition in requiredColumnsIds)
			{
				string errorString = string.Format("Error parsing Slow MRS Detector {0} log. Column name is {1}, column type is {2}", base.LogFileTypeName, columnDefinition.ColumnName, columnDefinition.ColumnType);
				flag &= base.TryAddStringValueByLookupId(columnDefinition, dataRow, row, errorString, false);
			}
			return flag;
		}
	}
}
