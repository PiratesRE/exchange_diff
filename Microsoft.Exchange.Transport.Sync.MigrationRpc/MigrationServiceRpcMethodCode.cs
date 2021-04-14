using System;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	internal enum MigrationServiceRpcMethodCode
	{
		None = 1,
		CreateSyncSubscription,
		UpdateSyncSubscription,
		GetSyncSubscriptionState,
		RegisterMigrationBatch,
		SubscriptionStatusChanged
	}
}
