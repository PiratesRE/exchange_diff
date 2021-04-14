using System;
using Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingEntries
{
	internal class SuccessfulMailboxRoutingEntry : MailboxRoutingEntry
	{
		public SuccessfulMailboxRoutingEntry(IRoutingKey key, DatabaseGuidRoutingDestination destination, long timestamp) : base(key, timestamp)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			this.destination = destination;
		}

		public override IRoutingDestination Destination
		{
			get
			{
				return this.destination;
			}
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private readonly DatabaseGuidRoutingDestination destination;
	}
}
