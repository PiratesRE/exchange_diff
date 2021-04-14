using System;
using Microsoft.Exchange.AutoDiscover;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationAutodiscoverComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationAutodiscoverComponent() : base("Autodiscover")
		{
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "AnonymousAuthentication", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "EnableMobileSyncRedirectBypass", typeof(IFeature), true));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "ParseBinarySecretHeader", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "SkipServiceTopologyDiscovery", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "StreamInsightUploader", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "LoadNegoExSspNames", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "NoADLookupForUser", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "NoCrossForestDiscover", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "EcpInternalExternalUrl", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "MapiHttpForOutlook14", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "OWAUrl", typeof(IOWAUrl), true));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "AccountInCloud", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "ConfigurePerformanceCounters", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "RedirectOutlookClient", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "WsSecurityEndpoint", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "UseMapiHttpADSetting", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "NoAuthenticationTokenToNego", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "RestrictedSettings", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "MapiHttp", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Autodiscover.settings.ini", "LogonViaStandardTokens", typeof(IFeature), false));
		}

		public VariantConfigurationSection AnonymousAuthentication
		{
			get
			{
				return base["AnonymousAuthentication"];
			}
		}

		public VariantConfigurationSection EnableMobileSyncRedirectBypass
		{
			get
			{
				return base["EnableMobileSyncRedirectBypass"];
			}
		}

		public VariantConfigurationSection ParseBinarySecretHeader
		{
			get
			{
				return base["ParseBinarySecretHeader"];
			}
		}

		public VariantConfigurationSection SkipServiceTopologyDiscovery
		{
			get
			{
				return base["SkipServiceTopologyDiscovery"];
			}
		}

		public VariantConfigurationSection StreamInsightUploader
		{
			get
			{
				return base["StreamInsightUploader"];
			}
		}

		public VariantConfigurationSection LoadNegoExSspNames
		{
			get
			{
				return base["LoadNegoExSspNames"];
			}
		}

		public VariantConfigurationSection NoADLookupForUser
		{
			get
			{
				return base["NoADLookupForUser"];
			}
		}

		public VariantConfigurationSection NoCrossForestDiscover
		{
			get
			{
				return base["NoCrossForestDiscover"];
			}
		}

		public VariantConfigurationSection EcpInternalExternalUrl
		{
			get
			{
				return base["EcpInternalExternalUrl"];
			}
		}

		public VariantConfigurationSection MapiHttpForOutlook14
		{
			get
			{
				return base["MapiHttpForOutlook14"];
			}
		}

		public VariantConfigurationSection OWAUrl
		{
			get
			{
				return base["OWAUrl"];
			}
		}

		public VariantConfigurationSection AccountInCloud
		{
			get
			{
				return base["AccountInCloud"];
			}
		}

		public VariantConfigurationSection ConfigurePerformanceCounters
		{
			get
			{
				return base["ConfigurePerformanceCounters"];
			}
		}

		public VariantConfigurationSection RedirectOutlookClient
		{
			get
			{
				return base["RedirectOutlookClient"];
			}
		}

		public VariantConfigurationSection WsSecurityEndpoint
		{
			get
			{
				return base["WsSecurityEndpoint"];
			}
		}

		public VariantConfigurationSection UseMapiHttpADSetting
		{
			get
			{
				return base["UseMapiHttpADSetting"];
			}
		}

		public VariantConfigurationSection NoAuthenticationTokenToNego
		{
			get
			{
				return base["NoAuthenticationTokenToNego"];
			}
		}

		public VariantConfigurationSection RestrictedSettings
		{
			get
			{
				return base["RestrictedSettings"];
			}
		}

		public VariantConfigurationSection MapiHttp
		{
			get
			{
				return base["MapiHttp"];
			}
		}

		public VariantConfigurationSection LogonViaStandardTokens
		{
			get
			{
				return base["LogonViaStandardTokens"];
			}
		}
	}
}
