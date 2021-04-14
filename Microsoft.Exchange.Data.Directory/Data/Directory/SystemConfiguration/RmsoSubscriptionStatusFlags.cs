using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum RmsoSubscriptionStatusFlags
	{
		Unknown,
		Enabled,
		Suspended,
		LockedOut,
		AdhocEnabled,
		Deleted = 7
	}
}
