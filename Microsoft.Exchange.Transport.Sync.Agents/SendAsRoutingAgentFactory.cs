using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.SendAs
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SendAsRoutingAgentFactory : RoutingAgentFactory
	{
		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new SendAsRoutingAgent(this.sendAsManager);
		}

		private SendAsManager sendAsManager = new SendAsManager();
	}
}
