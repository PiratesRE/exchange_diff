using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public interface IPendingRequestNotifier
	{
		event DataAvailableEventHandler DataAvailable;

		bool ShouldThrottle { get; }

		string SubscriptionId { get; }

		string ContextKey { get; }

		IList<NotificationPayloadBase> ReadDataAndResetState();
	}
}
