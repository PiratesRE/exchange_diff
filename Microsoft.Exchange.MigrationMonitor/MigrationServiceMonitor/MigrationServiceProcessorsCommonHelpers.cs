using System;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor.MigrationServiceMonitor
{
	internal static class MigrationServiceProcessorsCommonHelpers
	{
		public const string LocalizedErrorColumn = "LocalizedError";

		public const string InternalErrorColumn = "InternalError";

		public const string ObjectVersionColumn = "ObjectVersion";

		public static readonly ColumnDefinition<int> TenantName = new ColumnDefinition<int>("TenantName", "TenantNameId", KnownStringType.TenantName);

		public static readonly ColumnDefinition<int> MigrationType = new ColumnDefinition<int>("MigrationType", "MigrationTypeId", KnownStringType.MigrationType);

		public static readonly ColumnDefinition<int> Status = new ColumnDefinition<int>("Status", "MigrationStatusId", KnownStringType.MigrationStatus);

		public static readonly ColumnDefinition<int> WatsonHash = new ColumnDefinition<int>("WatsonHash", "WatsonHashId", KnownStringType.WatsonHash);
	}
}
