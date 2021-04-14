using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationService
	{
		CreateSyncSubscriptionResult CreateSyncSubscription(AbstractCreateSyncSubscriptionArgs args);

		UpdateSyncSubscriptionResult UpdateSyncSubscription(UpdateSyncSubscriptionArgs args);

		GetSyncSubscriptionStateResult GetSyncSubscriptionState(GetSyncSubscriptionStateArgs args);
	}
}
