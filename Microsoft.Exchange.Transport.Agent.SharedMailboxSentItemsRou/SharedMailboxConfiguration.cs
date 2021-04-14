using System;

namespace Microsoft.Exchange.Transport.Agent.SharedMailboxSentItemsRoutingAgent
{
	internal class SharedMailboxConfiguration
	{
		public SharedMailboxConfiguration(bool isSharedMailbox, SharedMailboxSentItemBehavior sentAsBehavior, SharedMailboxSentItemBehavior sentOnBehalfOfBehavior)
		{
			this.IsSharedMailbox = isSharedMailbox;
			this.SentAsBehavior = sentAsBehavior;
			this.SentOnBehalfOfBehavior = sentOnBehalfOfBehavior;
		}

		public bool IsSharedMailbox { get; private set; }

		public SharedMailboxSentItemBehavior SentAsBehavior { get; private set; }

		public SharedMailboxSentItemBehavior SentOnBehalfOfBehavior { get; private set; }
	}
}
