using System;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	internal enum UpdateSyncSubscriptionAction
	{
		Disable = 1,
		Delete,
		Finalize
	}
}
