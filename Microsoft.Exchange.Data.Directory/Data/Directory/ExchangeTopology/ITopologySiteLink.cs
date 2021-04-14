using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.ExchangeTopology
{
	internal interface ITopologySiteLink
	{
		string Name { get; }

		ReadOnlyCollection<ITopologySite> TopologySites { get; }

		int Cost { get; }

		Unlimited<ByteQuantifiedSize> MaxMessageSize { get; }

		ulong AbsoluteMaxMessageSize { get; }
	}
}
