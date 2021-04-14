using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.FrontendProxyRoutingAgent
{
	public class FrontendProxyAgentConfiguration
	{
		public RoutingHost ProxyDestinationOverride { get; private set; }

		public static FrontendProxyAgentConfiguration Load()
		{
			FrontendProxyAgentConfiguration frontendProxyAgentConfiguration = new FrontendProxyAgentConfiguration();
			string configString = TransportAppConfig.GetConfigString("FrontendProxyDestinationOverride", null);
			if (!string.IsNullOrEmpty(configString))
			{
				frontendProxyAgentConfiguration.ProxyDestinationOverride = new RoutingHost(configString);
			}
			return frontendProxyAgentConfiguration;
		}

		private const string ProxyDestinationString = "FrontendProxyDestinationOverride";
	}
}
