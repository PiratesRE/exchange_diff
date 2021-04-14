using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	public sealed class TextMessagingRoutingAgentFactory : RoutingAgentFactory
	{
		public TextMessagingRoutingAgentFactory()
		{
			this.session = new MobileSession();
		}

		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new TextMessagingRoutingAgent(this.session);
		}

		private MobileSession session;
	}
}
