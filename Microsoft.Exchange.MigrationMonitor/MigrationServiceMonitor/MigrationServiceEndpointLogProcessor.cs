using System;
using System.Data;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor.MigrationServiceMonitor
{
	internal class MigrationServiceEndpointLogProcessor : MigrationServiceBaseLogProcessor
	{
		public MigrationServiceEndpointLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, "Logging\\MigrationReports\\MigrationEndpoint"), "Migration Service Endpoint Log", MigrationMonitor.MigrationServiceEndpointCsvSchemaInstance)
		{
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetMigrationEndpointLogUpdateTimestamp";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertMigrationServiceEndpoint_BatchUpload";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "MigrationServiceEndpointList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "dbo.MigrationServiceEndpointData";
			}
		}

		protected override string[] DistinctColumns
		{
			get
			{
				return new string[]
				{
					"EndpointGuid"
				};
			}
		}

		protected override void LogUploadStoredProcedureHandlerError(SqlQueryFailedException ex)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error updating migration endpoint data to NewMan. Will attempt again next cycle.", new object[0]);
			throw new UploadMigrationEndpointInBatchFailureException(ex.InnerException);
		}

		protected override void LogInsertCsvRowHandlerError(FormatException ex, CsvRow row)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error parsing migration endpoint log, endpoint guid is {0}", new object[]
			{
				MigMonUtilities.GetColumnValue<Guid>(row, "EndpointGuid")
			});
		}

		protected override bool TryAddSchemaSpecificDataRowValues(DataRow dataRow, CsvRow row)
		{
			base.TryAddSimpleOptionalKnownStrings(dataRow, row);
			dataRow["EndpointName"] = MigMonUtilities.TruncateMessage(MigMonUtilities.GetColumnStringValue(row, "EndpointName"), 500);
			dataRow["EndpointRemoteServer"] = MigMonUtilities.TruncateMessage(MigMonUtilities.GetColumnStringValue(row, "EndpointRemoteServer"), 500);
			return true;
		}

		private const string LogTypeName = "Migration Service Endpoint Log";
	}
}
