using System;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	public enum MigrationSubscriptionStatus
	{
		None = 1,
		InvalidPathPrefix,
		MailboxNotFound,
		RpcThresholdExceeded,
		SubscriptionNotFound
	}
}
