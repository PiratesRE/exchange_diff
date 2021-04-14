using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IExchangeTopologyBridge
	{
		TimeSpan CacheTimerRefreshTimeout { get; }

		TimeSpan CacheExpirationTimeout { get; }

		TimeSpan GetServiceTopologyDefaultTimeout { get; }

		TimeSpan NotificationDelayTimeout { get; }

		TimeSpan MinExpirationTimeForCacheDueToCacheMiss { get; }

		ExchangeTopology ReadExchangeTopology(DateTime timestamp, ExchangeTopologyScope topologyScope, bool forceRefresh);

		IRegisteredExchangeTopologyNotification RegisterExchangeTopologyNotification(ADNotificationCallback callback, ExchangeTopologyScope scope);
	}
}
