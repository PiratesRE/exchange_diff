using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	[Flags]
	public enum FlightedFeatureScope
	{
		Client = 1,
		Server = 2,
		ClientServer = 4,
		Any = 7
	}
}
