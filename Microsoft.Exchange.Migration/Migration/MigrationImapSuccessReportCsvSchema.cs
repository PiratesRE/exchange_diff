using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationImapSuccessReportCsvSchema : ReportCsvSchema
	{
		public MigrationImapSuccessReportCsvSchema() : base(int.MaxValue, MigrationImapSuccessReportCsvSchema.requiredColumns.Value, MigrationImapSuccessReportCsvSchema.optionalColumns.Value, null)
		{
		}

		public override void WriteHeader(StreamWriter streamWriter)
		{
			streamWriter.WriteCsvLine(MigrationImapSuccessReportCsvSchema.Headers);
		}

		public override void WriteRow(StreamWriter streamWriter, MigrationJobItem jobItem)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			List<string> list = new List<string>(MigrationImapSuccessReportCsvSchema.Headers.Length);
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

		private static Lazy<Dictionary<string, ProviderPropertyDefinition>> optionalColumns = new Lazy<Dictionary<string, ProviderPropertyDefinition>>(() => MigrationCsvSchemaBase.GenerateDefaultColumnInfo(MigrationImapSuccessReportCsvSchema.OptionalColumnNames), LazyThreadSafetyMode.ExecutionAndPublication);

		private static Lazy<Dictionary<string, ProviderPropertyDefinition>> requiredColumns = new Lazy<Dictionary<string, ProviderPropertyDefinition>>(() => MigrationCsvSchemaBase.GenerateDefaultColumnInfo(MigrationImapSuccessReportCsvSchema.RequiredColumnNames), LazyThreadSafetyMode.ExecutionAndPublication);

		private static readonly string[] Headers = new string[]
		{
			"EmailAddress",
			"Status",
			"ItemsMigrated",
			"ItemsSkipped"
		};
	}
}
