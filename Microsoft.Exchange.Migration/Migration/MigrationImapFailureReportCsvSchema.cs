using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationImapFailureReportCsvSchema : FailureReportCsvSchema
	{
		public MigrationImapFailureReportCsvSchema(bool isCompletionReport) : base(int.MaxValue, MigrationImapFailureReportCsvSchema.requiredColumns.Value, MigrationImapFailureReportCsvSchema.optionalColumns.Value, null)
		{
			this.isCompletionReport = isCompletionReport;
		}

		private string[] Headers
		{
			get
			{
				if (this.isCompletionReport)
				{
					return MigrationImapFailureReportCsvSchema.CompletionHeaders;
				}
				return MigrationImapFailureReportCsvSchema.FinalizationHeaders;
			}
		}

		public override void WriteHeader(StreamWriter streamWriter)
		{
			streamWriter.WriteCsvLine(this.Headers);
		}

		public override void WriteRow(StreamWriter streamWriter, MigrationJobItem jobItem)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			List<string> list = new List<string>(this.Headers.Length);
			if (this.isCompletionReport)
			{
				list.Add(jobItem.CursorPosition.ToString(CultureInfo.InvariantCulture));
			}
			list.Add(jobItem.Identifier);
			list.Add(jobItem.LocalizedError ?? LocalizedString.Empty);
			streamWriter.WriteCsvLine(list);
		}

		public override void WriteRow(StreamWriter streamWriter, MigrationBatchError batchError)
		{
			MigrationUtil.ThrowOnNullArgument(batchError, "batchError");
			List<string> list = new List<string>(this.Headers.Length);
			if (this.isCompletionReport)
			{
				list.Add(batchError.RowIndex.ToString(CultureInfo.InvariantCulture));
			}
			list.Add(batchError.EmailAddress);
			list.Add(batchError.LocalizedErrorMessage);
			streamWriter.WriteCsvLine(list);
		}

		private static readonly string[] RequiredColumnNames = new string[]
		{
			"EmailAddress",
			"ErrorMessage"
		};

		private static readonly string[] OptionalColumnNames = new string[]
		{
			"RowIndex"
		};

		private static Lazy<Dictionary<string, ProviderPropertyDefinition>> optionalColumns = new Lazy<Dictionary<string, ProviderPropertyDefinition>>(() => MigrationCsvSchemaBase.GenerateDefaultColumnInfo(MigrationImapFailureReportCsvSchema.OptionalColumnNames), LazyThreadSafetyMode.ExecutionAndPublication);

		private static Lazy<Dictionary<string, ProviderPropertyDefinition>> requiredColumns = new Lazy<Dictionary<string, ProviderPropertyDefinition>>(() => MigrationCsvSchemaBase.GenerateDefaultColumnInfo(MigrationImapFailureReportCsvSchema.RequiredColumnNames), LazyThreadSafetyMode.ExecutionAndPublication);

		private static readonly string[] CompletionHeaders = new string[]
		{
			"RowIndex",
			"EmailAddress",
			"ErrorMessage"
		};

		private static readonly string[] FinalizationHeaders = new string[]
		{
			"EmailAddress",
			"ErrorMessage"
		};

		private readonly bool isCompletionReport;
	}
}
