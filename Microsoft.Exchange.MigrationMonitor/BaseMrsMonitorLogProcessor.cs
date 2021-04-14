using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal abstract class BaseMrsMonitorLogProcessor : BaseLogProcessor
	{
		protected BaseMrsMonitorLogProcessor(string logDirectoryPath, string logFileTypeName, BaseMigMonCsvSchema csvSchemaInstance) : base(logDirectoryPath, logFileTypeName, csvSchemaInstance, null)
		{
		}

		protected override void LogInsertCsvRowHandlerError(FormatException ex, CsvRow row)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error parsing MRS log, request guid is {0}", new object[]
			{
				MigMonUtilities.GetColumnValue<Guid>(row, "RequestGuid")
			});
		}

		protected override void LogUploadStoredProcedureHandlerError(SqlQueryFailedException ex)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error updating MRS log to NewMan. Will attempt again next cycle.", new object[0]);
			throw new UploadMrsLogInBatchFailureException(ex.InnerException);
		}

		public new const string KeyNameIsLogProcessorEnabled = "IsMRSLogProcessorEnabled";
	}
}
