using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class ExchangeServiceSubscription : DisposeTrackableBase
	{
		protected ExchangeServiceSubscription(string subscriptionId)
		{
			this.SubscriptionId = subscriptionId;
		}

		internal virtual void HandleNotification(Notification notification)
		{
			throw new NotImplementedException();
		}

		internal string SubscriptionId { get; private set; }
	}
}
