using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationMoveSuccessReportCsvSchema : ReportCsvSchema
	{
		public MigrationMoveSuccessReportCsvSchema() : base(int.MaxValue, MigrationMoveSuccessReportCsvSchema.requiredColumns.Value, MigrationMoveSuccessReportCsvSchema.optionalColumns.Value, null)
		{
		}

		public override void WriteHeader(StreamWriter streamWriter)
		{
			streamWriter.WriteCsvLine(MigrationMoveSuccessReportCsvSchema.Headers);
		}

		public override void WriteRow(StreamWriter streamWriter, MigrationJobItem jobItem)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			List<string> list = new List<string>(MigrationMoveSuccessReportCsvSchema.Headers.Length);
			list.Add(jobItem.Identifier);
			if (jobItem.ItemsSkipped != 0L)
			{
				list.Add(ServerStrings.MigrationStatisticsPartiallyCompleteStatus);
			}
			else
			{
				list.Add(ServerStrings.MigrationStatisticsCompleteStatus);
			}
			list.Add(jobItem.ItemsSynced.ToString(CultureInfo.CurrentCulture));
			list.Add(jobItem.ItemsSkipped.ToString(CultureInfo.CurrentCulture));
			streamWriter.WriteCsvLine(list);
		}

		private static readonly string[] RequiredColumnNames = new string[]
		{
			"EmailAddress",
			"Status",
			"ItemsMigrated",
			"ItemsSkipped"
		};

		private static readonly string[] OptionalColumnNames = new string[]
		{
			"AdditionalComments"
		};

		private static Lazy<Dictionary<string, ProviderPropertyDefinition>> optionalColumns = new Lazy<Dictionary<string, ProviderPropertyDefinition>>(() => MigrationCsvSchemaBase.GenerateDefaultColumnInfo(MigrationMoveSuccessReportCsvSchema.OptionalColumnNames), LazyThreadSafetyMode.ExecutionAndPublication);

		private static Lazy<Dictionary<string, ProviderPropertyDefinition>> requiredColumns = new Lazy<Dictionary<string, ProviderPropertyDefinition>>(() => MigrationCsvSchemaBase.GenerateDefaultColumnInfo(MigrationMoveSuccessReportCsvSchema.RequiredColumnNames), LazyThreadSafetyMode.ExecutionAndPublication);

		private static readonly string[] Headers = new string[]
		{
			"EmailAddress",
			"Status",
			"ItemsMigrated",
			"ItemsSkipped"
		};
	}
}
