using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DatabaseMountDialOverride
	{
		[LocDescription(DirectoryStrings.IDs.MountDialOverrideNone)]
		None = -1,
		[LocDescription(DirectoryStrings.IDs.MountDialOverrideLossless)]
		Lossless,
		[LocDescription(DirectoryStrings.IDs.MountDialOverrideGoodAvailability)]
		GoodAvailability = 6,
		[LocDescription(DirectoryStrings.IDs.MountDialOverrideBestAvailability)]
		BestAvailability = 12,
		[LocDescription(DirectoryStrings.IDs.MountDialOverrideBestEffort)]
		BestEffort = 10
	}
}
