using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.ExchangeTopology
{
	internal interface ITopologySite
	{
		string Name { get; }

		ReadOnlyCollection<ITopologySiteLink> TopologySiteLinks { get; }
	}
}
