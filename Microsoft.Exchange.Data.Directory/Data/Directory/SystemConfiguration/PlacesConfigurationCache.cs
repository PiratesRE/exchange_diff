using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PlacesConfigurationCache : LazyLookupTimeoutCache<string, ServiceEndpoint>
	{
		protected PlacesConfigurationCache() : base(1, 6, false, TimeSpan.FromDays(1.0))
		{
		}

		private string MapControlUrl
		{
			get
			{
				return PlacesConfigurationCache.GetEndpointUrl("MapControlUrl");
			}
		}

		public virtual bool IsFeatureEnabled
		{
			get
			{
				return !string.IsNullOrEmpty(this.LocationServicesUrl) && !string.IsNullOrEmpty(this.LocationServicesKey) && !string.IsNullOrEmpty(this.PhonebookServicesUrl) && !string.IsNullOrEmpty(this.PhonebookServicesKey) && !string.IsNullOrEmpty(this.StaticMapUrl) && !string.IsNullOrEmpty(this.MapControlUrl) && !string.IsNullOrEmpty(this.MapControlKey) && !string.IsNullOrEmpty(this.DirectionsPageUrl);
			}
		}

		public virtual string LocationServicesUrl
		{
			get
			{
				return PlacesConfigurationCache.GetEndpointUrl("LocationServicesUrl");
			}
		}

		public virtual string LocationServicesKey
		{
			get
			{
				return PlacesConfigurationCache.GetEndpointToken("LocationServicesUrl");
			}
		}

		public virtual string PhonebookServicesUrl
		{
			get
			{
				return PlacesConfigurationCache.GetEndpointUrl("PhonebookServicesUrl");
			}
		}

		public virtual string PhonebookServicesKey
		{
			get
			{
				return PlacesConfigurationCache.GetEndpointToken("PhonebookServicesUrl");
			}
		}

		public virtual string StaticMapUrl
		{
			get
			{
				return PlacesConfigurationCache.GetEndpointUrl("StaticMapUrl");
			}
		}

		public virtual string MapControlKey
		{
			get
			{
				return PlacesConfigurationCache.GetEndpointToken("MapControlUrl");
			}
		}

		public virtual string DirectionsPageUrl
		{
			get
			{
				return PlacesConfigurationCache.GetEndpointUrl("DirectionsPageUrl");
			}
		}

		protected override ServiceEndpoint CreateOnCacheMiss(string serviceEndpointId, ref bool shouldAdd)
		{
			PlacesConfigurationCache.Tracer.TraceDebug<string>((long)this.GetHashCode(), "PlacesConfigurationCache.CreateOnCacheMiss called for service endpoint id: {0}", serviceEndpointId);
			ServiceEndpoint result = null;
			try
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 215, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\PlacesConfigurationCache.cs");
				ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
				result = endpointContainer.GetEndpoint(serviceEndpointId);
			}
			catch (EndpointContainerNotFoundException)
			{
				PlacesConfigurationCache.Tracer.TraceDebug(0L, "PlacesConfigurationCache: Endpoint Container doesn't exist.");
			}
			catch (ServiceEndpointNotFoundException)
			{
				PlacesConfigurationCache.Tracer.TraceDebug<string>(0L, "PlacesConfigurationCache: Endpoint '{0}' doesn't exist.", serviceEndpointId);
			}
			catch (LocalizedException arg)
			{
				PlacesConfigurationCache.Tracer.TraceError<LocalizedException>(0L, "PlacesConfigurationCache: Unable to read service endpoint due to exception: {0}", arg);
			}
			return result;
		}

		private static string GetEndpointUrl(string serviceEndpointId)
		{
			ServiceEndpoint serviceEndpoint = PlacesConfigurationCache.Instance.Get(serviceEndpointId);
			if (serviceEndpoint != null && serviceEndpoint.Uri != null)
			{
				return serviceEndpoint.Uri.OriginalString;
			}
			return null;
		}

		private static string GetEndpointToken(string serviceEndpointId)
		{
			ServiceEndpoint serviceEndpoint = PlacesConfigurationCache.Instance.Get(serviceEndpointId);
			if (serviceEndpoint != null)
			{
				return serviceEndpoint.Token;
			}
			return null;
		}

		public static bool IsRestrictedCulture(string culture)
		{
			return PlacesConfigurationCache.RestrictedCultures.Contains(culture);
		}

		public static string GetMapControlUrl(string culture)
		{
			if (PlacesConfigurationCache.Instance.IsFeatureEnabled && !PlacesConfigurationCache.IsRestrictedCulture(culture))
			{
				StringBuilder stringBuilder = new StringBuilder(PlacesConfigurationCache.Instance.MapControlUrl);
				stringBuilder.AppendFormat("&mkt={0}", culture);
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		private const string MarketParameter = "&mkt={0}";

		private static readonly Trace Tracer = ExTraceGlobals.SystemConfigurationCacheTracer;

		private static readonly HashSet<string> RestrictedCultures = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"hy-AM",
			"az-Cyrl-AZ",
			"az-Latn-AZ",
			"es-AR",
			"zh-CN",
			"zh-HK",
			"zh-TW",
			"zh-MO",
			"mn-Mong-CN",
			"bo-CN",
			"ug-CN",
			"ii-CN",
			"as-IN",
			"bn-IN",
			"gu-IN",
			"hi-IN",
			"kn-IN",
			"kok-IN",
			"ml-IN",
			"mr-IN",
			"or-IN",
			"pa-IN",
			"sa-IN",
			"ta-IN",
			"te-IN",
			"he-IL",
			"ko-KR",
			"ar-MA",
			"pa-Arab-PK",
			"sd-Arab-PK",
			"ur-PK",
			"sr-Cyrl-RS",
			"sr-Latn-RS",
			"sr-Cyrl-CS",
			"sr-Latn-CS",
			"tr-TR",
			"es-VE"
		};

		public static readonly PlacesConfigurationCache Instance = new PlacesConfigurationCache();
	}
}
