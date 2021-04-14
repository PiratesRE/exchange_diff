using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	public interface IRoutingDestination : IEquatable<IRoutingDestination>
	{
		RoutingItemType RoutingItemType { get; }

		string Value { get; }

		IList<string> Properties { get; }
	}
}
