using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationExchangeFailureReportCsvSchema : FailureReportCsvSchema
	{
		public MigrationExchangeFailureReportCsvSchema() : base(int.MaxValue, MigrationExchangeFailureReportCsvSchema.requiredColumns.Value, MigrationExchangeFailureReportCsvSchema.optionalColumns.Value, null)
		{
		}

		public override void WriteHeader(StreamWriter streamWriter)
		{
			streamWriter.WriteCsvLine(MigrationExchangeFailureReportCsvSchema.Headers);
		}

		public override void WriteRow(StreamWriter streamWriter, MigrationJobItem jobItem)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			string[] columnData = new string[]
			{
				jobItem.Identifier,
				jobItem.LocalizedError ?? LocalizedString.Empty
			};
			streamWriter.WriteCsvLine(columnData);
		}

		public override void WriteRow(StreamWriter streamWriter, MigrationBatchError batchError)
		{
			MigrationUtil.ThrowOnNullArgument(batchError, "batchError");
			string[] columnData = new string[]
			{
				batchError.EmailAddress,
				batchError.LocalizedErrorMessage
			};
			streamWriter.WriteCsvLine(columnData);
		}

		private static readonly string[] RequiredColumnNames = new string[]
		{
			"EmailAddress",
			"ErrorMessage"
		};

		private static readonly string[] OptionalColumnNames = new string[0];

		private static Lazy<Dictionary<string, ProviderPropertyDefinition>> optionalColumns = new Lazy<Dictionary<string, ProviderPropertyDefinition>>(() => MigrationCsvSchemaBase.GenerateDefaultColumnInfo(MigrationExchangeFailureReportCsvSchema.OptionalColumnNames), LazyThreadSafetyMode.ExecutionAndPublication);

		private static Lazy<Dictionary<string, ProviderPropertyDefinition>> requiredColumns = new Lazy<Dictionary<string, ProviderPropertyDefinition>>(() => MigrationCsvSchemaBase.GenerateDefaultColumnInfo(MigrationExchangeFailureReportCsvSchema.RequiredColumnNames), LazyThreadSafetyMode.ExecutionAndPublication);

		private static readonly string[] Headers = new string[]
		{
			"EmailAddress",
			"ErrorMessage"
		};
	}
}
