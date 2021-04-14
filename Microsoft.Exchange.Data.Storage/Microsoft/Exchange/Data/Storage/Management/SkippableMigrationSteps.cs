using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Flags]
	[Serializable]
	public enum SkippableMigrationSteps
	{
		[LocDescription(ServerStrings.IDs.MigrationSkippableStepNone)]
		None = 0,
		[LocDescription(ServerStrings.IDs.MigrationSkippableStepSettingTargetAddress)]
		SettingTargetAddress = 1
	}
}
