using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal static class MigrationSuccessReportCsvSchema
	{
		public static void WriteHeader(StreamWriter streamWriter, MigrationType migrationType, bool isBatchCompletionReport, bool isStaged)
		{
			ReportCsvSchema schema = MigrationSuccessReportCsvSchema.GetSchema(migrationType, isBatchCompletionReport, isStaged);
			schema.WriteHeader(streamWriter);
		}

		public static void Write(StreamWriter streamWriter, MigrationType migrationType, IEnumerable<MigrationJobItem> jobItemCollection, bool isBatchCompletionReport, bool isStaged)
		{
			ReportCsvSchema schema = MigrationSuccessReportCsvSchema.GetSchema(migrationType, isBatchCompletionReport, isStaged);
			foreach (MigrationJobItem jobItem in jobItemCollection)
			{
				schema.WriteRow(streamWriter, jobItem);
			}
		}

		internal static ReportCsvSchema GetSchema(MigrationType migrationType, bool isBatchCompletionReport, bool isStaged)
		{
			if (migrationType <= MigrationType.ExchangeOutlookAnywhere)
			{
				if (migrationType == MigrationType.IMAP)
				{
					return new MigrationImapSuccessReportCsvSchema();
				}
				if (migrationType == MigrationType.ExchangeOutlookAnywhere)
				{
					return new MigrationExchangeSuccessReportCsvSchema(isBatchCompletionReport && !isStaged);
				}
			}
			else if (migrationType == MigrationType.ExchangeRemoteMove || migrationType == MigrationType.ExchangeLocalMove || migrationType == MigrationType.PublicFolder)
			{
				return new MigrationMoveSuccessReportCsvSchema();
			}
			string text = "MigrationSuccessReportCsvSchema invoked with unsupported migrationType " + migrationType;
			MigrationLogger.Log(MigrationEventType.Error, text, new object[0]);
			throw new InvalidOperationException(text);
		}

		public const int InternalMaximumRowCount = 2147483647;

		public const string PasswordColumnName = "Password";

		public const string EmailColumnName = "EmailAddress";

		public const string StatusColumnName = "Status";

		public const string ItemsMigratedColumnName = "ItemsMigrated";

		public const string ItemsSkippedColumnName = "ItemsSkipped";

		public const string TypeColumnName = "ObjectType";

		public const string AdditionalComments = "AdditionalComments";
	}
}
