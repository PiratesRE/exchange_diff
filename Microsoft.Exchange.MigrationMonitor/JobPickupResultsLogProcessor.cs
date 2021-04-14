using System;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class JobPickupResultsLogProcessor : SlowMRSDetectorBaseLogProcessor
	{
		public JobPickupResultsLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, "Logging\\MailboxReplicationService\\JobPickupResults"), "JobPickupResults", MigrationMonitor.JobPickupResultsCsvSchemaInstance, "JobPickupResults*.LOG")
		{
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetJobPickupResultsUpdateTimestamp";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertJobPickupResults_BatchUpload";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "JobPickupResultsList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "JobPickupResultsData";
			}
		}

		protected override void LogUploadStoredProcedureHandlerError(SqlQueryFailedException ex)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error uploading Job Pickup Results logs. Will attempt again next cycle.", new object[0]);
			throw new UploadJobPickupResultsLogInBatchFailureException(ex.InnerException);
		}

		protected override void LogInsertCsvRowHandlerError(FormatException ex, CsvRow row)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error parsing Job Pickup Results log.", new object[0]);
		}

		public new const string KeyNameIsLogProcessorEnabled = "IsJobPickupResultsLogProcessorEnabled";

		public const string LogTypeName = "JobPickupResults";

		public const string RelativeLogPath = "Logging\\MailboxReplicationService\\JobPickupResults";
	}
}
