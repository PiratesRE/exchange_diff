using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Core
{
	internal interface ISubscriptionEventHandler
	{
		bool IsDisposed { get; }

		void EventsAvailable(StreamingSubscription subscription);

		void DisconnectSubscription(StreamingSubscription subscription, LocalizedException exception);
	}
}
