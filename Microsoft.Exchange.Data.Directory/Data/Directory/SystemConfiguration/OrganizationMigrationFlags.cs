using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum OrganizationMigrationFlags
	{
		None = 0,
		IsExcludedFromOnboardMigration = 1,
		IsExcludedFromOffboardMigration = 2,
		IsFfoMigrationInProgress = 4
	}
}
