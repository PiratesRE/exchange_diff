using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum SoftDeletedFeatureStatusFlags
	{
		Disabled = 0,
		EDUEnabled = 1,
		MSOEnabled = 2,
		ForwardSyncEnabled = 4
	}
}
