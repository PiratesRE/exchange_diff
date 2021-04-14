using System;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingEntries
{
	internal abstract class DatabaseGuidRoutingEntry : RoutingEntryBase
	{
		protected DatabaseGuidRoutingEntry(DatabaseGuidRoutingKey key, long timestamp)
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

		private readonly DatabaseGuidRoutingKey key;

		private readonly long timestamp;
	}
}
