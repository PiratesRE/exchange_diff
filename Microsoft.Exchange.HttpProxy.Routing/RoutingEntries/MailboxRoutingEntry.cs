using System;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingEntries
{
	internal abstract class MailboxRoutingEntry : RoutingEntryBase
	{
		protected MailboxRoutingEntry(IRoutingKey key, long timestamp)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.key = key;
			this.timestamp = timestamp;
		}

		public override IRoutingKey Key
		{
			get
			{
				return this.key;
			}
		}

		public override long Timestamp
		{
			get
			{
				return this.timestamp;
			}
		}

		private readonly IRoutingKey key;

		private readonly long timestamp;
	}
}
