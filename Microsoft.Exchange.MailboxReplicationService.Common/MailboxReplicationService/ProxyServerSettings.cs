using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal struct ProxyServerSettings
	{
		public ProxyServerSettings(string fqdn, LocalMailboxFlags extraFlags, ProxyScenarios proxyScenario)
		{
			this = default(ProxyServerSettings);
			this.Fqdn = fqdn;
			this.ExtraFlags = extraFlags;
			this.Scenario = proxyScenario;
		}

		public string Fqdn { get; private set; }

		public LocalMailboxFlags ExtraFlags { get; private set; }

		public ProxyScenarios Scenario { get; private set; }

		public bool IsProxyLocal
		{
			get
			{
				return this.Scenario == ProxyScenarios.LocalMdbAndProxy || this.Scenario == ProxyScenarios.LocalProxyRemoteMdb;
			}
		}
	}
}
