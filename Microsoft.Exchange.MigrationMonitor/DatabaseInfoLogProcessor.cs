using System;
using System.Data;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class DatabaseInfoLogProcessor : StatsAndInfoCommonBaseLogProcessor
	{
		public DatabaseInfoLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, MigrationMonitor.MigrationMonitorContext.Config.GetConfig<string>("MbxDBStatsFolder")), "DBMailboxStats Log", MigrationMonitor.DatabaseInfoCsvSchemaInstance, MigrationMonitor.MigrationMonitorContext.Config.GetConfig<string>("DBStatsFileName"))
		{
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetDataBaseInfoUpdateTimestampV2";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertDataBaseInfo_BatchUploadV3";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "DatabaseInfoList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "dbo.DatabaseInfoDataV3";
			}
		}

		protected override bool TryAddSchemaSpecificDataRowValues(DataRow dataRow, CsvRow row)
		{
			return base.TryAddDatabaseIdKeyValue(dataRow);
		}

		protected override void LogUploadStoredProcedureHandlerError(SqlQueryFailedException ex)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error updating information to NewMan for database {0}. Will attempt again next cycle.", new object[]
			{
				base.CurrentDatabaseName
			});
			throw new UploadDatabaseInfoInBatchFailureException(ex.InnerException);
		}

		protected override void LogInsertCsvRowHandlerError(FormatException ex, CsvRow row)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error parsing information log for database {0}", new object[]
			{
				base.CurrentDatabaseName
			});
		}

		public new const string KeyNameIsLogProcessorEnabled = "IsDatabaseInfoLogProcessorEnabled";

		public const string KeyNameMbxDBStatsFolder = "MbxDBStatsFolder";

		public const string DefaultMbxDBStatsFolder = "Logging\\CompleteMailboxStats";

		public const string KeyNameDBStatsFileName = "DBStatsFileName";

		public const string DefaultDBStatsFileNamePattern = "*DBStats*.log";

		private const string LogTypeName = "DBMailboxStats Log";
	}
}
