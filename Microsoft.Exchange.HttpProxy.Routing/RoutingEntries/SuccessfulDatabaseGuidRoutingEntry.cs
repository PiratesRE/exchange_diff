using System;
using Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingEntries
{
	internal class SuccessfulDatabaseGuidRoutingEntry : DatabaseGuidRoutingEntry
	{
		public SuccessfulDatabaseGuidRoutingEntry(DatabaseGuidRoutingKey key, ServerRoutingDestination destination, long timestamp) : base(key, timestamp)
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

		private readonly ServerRoutingDestination destination;
	}
}
