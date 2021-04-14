using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum OutboundConnectorFlagTypes
	{
		RouteAllMessagesViaOnPremises = 1,
		TransportRuleScoped = 4096,
		CloudServicesMailEnabled = 8192,
		AllAcceptedDomains = 16384
	}
}
