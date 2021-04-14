using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor.MigrationServiceMonitor
{
	internal class MigrationServiceJobCsvSchema : BaseMigMonCsvSchema
	{
		public MigrationServiceJobCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(MigrationServiceJobCsvSchema.requiredColumnsIds, MigrationServiceJobCsvSchema.requiredColumnsAsIs, "Time"), BaseMigMonCsvSchema.GetOptionalColumns(MigrationServiceJobCsvSchema.optionalColumnsIds, MigrationServiceJobCsvSchema.optionalColumnsAsIs), null)
		{
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return MigrationServiceJobCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return MigrationServiceJobCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return MigrationServiceJobCsvSchema.optionalColumnsIds;
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return MigrationServiceJobCsvSchema.optionalColumnsAsIs;
		}

		public const string JobIdColumn = "JobId";

		public const string TargetDeliveryDomainColumn = "TargetDeliveryDomain";

		public const string InitialSyncDurationColumn = "InitialSyncDuration";

		public static readonly ColumnDefinition<int> SourceEndpointGuid = new ColumnDefinition<int>("SourceEndpointGuid", "SourceEndpointGuidId", KnownStringType.EndpointGuid);

		public static readonly ColumnDefinition<int> TargetEndpointGuid = new ColumnDefinition<int>("TargetEndpointGuid", "TargetEndpointGuidId", KnownStringType.EndpointGuid);

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>();

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<Guid>("JobId")
		};

		private static readonly List<ColumnDefinition<int>> optionalColumnsIds = new List<ColumnDefinition<int>>
		{
			MigrationServiceProcessorsCommonHelpers.TenantName,
			MigrationServiceProcessorsCommonHelpers.MigrationType,
			MigrationServiceProcessorsCommonHelpers.Status,
			MigrationServiceProcessorsCommonHelpers.WatsonHash,
			new ColumnDefinition<int>("Direction", "DirectionId", KnownStringType.MigrationDirection),
			new ColumnDefinition<int>("Locale", "LocaleId", KnownStringType.Locale),
			new ColumnDefinition<int>("BatchFlags", "BatchFlagsId", KnownStringType.MigrationBatchFlags),
			new ColumnDefinition<int>("SkipSteps", "SkipStepsId", KnownStringType.MigrationSkipSteps),
			MigrationServiceJobCsvSchema.SourceEndpointGuid,
			MigrationServiceJobCsvSchema.TargetEndpointGuid
		};

		private static readonly List<IColumnDefinition> optionalColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<SqlDateTime>("CreationTime"),
			new ColumnDefinition<SqlDateTime>("StartTime"),
			new ColumnDefinition<SqlDateTime>("InitialSyncTime"),
			new ColumnDefinition<SqlDateTime>("FinalizedTime"),
			new ColumnDefinition<SqlDateTime>("LastSyncTime"),
			new ColumnDefinition<SqlDateTime>("StartAfterTime"),
			new ColumnDefinition<SqlDateTime>("CompleteAfter"),
			new ColumnDefinition<string>("InitialSyncDuration"),
			new ColumnDefinition<int>("TotalCount"),
			new ColumnDefinition<int>("ActiveCount"),
			new ColumnDefinition<int>("StoppedCount"),
			new ColumnDefinition<int>("SyncedCount"),
			new ColumnDefinition<int>("FinalizedCount"),
			new ColumnDefinition<int>("FailedCount"),
			new ColumnDefinition<int>("PendingCount"),
			new ColumnDefinition<int>("ProvisionedCount"),
			new ColumnDefinition<int>("ValidationWarningCount"),
			new ColumnDefinition<int>("AutoRetryCount"),
			new ColumnDefinition<int>("CurrentRetryCount"),
			new ColumnDefinition<int>("BadItemLimit"),
			new ColumnDefinition<int>("LargeItemLimit"),
			new ColumnDefinition<bool>("PrimaryOnly"),
			new ColumnDefinition<bool>("ArchiveOnly"),
			new ColumnDefinition<string>("TargetDeliveryDomain"),
			new ColumnDefinition<int>("ObjectVersion"),
			new ColumnDefinition<string>("LocalizedError"),
			new ColumnDefinition<string>("InternalError"),
			new ColumnDefinition<long>("ProcessingDuration")
		};
	}
}
