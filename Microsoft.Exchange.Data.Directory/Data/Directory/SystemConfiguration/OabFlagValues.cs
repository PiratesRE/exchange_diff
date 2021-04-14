using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum OabFlagValues
	{
		None = 0,
		PublicFolderDistributionEnabled = 1,
		GlobalWebDistributionEnabled = 2,
		ShadowMailboxDistributionEnabled = 4
	}
}
