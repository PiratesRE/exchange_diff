using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class DatabaseInfoCsvSchema : BaseMigMonCsvSchema
	{
		public DatabaseInfoCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(DatabaseInfoCsvSchema.requiredColumnsIds, DatabaseInfoCsvSchema.requiredColumnsAsIs, "Time"), BaseMigMonCsvSchema.GetOptionalColumns(DatabaseInfoCsvSchema.optionalColumnsIds, DatabaseInfoCsvSchema.optionalColumnsAsIs), null)
		{
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return DatabaseInfoCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return DatabaseInfoCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return DatabaseInfoCsvSchema.optionalColumnsIds;
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return DatabaseInfoCsvSchema.optionalColumnsAsIs;
		}

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("DatabaseName", "DatabaseId", KnownStringType.DatabaseName)
		};

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<bool>("IsExcludedFromProvisioning"),
			new ColumnDefinition<bool>("IsExcludedFromProvisioningBySpaceMonitoring"),
			new ColumnDefinition<bool>("IsSuspendedFromProvisioning"),
			new ColumnDefinition<bool>("IsExcludedFromInitialProvisioning"),
			new ColumnDefinition<double>("DatabaseSizeInM"),
			new ColumnDefinition<double>("AvailableNewMailboxSpaceInM"),
			new ColumnDefinition<double>("ConnectedPhysicalSizeInM"),
			new ColumnDefinition<double>("ConnectedLogicalSizeInM"),
			new ColumnDefinition<double>("DisconnectedPhysicalSizeInM"),
			new ColumnDefinition<double>("DisconnectedLogicalSizeInM"),
			new ColumnDefinition<int>("ConnectedMbxCount"),
			new ColumnDefinition<int>("DisconnectedMbxCount"),
			new ColumnDefinition<double>("MoveDestinationPhysicalSizeInM"),
			new ColumnDefinition<double>("MoveDestinationLogicalSizeInM"),
			new ColumnDefinition<int>("MoveDestinationMbxCount"),
			new ColumnDefinition<double>("SoftDeletedPhysicalSizeInM"),
			new ColumnDefinition<double>("SoftDeletedLogicalSizeInM"),
			new ColumnDefinition<int>("SoftDeletedMbxCount")
		};

		private static readonly List<ColumnDefinition<int>> optionalColumnsIds = new List<ColumnDefinition<int>>();

		private static readonly List<IColumnDefinition> optionalColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<double>("DisconnectedIn48HoursMbxPhysicalSize"),
			new ColumnDefinition<int>("DisconnectedIn48HoursMbxCount")
		};
	}
}
