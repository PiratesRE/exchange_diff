using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class RemoteSubscriptionEventArgs : EventArgs
	{
		public RemoteSubscriptionEventArgs(string contextKey, string subscriptionId)
		{
			this.ContextKey = contextKey;
			this.SubscriptionId = subscriptionId;
		}

		public string ContextKey { get; private set; }

		public string SubscriptionId { get; private set; }
	}
}
