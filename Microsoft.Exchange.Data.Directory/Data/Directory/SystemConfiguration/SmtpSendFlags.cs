using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum SmtpSendFlags
	{
		UseExternalDNSServers = 2,
		DomainSecureEnabled = 4,
		RequireOorg = 8,
		TransportRuleScoped = 4096,
		FrontendProxyEnabled = 8192,
		EdgeSynced = 65536,
		CloudServicesMailEnabled = 131072
	}
}
