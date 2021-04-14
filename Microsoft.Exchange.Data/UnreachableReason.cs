using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	internal enum UnreachableReason
	{
		None = 0,
		NoMdb = 1,
		NoRouteToMdb = 2,
		NoRouteToMta = 4,
		NonBHExpansionServer = 8,
		NoMatchingConnector = 16,
		IncompatibleDeliveryDomain = 32
	}
}
