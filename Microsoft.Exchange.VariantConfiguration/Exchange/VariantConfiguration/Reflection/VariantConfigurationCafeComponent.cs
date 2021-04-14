using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationCafeComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationCafeComponent() : base("Cafe")
		{
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "CheckServerOnlineForActiveServer", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "ExplicitDomain", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "UseExternalPopIMapSettings", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "NoServiceTopologyTryGetServerVersion", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "NoFormBasedAuthentication", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "RUMUseADCache", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "PartitionedRouting", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "DownLevelServerPing", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "UseResourceForest", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "TrustClientXForwardedFor", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "MailboxServerSharedCache", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "LoadBalancedPartnerRouting", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "CompositeIdentity", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "CafeV2", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "RetryOnError", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "PreferServersCacheForRandomBackEnd", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "AnchorMailboxSharedCache", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "CafeV1RUM", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "DebugResponseHeaders", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "SyndicatedAdmin", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "EnableTls11", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "ConfigurePerformanceCounters", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "EnableTls12", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "ServersCache", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "NoCrossForestServerLocate", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "SiteNameFromServerFqdnTranslation", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "CacheLocalSiteLiveE15Servers", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "EnforceConcurrencyGuards", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "NoVDirLocationHint", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "NoCrossSiteRedirect", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "CheckServerLocatorServersForMaintenanceMode", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "UseExchClientVerInRPS", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Cafe.settings.ini", "RUMLegacyRoutingEntry", typeof(IFeature), false));
		}

		public VariantConfigurationSection CheckServerOnlineForActiveServer
		{
			get
			{
				return base["CheckServerOnlineForActiveServer"];
			}
		}

		public VariantConfigurationSection ExplicitDomain
		{
			get
			{
				return base["ExplicitDomain"];
			}
		}

		public VariantConfigurationSection UseExternalPopIMapSettings
		{
			get
			{
				return base["UseExternalPopIMapSettings"];
			}
		}

		public VariantConfigurationSection NoServiceTopologyTryGetServerVersion
		{
			get
			{
				return base["NoServiceTopologyTryGetServerVersion"];
			}
		}

		public VariantConfigurationSection NoFormBasedAuthentication
		{
			get
			{
				return base["NoFormBasedAuthentication"];
			}
		}

		public VariantConfigurationSection RUMUseADCache
		{
			get
			{
				return base["RUMUseADCache"];
			}
		}

		public VariantConfigurationSection PartitionedRouting
		{
			get
			{
				return base["PartitionedRouting"];
			}
		}

		public VariantConfigurationSection DownLevelServerPing
		{
			get
			{
				return base["DownLevelServerPing"];
			}
		}

		public VariantConfigurationSection UseResourceForest
		{
			get
			{
				return base["UseResourceForest"];
			}
		}

		public VariantConfigurationSection TrustClientXForwardedFor
		{
			get
			{
				return base["TrustClientXForwardedFor"];
			}
		}

		public VariantConfigurationSection MailboxServerSharedCache
		{
			get
			{
				return base["MailboxServerSharedCache"];
			}
		}

		public VariantConfigurationSection LoadBalancedPartnerRouting
		{
			get
			{
				return base["LoadBalancedPartnerRouting"];
			}
		}

		public VariantConfigurationSection CompositeIdentity
		{
			get
			{
				return base["CompositeIdentity"];
			}
		}

		public VariantConfigurationSection CafeV2
		{
			get
			{
				return base["CafeV2"];
			}
		}

		public VariantConfigurationSection RetryOnError
		{
			get
			{
				return base["RetryOnError"];
			}
		}

		public VariantConfigurationSection PreferServersCacheForRandomBackEnd
		{
			get
			{
				return base["PreferServersCacheForRandomBackEnd"];
			}
		}

		public VariantConfigurationSection AnchorMailboxSharedCache
		{
			get
			{
				return base["AnchorMailboxSharedCache"];
			}
		}

		public VariantConfigurationSection CafeV1RUM
		{
			get
			{
				return base["CafeV1RUM"];
			}
		}

		public VariantConfigurationSection DebugResponseHeaders
		{
			get
			{
				return base["DebugResponseHeaders"];
			}
		}

		public VariantConfigurationSection SyndicatedAdmin
		{
			get
			{
				return base["SyndicatedAdmin"];
			}
		}

		public VariantConfigurationSection EnableTls11
		{
			get
			{
				return base["EnableTls11"];
			}
		}

		public VariantConfigurationSection ConfigurePerformanceCounters
		{
			get
			{
				return base["ConfigurePerformanceCounters"];
			}
		}

		public VariantConfigurationSection EnableTls12
		{
			get
			{
				return base["EnableTls12"];
			}
		}

		public VariantConfigurationSection ServersCache
		{
			get
			{
				return base["ServersCache"];
			}
		}

		public VariantConfigurationSection NoCrossForestServerLocate
		{
			get
			{
				return base["NoCrossForestServerLocate"];
			}
		}

		public VariantConfigurationSection SiteNameFromServerFqdnTranslation
		{
			get
			{
				return base["SiteNameFromServerFqdnTranslation"];
			}
		}

		public VariantConfigurationSection CacheLocalSiteLiveE15Servers
		{
			get
			{
				return base["CacheLocalSiteLiveE15Servers"];
			}
		}

		public VariantConfigurationSection EnforceConcurrencyGuards
		{
			get
			{
				return base["EnforceConcurrencyGuards"];
			}
		}

		public VariantConfigurationSection NoVDirLocationHint
		{
			get
			{
				return base["NoVDirLocationHint"];
			}
		}

		public VariantConfigurationSection NoCrossSiteRedirect
		{
			get
			{
				return base["NoCrossSiteRedirect"];
			}
		}

		public VariantConfigurationSection CheckServerLocatorServersForMaintenanceMode
		{
			get
			{
				return base["CheckServerLocatorServersForMaintenanceMode"];
			}
		}

		public VariantConfigurationSection UseExchClientVerInRPS
		{
			get
			{
				return base["UseExchClientVerInRPS"];
			}
		}

		public VariantConfigurationSection RUMLegacyRoutingEntry
		{
			get
			{
				return base["RUMLegacyRoutingEntry"];
			}
		}
	}
}
