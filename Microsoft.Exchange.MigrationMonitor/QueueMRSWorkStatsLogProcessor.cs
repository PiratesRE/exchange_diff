using System;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class QueueMRSWorkStatsLogProcessor : SlowMRSDetectorBaseLogProcessor
	{
		public QueueMRSWorkStatsLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, "Logging\\MailboxReplicationService\\QueueStats"), "QueueMRSStats", MigrationMonitor.QueueMRSWorkStatsCsvSchemaInstance, "QueueMRSStats*.LOG")
		{
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetQueueMRSStatsUpdateTimestamp";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertQueueMRSStats_BatchUpload";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "QueueMRSStatsList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "QueueMRSStatsData";
			}
		}

		protected override void LogUploadStoredProcedureHandlerError(SqlQueryFailedException ex)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error uploading Queue Statistics logs. Will attempt again next cycle.", new object[0]);
			throw new UploadQueueStatsLogInBatchFailureException(ex.InnerException);
		}

		protected override void LogInsertCsvRowHandlerError(FormatException ex, CsvRow row)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error parsing Queue Statistics log.", new object[0]);
		}

		public new const string KeyNameIsLogProcessorEnabled = "IsQueueMRSWorkStatsLogProcessorEnabled";

		public const string LogTypeName = "QueueMRSStats";

		public const string RelativeLogPath = "Logging\\MailboxReplicationService\\QueueStats";
	}
}
