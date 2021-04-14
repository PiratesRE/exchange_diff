using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum AutoDatabaseMountDial
	{
		[LocDescription(DirectoryStrings.IDs.AutoDatabaseMountDialLossless)]
		Lossless,
		[LocDescription(DirectoryStrings.IDs.AutoDatabaseMountDialGoodAvailability)]
		GoodAvailability = 6,
		[LocDescription(DirectoryStrings.IDs.AutoDatabaseMountDialBestAvailability)]
		BestAvailability = 12
	}
}
