using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Worker.Agents
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PeopleConnectSubscriptionAgentFactory : SubscriptionAgentFactory
	{
		public override SubscriptionAgent CreateAgent()
		{
			return new PeopleConnectSubscriptionAgent();
		}
	}
}
