using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal enum Proximity : short
	{
		LocalServer,
		LocalADSite,
		RemoteADSite,
		RemoteRoutingGroup,
		None = 32767
	}
}
