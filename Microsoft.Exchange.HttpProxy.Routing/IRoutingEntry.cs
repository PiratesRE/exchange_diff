using System;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	public interface IRoutingEntry : IEquatable<IRoutingEntry>
	{
		IRoutingDestination Destination { get; }

		IRoutingKey Key { get; }

		long Timestamp { get; }
	}
}
