using System;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingEntries
{
	internal class GenericRoutingEntry : RoutingEntryBase
	{
		public GenericRoutingEntry(IRoutingKey key, IRoutingDestination destination, long timestamp)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			this.key = key;
			this.destination = destination;
			this.timestamp = timestamp;
		}

		public override IRoutingDestination Destination
		{
			get
			{
				return this.destination;
			}
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

		private readonly IRoutingDestination destination;

		private readonly long timestamp;
	}
}
