using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.Transport.FrontendProxyRoutingAgent
{
	internal class FrontendProxyAgent : SmtpReceiveAgent
	{
		public FrontendProxyAgent(FrontendProxyAgentConfiguration configuration, IProxyHubSelector hubSelector)
		{
			this.configuration = configuration;
			this.hubSelector = hubSelector;
			base.OnProxyInboundMessage += this.OnProxyInboundMessageEventHandler;
		}

		private void OnProxyInboundMessageEventHandler(ProxyInboundMessageEventSource source, ProxyInboundMessageEventArgs e)
		{
			IEnumerable<INextHopServer> destinationServers = null;
			if (this.configuration.ProxyDestinationOverride != null)
			{
				FrontendProxyAgent.systemProbeTracer.TracePass(e.MailItem, (long)this.GetHashCode(), "The proxy destination override {0} was found in configuration and will be used as a proxy target", this.configuration.ProxyDestinationOverride);
				destinationServers = new RoutingHost[]
				{
					this.configuration.ProxyDestinationOverride
				};
			}
			else if (e.ClientIsPreE15InternalServer && e.LocalFrontendIsColocatedWithHub)
			{
				destinationServers = new FrontendProxyAgent.ColocatedNextHopServer[]
				{
					new FrontendProxyAgent.ColocatedNextHopServer(e.LocalServerFqdn)
				};
			}
			else
			{
				ITransportMailItemWrapperFacade transportMailItemWrapperFacade = (ITransportMailItemWrapperFacade)e.MailItem;
				TransportMailItem mailItem = (TransportMailItem)transportMailItemWrapperFacade.TransportMailItem;
				if (!this.hubSelector.TrySelectHubServers(mailItem, out destinationServers))
				{
					FrontendProxyAgent.systemProbeTracer.TraceFail(e.MailItem, (long)this.GetHashCode(), "TrySelectHubServers() failed");
					FrontendProxyAgentPerformanceCounters.MessagesFailedToRoute.Increment();
					return;
				}
				FrontendProxyAgent.systemProbeTracer.TracePass(e.MailItem, (long)this.GetHashCode(), "TrySelectHubServers() succeeded");
			}
			source.SetProxyRoutingOverride(destinationServers, true);
			FrontendProxyAgentPerformanceCounters.MessagesSuccessfullyRouted.Increment();
		}

		private static readonly SystemProbeTrace systemProbeTracer = new SystemProbeTrace(ExTraceGlobals.FrontendProxyAgentTracer, "FrontendProxyRoutingAgent");

		private FrontendProxyAgentConfiguration configuration;

		private IProxyHubSelector hubSelector;

		private class ColocatedNextHopServer : INextHopServer
		{
			public ColocatedNextHopServer(string fqdn)
			{
				this.Fqdn = fqdn;
			}

			public bool IsIPAddress
			{
				get
				{
					return false;
				}
			}

			public IPAddress Address
			{
				get
				{
					throw new InvalidOperationException("INextHopServer.IPAddress must not be requested since IsIPAddress is false");
				}
			}

			public string Fqdn { get; private set; }

			public bool IsFrontendAndHubColocatedServer
			{
				get
				{
					return true;
				}
			}
		}
	}
}
