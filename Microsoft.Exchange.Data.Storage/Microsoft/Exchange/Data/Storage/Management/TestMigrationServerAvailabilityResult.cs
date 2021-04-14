using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public enum TestMigrationServerAvailabilityResult
	{
		[LocDescription(ServerStrings.IDs.MigrationTestMSASuccess)]
		Success,
		[LocDescription(ServerStrings.IDs.MigrationTestMSAFailed)]
		Failed,
		[LocDescription(ServerStrings.IDs.MigrationTestMSAWarning)]
		Warning
	}
}
