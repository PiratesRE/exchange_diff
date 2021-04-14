using System;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingKeys
{
	public abstract class RoutingKeyBase : IRoutingKey, IEquatable<IRoutingKey>
	{
		public abstract RoutingItemType RoutingItemType { get; }

		public abstract string Value { get; }

		public static bool operator ==(RoutingKeyBase firstRoutingKey, RoutingKeyBase secondRoutingKey)
		{
			return object.ReferenceEquals(firstRoutingKey, secondRoutingKey) || (!object.ReferenceEquals(firstRoutingKey, null) && !object.ReferenceEquals(secondRoutingKey, null) && firstRoutingKey.RoutingItemType == secondRoutingKey.RoutingItemType && firstRoutingKey.Value.Equals(secondRoutingKey.Value, StringComparison.OrdinalIgnoreCase));
		}

		public static bool operator !=(RoutingKeyBase firstRoutingKey, RoutingKeyBase secondRoutingKey)
		{
			return !(firstRoutingKey == secondRoutingKey);
		}

		public bool Equals(IRoutingKey other)
		{
			return other as RoutingKeyBase == this;
		}

		public override bool Equals(object obj)
		{
			return this.Equals((IRoutingKey)obj);
		}

		public override int GetHashCode()
		{
			return this.RoutingItemType.GetHashCode() ^ this.Value.GetHashCode();
		}
	}
}
