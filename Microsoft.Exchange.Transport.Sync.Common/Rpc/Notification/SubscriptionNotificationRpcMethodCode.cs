using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Rpc.Notification
{
	internal enum SubscriptionNotificationRpcMethodCode
	{
		None = 1,
		SubscriptionAdd,
		SubscriptionDelete,
		SubscriptionSyncNowNeeded,
		SubscriptionUpdated,
		SubscriptionUpdatedAndSyncNowNeeded,
		OWALogonTriggeredSyncNow,
		OWAActivityTriggeredSyncNow,
		OWARefreshButtonTriggeredSyncNow
	}
}
