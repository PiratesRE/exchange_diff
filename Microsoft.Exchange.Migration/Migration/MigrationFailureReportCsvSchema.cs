using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MigrationFailureReportCsvSchema
	{
		public static void WriteHeader(StreamWriter streamWriter, MigrationType migrationType, bool isBatchCompletionReport)
		{
			FailureReportCsvSchema schema = MigrationFailureReportCsvSchema.GetSchema(migrationType, isBatchCompletionReport);
			schema.WriteHeader(streamWriter);
		}

		public static void Write(StreamWriter streamWriter, MigrationType migrationType, IEnumerable<MigrationJobItem> jobItemCollection, bool isBatchCompletionReport)
		{
			FailureReportCsvSchema schema = MigrationFailureReportCsvSchema.GetSchema(migrationType, isBatchCompletionReport);
			foreach (MigrationJobItem jobItem in jobItemCollection)
			{
				schema.WriteRow(streamWriter, jobItem);
			}
		}

		public static void Write(MigrationType migrationType, StreamWriter streamWriter, IEnumerable<MigrationBatchError> errorCollection, bool isBatchCompletionReport)
		{
			FailureReportCsvSchema schema = MigrationFailureReportCsvSchema.GetSchema(migrationType, isBatchCompletionReport);
			foreach (MigrationBatchError batchError in errorCollection)
			{
				schema.WriteRow(streamWriter, batchError);
			}
		}

		internal static FailureReportCsvSchema GetSchema(MigrationType migrationType, bool isBatchCompletionReport)
		{
			if (migrationType <= MigrationType.ExchangeOutlookAnywhere)
			{
				if (migrationType == MigrationType.IMAP)
				{
					return new MigrationImapFailureReportCsvSchema(isBatchCompletionReport);
				}
				if (migrationType != MigrationType.ExchangeOutlookAnywhere)
				{
					goto IL_35;
				}
			}
			else if (migrationType != MigrationType.ExchangeRemoteMove && migrationType != MigrationType.ExchangeLocalMove && migrationType != MigrationType.PublicFolder)
			{
				goto IL_35;
			}
			return new MigrationExchangeFailureReportCsvSchema();
			IL_35:
			string text = "MigrationSuccessReportCsvSchema invoked with unsupported migrationType " + migrationType;
			MigrationLogger.Log(MigrationEventType.Error, text, new object[0]);
			throw new InvalidOperationException(text);
		}

		public const int InternalMaximumRowCount = 2147483647;

		public const string RowIndexColumnName = "RowIndex";

		public const string EmailColumnName = "EmailAddress";

		public const string ErrorMessageColumnName = "ErrorMessage";
	}
}
