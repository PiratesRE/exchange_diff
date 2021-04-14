using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum AccountPartitionStateFlags
	{
		None = 0,
		IsLocalForest = 1,
		EnabledForProvisioning = 2,
		SecondaryAccountPartition = 4
	}
}
