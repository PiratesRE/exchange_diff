using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public enum MigrationBatchDirection
	{
		[LocDescription(ServerStrings.IDs.MigrationBatchDirectionLocal)]
		Local = 1,
		[LocDescription(ServerStrings.IDs.MigrationBatchDirectionOnboarding)]
		Onboarding,
		[LocDescription(ServerStrings.IDs.MigrationBatchDirectionOffboarding)]
		Offboarding = 4
	}
}
