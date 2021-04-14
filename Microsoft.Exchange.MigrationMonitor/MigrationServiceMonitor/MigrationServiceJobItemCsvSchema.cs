using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor.MigrationServiceMonitor
{
	internal class MigrationServiceJobItemCsvSchema : BaseMigMonCsvSchema
	{
		public MigrationServiceJobItemCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(MigrationServiceJobItemCsvSchema.requiredColumnsIds, MigrationServiceJobItemCsvSchema.requiredColumnsAsIs, "Time"), BaseMigMonCsvSchema.GetOptionalColumns(MigrationServiceJobItemCsvSchema.optionalColumnsIds, MigrationServiceJobItemCsvSchema.optionalColumnsAsIs), null)
		{
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return MigrationServiceJobItemCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return MigrationServiceJobItemCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return MigrationServiceJobItemCsvSchema.optionalColumnsIds;
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return MigrationServiceJobItemCsvSchema.optionalColumnsAsIs;
		}

		public const string MigrationJobItemGuidColumn = "JobItemGuid";

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>();

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<Guid>("JobItemGuid"),
			new ColumnDefinition<string>("SubscriptionId")
		};

		private static readonly List<ColumnDefinition<int>> optionalColumnsIds = new List<ColumnDefinition<int>>
		{
			MigrationServiceProcessorsCommonHelpers.TenantName,
			MigrationServiceProcessorsCommonHelpers.MigrationType,
			MigrationServiceProcessorsCommonHelpers.Status,
			MigrationServiceProcessorsCommonHelpers.WatsonHash
		};

		private static readonly List<IColumnDefinition> optionalColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<int>("ObjectVersion"),
			new ColumnDefinition<string>("LocalizedError"),
			new ColumnDefinition<string>("InternalError"),
			new ColumnDefinition<Guid>("MigrationJobId"),
			new ColumnDefinition<long>("ItemsSynced"),
			new ColumnDefinition<long>("ItemsSkipped"),
			new ColumnDefinition<long>("OverallCmdletDuration"),
			new ColumnDefinition<long>("SubscriptionInjectionDuration"),
			new ColumnDefinition<long>("ProvisioningDuration"),
			new ColumnDefinition<SqlDateTime>("ProvisionedTime"),
			new ColumnDefinition<SqlDateTime>("SubscriptionQueuedTime")
		};
	}
}
