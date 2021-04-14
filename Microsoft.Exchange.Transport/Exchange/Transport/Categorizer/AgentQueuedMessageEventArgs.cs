using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class AgentQueuedMessageEventArgs : QueuedMessageEventArgs
	{
		public AgentQueuedMessageEventArgs(MailItem message)
		{
			this.message = message;
		}

		public override MailItem MailItem
		{
			get
			{
				return this.message;
			}
		}

		private MailItem message;
	}
}
