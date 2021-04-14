using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Transport.StoreDriver
{
	internal abstract class StoreDriverDeliveryEventArgs : StoreDriverEventArgs
	{
		internal StoreDriverDeliveryEventArgs()
		{
		}

		public abstract DeliverableMailItem MailItem { get; }

		public abstract RoutingAddress RecipientAddress { get; }

		public abstract string MessageClass { get; }

		public abstract void AddAgentInfo(string agentName, string eventName, List<KeyValuePair<string, string>> data);
	}
}
