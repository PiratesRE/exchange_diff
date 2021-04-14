using System;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	public interface IRoutingKey : IEquatable<IRoutingKey>
	{
		RoutingItemType RoutingItemType { get; }

		string Value { get; }
	}
}
