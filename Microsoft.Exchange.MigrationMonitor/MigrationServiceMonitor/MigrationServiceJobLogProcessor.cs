using System;
using System.Data;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor.MigrationServiceMonitor
{
	internal class MigrationServiceJobLogProcessor : MigrationServiceBaseLogProcessor
	{
		public MigrationServiceJobLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, "Logging\\MigrationReports\\MigrationJob"), "Migration Service Job Log", MigrationMonitor.MigrationServiceJobCsvSchemaInstance)
		{
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetMigrationJobLogUpdateTimestamp";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertMigrationServiceJob_BatchUploadV3";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "MigrationServiceJobList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "dbo.MigrationServiceJobDataV3";
			}
		}

		protected override string[] DistinctColumns
		{
			get
			{
				return new string[]
				{
					"JobId"
				};
			}
		}

		protected override void LogUploadStoredProcedureHandlerError(SqlQueryFailedException ex)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error updating migration job data to NewMan. Will attempt again next cycle.", new object[0]);
			throw new UploadMigrationJobInBatchFailureException(ex.InnerException);
		}

		protected override void LogInsertCsvRowHandlerError(FormatException ex, CsvRow row)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error parsing migration job log, job id is {0}", new object[]
			{
				MigMonUtilities.GetColumnValue<Guid>(row, "JobId")
			});
		}

		protected override bool TryAddSchemaSpecificDataRowValues(DataRow dataRow, CsvRow row)
		{
			dataRow["TargetDeliveryDomain"] = MigMonUtilities.TruncateMessage(MigMonUtilities.GetColumnStringValue(row, "TargetDeliveryDomain"), 500);
			dataRow[MigrationServiceJobCsvSchema.SourceEndpointGuid.DataTableKeyColumnName] = DBNull.Value;
			dataRow[MigrationServiceJobCsvSchema.TargetEndpointGuid.DataTableKeyColumnName] = DBNull.Value;
			string columnStringValue = MigMonUtilities.GetColumnStringValue(row, MigrationServiceJobCsvSchema.SourceEndpointGuid.ColumnName);
			if (!string.IsNullOrWhiteSpace(columnStringValue))
			{
				dataRow[MigrationServiceJobCsvSchema.SourceEndpointGuid.DataTableKeyColumnName] = MigMonUtilities.GetEndpointId(columnStringValue);
			}
			string columnStringValue2 = MigMonUtilities.GetColumnStringValue(row, MigrationServiceJobCsvSchema.TargetEndpointGuid.ColumnName);
			if (!string.IsNullOrWhiteSpace(columnStringValue2))
			{
				dataRow[MigrationServiceJobCsvSchema.TargetEndpointGuid.DataTableKeyColumnName] = MigMonUtilities.GetEndpointId(columnStringValue2);
			}
			dataRow["InitialSyncDuration"] = DBNull.Value;
			return base.TryAddMigrationServiceCommonDataRowValues(dataRow, row);
		}

		private const string LogTypeName = "Migration Service Job Log";
	}
}
