using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations
{
	public abstract class RoutingDestinationBase : IRoutingDestination, IEquatable<IRoutingDestination>
	{
		public abstract RoutingItemType RoutingItemType { get; }

		public abstract string Value { get; }

		public abstract IList<string> Properties { get; }

		public static bool operator ==(RoutingDestinationBase firstRoutingDestination, RoutingDestinationBase secondRoutingDestination)
		{
			return object.ReferenceEquals(firstRoutingDestination, secondRoutingDestination) || (!object.ReferenceEquals(firstRoutingDestination, null) && !object.ReferenceEquals(secondRoutingDestination, null) && (firstRoutingDestination.RoutingItemType == secondRoutingDestination.RoutingItemType && firstRoutingDestination.Value.Equals(secondRoutingDestination.Value, StringComparison.OrdinalIgnoreCase)) && firstRoutingDestination.Properties.SequenceEqual(secondRoutingDestination.Properties));
		}

		public static bool operator !=(RoutingDestinationBase firstRoutingDestination, RoutingDestinationBase secondRoutingDestination)
		{
			return !(firstRoutingDestination == secondRoutingDestination);
		}

		public bool Equals(IRoutingDestination other)
		{
			return other as RoutingDestinationBase == this;
		}

		public override bool Equals(object obj)
		{
			return this.Equals((IRoutingDestination)obj);
		}

		public override int GetHashCode()
		{
			return this.RoutingItemType.GetHashCode() ^ this.Value.GetHashCode();
		}
	}
}
