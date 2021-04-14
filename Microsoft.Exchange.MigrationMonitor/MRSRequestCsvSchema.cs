using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class MRSRequestCsvSchema : BaseMrsMonitorCsvSchema
	{
		public MRSRequestCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(MRSRequestCsvSchema.requiredColumnsIds, MRSRequestCsvSchema.requiredColumnsAsIs, "Time"), BaseMigMonCsvSchema.GetOptionalColumns(MRSRequestCsvSchema.optionalColumnsIds, MRSRequestCsvSchema.optionalColumnsAsIs), null)
		{
		}

		public override DataTable GetCsvSchemaDataTable()
		{
			DataTable csvSchemaDataTable = base.GetCsvSchemaDataTable();
			csvSchemaDataTable.Columns.Add("RequestTypeId", typeof(int));
			return csvSchemaDataTable;
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return MRSRequestCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return MRSRequestCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return MRSRequestCsvSchema.optionalColumnsIds;
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return MRSRequestCsvSchema.optionalColumnsAsIs;
		}

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>();

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<Guid>("RequestGuid")
		};

		private static readonly List<ColumnDefinition<int>> optionalColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("UserOrgName", "UserOrgNameId", KnownStringType.TenantName),
			new ColumnDefinition<int>("Status", "StatusId", KnownStringType.RequestStatus),
			new ColumnDefinition<int>("StatusDetail", "StatusDetailId", KnownStringType.RequestStatusDetail),
			new ColumnDefinition<int>("Priority", "PriorityId", KnownStringType.RequestPriority),
			new ColumnDefinition<int>("BatchName", "BatchNameId", KnownStringType.RequestBatchName),
			new ColumnDefinition<int>("SourceVersion", "SourceVersionId", KnownStringType.Version),
			new ColumnDefinition<int>("SourceServer", "SourceServerId", KnownStringType.ServerName),
			new ColumnDefinition<int>("SourceDatabase", "SourceDatabaseId", KnownStringType.DatabaseName),
			new ColumnDefinition<int>("SourceArchiveDatabase", "SourceArchiveDatabaseId", KnownStringType.DatabaseName),
			new ColumnDefinition<int>("TargetVersion", "TargetVersionId", KnownStringType.Version),
			new ColumnDefinition<int>("TargetDatabase", "TargetDatabaseId", KnownStringType.DatabaseName),
			new ColumnDefinition<int>("TargetServer", "TargetServerId", KnownStringType.ServerName),
			new ColumnDefinition<int>("RemoteHostName", "RemoteHostNameId", KnownStringType.RemoteHostName),
			new ColumnDefinition<int>("RemoteDatabaseName", "RemoteDatabaseNameId", KnownStringType.DatabaseName),
			new ColumnDefinition<int>("TargetDeliveryDomain", "TargetDeliveryDomainId", KnownStringType.TargetDeliveryDomain),
			new ColumnDefinition<int>("MRSServerName", "MRSServerNameId", KnownStringType.ServerName),
			new ColumnDefinition<int>("FailureType", "FailureTypeId", KnownStringType.FailureType),
			new ColumnDefinition<int>("FailureSide", "FailureSideId", KnownStringType.FailureSide),
			new ColumnDefinition<int>("SyncStage", "SyncStageId", KnownStringType.RequestSyncStage),
			new ColumnDefinition<int>("JobType", "JobTypeId", KnownStringType.RequestJobType),
			new ColumnDefinition<int>("WorkloadType", "WorkloadTypeId", KnownStringType.RequestWorkloadType),
			new ColumnDefinition<int>("SyncProtocol", "SyncProtocolID", KnownStringType.SyncProtocol)
		};

		private static readonly List<IColumnDefinition> optionalColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<string>("Identity"),
			new ColumnDefinition<string>("Message"),
			new ColumnDefinition<SqlDateTime>("TS_Creation"),
			new ColumnDefinition<SqlDateTime>("TS_Start"),
			new ColumnDefinition<SqlDateTime>("TS_LastUpdate"),
			new ColumnDefinition<SqlDateTime>("TS_InitialSeedingCompleted"),
			new ColumnDefinition<SqlDateTime>("TS_FinalSync"),
			new ColumnDefinition<SqlDateTime>("TS_Completion"),
			new ColumnDefinition<SqlDateTime>("TS_Suspended"),
			new ColumnDefinition<SqlDateTime>("TS_Failure"),
			new ColumnDefinition<SqlDateTime>("TS_StartAfter"),
			new ColumnDefinition<int>("PercentComplete"),
			new ColumnDefinition<int>("BadItemLimit"),
			new ColumnDefinition<int>("BadItemsEncountered"),
			new ColumnDefinition<int>("TotalMailboxItemCount"),
			new ColumnDefinition<int>("ItemsTransferred"),
			new ColumnDefinition<int>("JobInternalFlags"),
			new ColumnDefinition<int>("FailureCode"),
			new ColumnDefinition<int>("LargeItemLimit"),
			new ColumnDefinition<int>("LargeItemsEncountered"),
			new ColumnDefinition<int>("MissingItemsEncountered"),
			new ColumnDefinition<int>("PoisonCount"),
			new ColumnDefinition<int>("Flags"),
			new ColumnDefinition<long>("Duration_OverallMove"),
			new ColumnDefinition<long>("Duration_Finalization"),
			new ColumnDefinition<long>("Duration_DataReplicationWait"),
			new ColumnDefinition<long>("Duration_Suspended"),
			new ColumnDefinition<long>("Duration_Failed"),
			new ColumnDefinition<long>("Duration_Queued"),
			new ColumnDefinition<long>("Duration_InProgress"),
			new ColumnDefinition<long>("Duration_StalledDueToCI"),
			new ColumnDefinition<long>("Duration_StalledDueToHA"),
			new ColumnDefinition<long>("Duration_StalledDueToReadThrottle"),
			new ColumnDefinition<long>("Duration_StalledDueToWriteThrottle"),
			new ColumnDefinition<long>("Duration_StalledDueToReadCpu"),
			new ColumnDefinition<long>("Duration_StalledDueToWriteCpu"),
			new ColumnDefinition<long>("Duration_StalledDueToMailboxLock"),
			new ColumnDefinition<long>("Duration_TransientFailure"),
			new ColumnDefinition<long>("Duration_Idle"),
			new ColumnDefinition<long>("TotalMailboxSize"),
			new ColumnDefinition<long>("BytesTransferred"),
			new ColumnDefinition<Guid>("ExchangeGuid"),
			new ColumnDefinition<Guid>("SourceExchangeGuid"),
			new ColumnDefinition<Guid>("TargetExchangeGuid"),
			new ColumnDefinition<string>("CancelRequest")
		};
	}
}
