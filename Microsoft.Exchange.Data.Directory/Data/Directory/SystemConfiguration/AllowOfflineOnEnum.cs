using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum AllowOfflineOnEnum
	{
		[LocDescription(DirectoryStrings.IDs.PrivateComputersOnly)]
		PrivateComputersOnly = 1,
		[LocDescription(DirectoryStrings.IDs.NoComputers)]
		NoComputers,
		[LocDescription(DirectoryStrings.IDs.AllComputers)]
		AllComputers
	}
}
