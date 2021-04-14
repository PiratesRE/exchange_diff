using System;
using System.Data;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class MailboxStatsLogProcessor : StatsAndInfoCommonBaseLogProcessor
	{
		public MailboxStatsLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, MigrationMonitor.MigrationMonitorContext.Config.GetConfig<string>("MbxDBStatsFolder")), "MailboxStats Log", MigrationMonitor.MailboxStatsCsvSchemaInstance, MigrationMonitor.MigrationMonitorContext.Config.GetConfig<string>("MbxStatsFileName"))
		{
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetMailboxStatsUpdateTimestampV2";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertMailboxStatsV5";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "MailboxStatsList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "dbo.MailboxStatsDataV5";
			}
		}

		protected override void LogUploadStoredProcedureHandlerError(SqlQueryFailedException ex)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error updating mailbox stats to NewMan for database {0}. Will attempt again next cycle.", new object[]
			{
				base.CurrentDatabaseName
			});
			throw new UploadMailboxStatsInBatchFailureException(ex.InnerException);
		}

		protected override void LogInsertCsvRowHandlerError(FormatException ex, CsvRow row)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error parsing mailbox stats log, mailbox guid is {0}", new object[]
			{
				MigMonUtilities.GetColumnValue<Guid>(row, "MailboxGuid")
			});
		}

		protected override bool TryAddSchemaSpecificDataRowValues(DataRow dataRow, CsvRow row)
		{
			if (!base.TryAddDatabaseIdKeyValue(dataRow))
			{
				return false;
			}
			string errorString = string.Format("Error parsing mailbox stats log, mailbox guid is {0} and its mailbox type is empty", MigMonUtilities.GetColumnValue<Guid>(row, "MailboxGuid"));
			string errorString2 = string.Format("Error parsing mailbox stats log, mailbox guid is {0} and its disconnectReason is invalid", MigMonUtilities.GetColumnValue<Guid>(row, "MailboxGuid"));
			base.TryAddStringValueByLookupId(dataRow, row, KnownStringType.DisconnectReason, errorString2, true);
			return base.TryAddStringValueByLookupId(dataRow, row, KnownStringType.MailboxType, errorString, false);
		}

		public new const string KeyNameIsLogProcessorEnabled = "IsMailboxStatsLogProcessorEnabled";

		public const string KeyNameMbxStatsFileName = "MbxStatsFileName";

		public const string DefaultMbxStatsFileNamePattern = "*MbxStats*.log";

		private const string LogTypeName = "MailboxStats Log";
	}
}
