using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	public enum SyncPhase
	{
		Initial,
		Incremental,
		Finalization,
		Completed,
		Delete
	}
}
