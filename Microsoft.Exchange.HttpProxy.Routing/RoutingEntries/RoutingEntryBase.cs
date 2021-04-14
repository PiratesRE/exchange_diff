using System;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingEntries
{
	public abstract class RoutingEntryBase : IRoutingEntry, IEquatable<IRoutingEntry>
	{
		public abstract IRoutingDestination Destination { get; }

		public abstract IRoutingKey Key { get; }

		public abstract long Timestamp { get; }

		public static bool operator ==(RoutingEntryBase firstRoutingEntry, RoutingEntryBase secondRoutingEntry)
		{
			return object.ReferenceEquals(firstRoutingEntry, secondRoutingEntry) || (!object.ReferenceEquals(firstRoutingEntry, null) && !object.ReferenceEquals(secondRoutingEntry, null) && firstRoutingEntry.Key.Equals(secondRoutingEntry.Key) && firstRoutingEntry.Destination.Equals(secondRoutingEntry.Destination));
		}

		public static bool operator !=(RoutingEntryBase firstRoutingEntry, RoutingEntryBase secondRoutingEntry)
		{
			return !(firstRoutingEntry == secondRoutingEntry);
		}

		public bool Equals(IRoutingEntry other)
		{
			return other as RoutingEntryBase == this;
		}

		public override bool Equals(object obj)
		{
			return this.Equals((IRoutingEntry)obj);
		}

		public override int GetHashCode()
		{
			return this.Key.GetHashCode() ^ this.Destination.GetHashCode();
		}
	}
}
