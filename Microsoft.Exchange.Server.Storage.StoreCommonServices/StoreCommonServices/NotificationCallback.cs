using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public delegate void NotificationCallback(NotificationPublishPhase phase, Context transactionContext, NotificationEvent nev);
}
