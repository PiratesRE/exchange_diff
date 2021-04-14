using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor.MigrationServiceMonitor
{
	internal class MigrationServiceEndpointCsvSchema : BaseMigMonCsvSchema
	{
		public MigrationServiceEndpointCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(MigrationServiceEndpointCsvSchema.requiredColumnsIds, MigrationServiceEndpointCsvSchema.requiredColumnsAsIs, "Time"), BaseMigMonCsvSchema.GetOptionalColumns(MigrationServiceEndpointCsvSchema.optionalColumnsIds, MigrationServiceEndpointCsvSchema.optionalColumnsAsIs), null)
		{
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return MigrationServiceEndpointCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return MigrationServiceEndpointCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return MigrationServiceEndpointCsvSchema.optionalColumnsIds;
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return MigrationServiceEndpointCsvSchema.optionalColumnsAsIs;
		}

		public const string EndpointGuidColumn = "EndpointGuid";

		public const string EndpointNameColumn = "EndpointName";

		public const string EndpointRemoteColumn = "EndpointRemoteServer";

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>();

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<Guid>("EndpointGuid")
		};

		private static readonly List<ColumnDefinition<int>> optionalColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("EndpointType", "EndpointTypeId", KnownStringType.MigrationType),
			new ColumnDefinition<int>("EndpointMailboxPermission", "EndpointMailboxPermissionId", KnownStringType.EndpointPermission),
			new ColumnDefinition<int>("EndpointState", "EndpointStateId", KnownStringType.EndpointState)
		};

		private static readonly List<IColumnDefinition> optionalColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<string>("EndpointName"),
			new ColumnDefinition<string>("EndpointRemoteServer"),
			new ColumnDefinition<int>("EndpointMaxConcurrentMigrations"),
			new ColumnDefinition<int>("EndpointMaxConcurrentIncrementalSyncs"),
			new ColumnDefinition<SqlDateTime>("EndpointLastModifiedTime")
		};
	}
}
