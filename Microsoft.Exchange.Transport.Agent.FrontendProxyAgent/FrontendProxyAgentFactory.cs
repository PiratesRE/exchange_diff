using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.Transport.FrontendProxyRoutingAgent
{
	public class FrontendProxyAgentFactory : SmtpReceiveAgentFactory
	{
		public FrontendProxyAgentFactory()
		{
			this.agentConfiguration = FrontendProxyAgentConfiguration.Load();
			ExTraceGlobals.FrontendProxyAgentTracer.TraceInformation(0, (long)this.GetHashCode(), "The FrontendProxyAgentFactory is created and configuration loaded");
			Components.PerfCountersLoaderComponent.AddCounterToGetExchangeDiagnostics(typeof(FrontendProxyAgentPerformanceCounters), "FrontendProxyAgentPerformanceCounters");
		}

		internal FrontendProxyAgentFactory(IProxyHubSelector hubSelector) : this()
		{
			this.hubSelector = hubSelector;
		}

		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			return new FrontendProxyAgent(this.agentConfiguration, this.hubSelector ?? Components.ProxyHubSelectorComponent.ProxyHubSelector);
		}

		private FrontendProxyAgentConfiguration agentConfiguration;

		private IProxyHubSelector hubSelector;
	}
}
