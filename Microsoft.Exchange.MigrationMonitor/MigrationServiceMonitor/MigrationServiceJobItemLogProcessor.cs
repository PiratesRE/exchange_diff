using System;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor.MigrationServiceMonitor
{
	internal class MigrationServiceJobItemLogProcessor : MigrationServiceBaseLogProcessor
	{
		public MigrationServiceJobItemLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, "Logging\\MigrationReports\\MigrationJobItem"), "Migration Service Job Item Log", MigrationMonitor.MigrationServiceJobItemCsvSchemaInstance)
		{
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetMigrationJobItemLogUpdateTimestamp";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertMigrationServiceJobItem_BatchUploadV3";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "MigrationServiceJobItemList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "dbo.MigrationServiceJobItemDataV3";
			}
		}

		protected override string[] DistinctColumns
		{
			get
			{
				return new string[]
				{
					"JobItemGuid"
				};
			}
		}

		protected override void LogUploadStoredProcedureHandlerError(SqlQueryFailedException ex)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error updating migration job item data to NewMan. Will attempt again next cycle.", new object[0]);
			throw new UploadMigrationJobItemInBatchFailureException(ex.InnerException);
		}

		protected override void LogInsertCsvRowHandlerError(FormatException ex, CsvRow row)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error parsing migration job item log, job item id is {0}", new object[]
			{
				MigMonUtilities.GetColumnValue<Guid>(row, "JobItemGuid")
			});
		}

		private const string LogTypeName = "Migration Service Job Item Log";
	}
}
