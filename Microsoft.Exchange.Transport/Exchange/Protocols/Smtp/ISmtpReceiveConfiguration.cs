using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpReceiveConfiguration
	{
		IDiagnosticsConfigProvider DiagnosticsConfiguration { get; }

		IRoutingConfigProvider RoutingConfiguration { get; }

		ITransportConfigProvider TransportConfiguration { get; }
	}
}
