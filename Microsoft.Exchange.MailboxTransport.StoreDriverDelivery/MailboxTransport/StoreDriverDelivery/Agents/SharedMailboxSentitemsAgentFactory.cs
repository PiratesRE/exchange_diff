using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.SharedMailboxSentItems;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal sealed class SharedMailboxSentitemsAgentFactory : StoreDriverDeliveryAgentFactory
	{
		public override StoreDriverDeliveryAgent CreateAgent(SmtpServer server)
		{
			return new SharedMailboxSentItemsAgent(new PerformanceCountersFactory(), new Logger(SharedMailboxSentitemsAgentFactory.TracerInstance));
		}

		private static readonly Trace TracerInstance = ExTraceGlobals.SharedMailboxSentItemsAgentTracer;
	}
}
