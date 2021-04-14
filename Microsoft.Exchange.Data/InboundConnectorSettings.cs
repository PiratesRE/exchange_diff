using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	internal enum InboundConnectorSettings
	{
		Default = 0,
		ConnectionFilteringEnabled = 1,
		AntiSpamFilteringEnabled = 2,
		AntiMalwareFilteringEnabled = 4,
		PolicyFilteringEnabled = 8,
		RejectAnonymousConnection = 16,
		RequireTls = 32,
		RejectUntrustedConnection = 64,
		CloudServicesMailEnabled = 128
	}
}
