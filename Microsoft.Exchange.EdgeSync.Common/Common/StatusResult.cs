using System;

namespace Microsoft.Exchange.EdgeSync.Common
{
	public enum StatusResult
	{
		InProgress,
		Success,
		Aborted,
		CouldNotConnect,
		CouldNotLease,
		LostLease,
		Incomplete,
		NoSubscriptions,
		SyncDisabled
	}
}
