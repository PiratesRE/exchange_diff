using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Worker.Agents
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SubscriptionAgentFactory
	{
		public virtual void Close()
		{
		}

		public abstract SubscriptionAgent CreateAgent();
	}
}
