using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;

namespace Microsoft.Exchange.Transport.Agent.SharedMailboxSentItemsRoutingAgent
{
	internal sealed class SharedMailboxSentItemsRoutingFactory : RoutingAgentFactory
	{
		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			ITracer sharedMailboxSentItemsAgentTracer = ExTraceGlobals.SharedMailboxSentItemsAgentTracer;
			return new SharedMailboxSentItemsRoutingAgent(new SharedMailboxConfigurationFactory(), new SentItemWrapperCreator(sharedMailboxSentItemsAgentTracer), sharedMailboxSentItemsAgentTracer);
		}
	}
}
