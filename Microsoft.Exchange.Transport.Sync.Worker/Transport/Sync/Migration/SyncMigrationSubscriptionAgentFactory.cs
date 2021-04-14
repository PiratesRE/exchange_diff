using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Worker.Agents;

namespace Microsoft.Exchange.Transport.Sync.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SyncMigrationSubscriptionAgentFactory : SubscriptionAgentFactory
	{
		public override SubscriptionAgent CreateAgent()
		{
			return new SyncMigrationSubscriptionAgent();
		}
	}
}
