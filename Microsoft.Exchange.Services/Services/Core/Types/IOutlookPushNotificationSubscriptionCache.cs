using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class IOutlookPushNotificationSubscriptionCache
	{
		internal abstract bool QueryMailboxSubscriptions(string notificationContext, out string tenantId, out List<OutlookServiceNotificationSubscription> activeSubscriptions);
	}
}
