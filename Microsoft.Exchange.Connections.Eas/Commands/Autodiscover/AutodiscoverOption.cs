using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[Flags]
	public enum AutodiscoverOption
	{
		ExistingEndpoint = 1,
		Probes = 2,
		ExistingEndpointAndProbes = 3
	}
}
