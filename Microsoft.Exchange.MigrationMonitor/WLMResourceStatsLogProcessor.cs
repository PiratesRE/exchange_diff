using System;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class WLMResourceStatsLogProcessor : SlowMRSDetectorBaseLogProcessor
	{
		public WLMResourceStatsLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, "Logging\\MailboxReplicationService\\WLMResourceStatss"), "WLMResourceStats", MigrationMonitor.WLMResourceStatsCsvSchemaInstance, "WLMResourceStats*.LOG")
		{
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetWLMResouceStatsUpdateTimestamp";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertWLMResouceStats_BatchUpload";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "WLMResouceStatsList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "WLMResouceStatsData";
			}
		}

		protected override void LogUploadStoredProcedureHandlerError(SqlQueryFailedException ex)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error uploading WLM Resource Statistics logs. Will attempt again next cycle.", new object[0]);
			throw new UploadWLMResourceStatsLogInBatchFailureException(ex.InnerException);
		}

		protected override void LogInsertCsvRowHandlerError(FormatException ex, CsvRow row)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error parsing WLM Resource Statistics log.", new object[0]);
		}

		public new const string KeyNameIsLogProcessorEnabled = "IsWLMResourceStatsLogProcessorEnabled";

		public const string LogTypeName = "WLMResourceStats";

		public const string RelativeLogPath = "Logging\\MailboxReplicationService\\WLMResourceStatss";
	}
}
