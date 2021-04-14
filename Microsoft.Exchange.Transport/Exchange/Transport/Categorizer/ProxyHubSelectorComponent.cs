using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ProxyHubSelectorComponent : IProxyHubSelectorComponent, ITransportComponent
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ProxyHubSelectorTracer;
			}
		}

		public static SystemProbeTrace SystemProbeTracer
		{
			get
			{
				return ProxyHubSelectorComponent.systemProbeTracer;
			}
		}

		public IProxyHubSelector ProxyHubSelector
		{
			get
			{
				if (this.hubSelector == null)
				{
					throw new InvalidOperationException("Hub selector cannot be used before the component is loaded");
				}
				return this.hubSelector;
			}
		}

		public void Load()
		{
			if (this.router == null)
			{
				throw new InvalidOperationException("SetLoadTimeDependencies() must be called before Load()");
			}
			if (this.configuration.ProcessTransportRole == ProcessTransportRole.FrontEnd && !Datacenter.IsForefrontForOfficeDatacenter() && (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.OrganizationMailboxRouting.Enabled || Datacenter.IsPartnerHostedOnly(false)))
			{
				this.orgMailboxCache = new OrganizationMailboxDatabaseCache(this.configuration.AppConfig.PerTenantCache, this.configuration.ProcessTransportRole);
			}
			ProxyHubSelectorPerformanceCounters perfCounters = new ProxyHubSelectorPerformanceCounters(this.configuration.ProcessTransportRole);
			this.hubSelector = new ProxyHubSelector(this.router, this.orgMailboxCache, perfCounters, this.configuration.AppConfig.Routing);
		}

		public void Unload()
		{
			this.hubSelector = null;
			if (this.orgMailboxCache != null)
			{
				this.orgMailboxCache.Dispose();
				this.orgMailboxCache = null;
			}
			this.router = null;
			this.configuration = null;
		}

		public void SetLoadTimeDependencies(IMailRouter router, ITransportConfiguration configuration)
		{
			if (router == null)
			{
				throw new ArgumentNullException("router");
			}
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			this.router = router;
			this.configuration = configuration;
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		private static readonly SystemProbeTrace systemProbeTracer = new SystemProbeTrace(ExTraceGlobals.ProxyHubSelectorTracer, "ProxyHubSelector");

		private ProxyHubSelector hubSelector;

		private OrganizationMailboxDatabaseCache orgMailboxCache;

		private IMailRouter router;

		private ITransportConfiguration configuration;
	}
}
