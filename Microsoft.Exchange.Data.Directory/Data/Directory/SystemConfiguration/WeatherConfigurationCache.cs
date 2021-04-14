using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class WeatherConfigurationCache : LazyLookupTimeoutCache<string, ServiceEndpoint>, IWeatherConfigurationCache
	{
		private WeatherConfigurationCache() : base(1, 6, false, TimeSpan.FromDays(1.0))
		{
		}

		public bool IsFeatureEnabled
		{
			get
			{
				return !string.IsNullOrEmpty(this.WeatherServiceUrl);
			}
		}

		public string WeatherServiceUrl
		{
			get
			{
				return WeatherConfigurationCache.GetEndpointUrl("WeatherServicesUrl");
			}
		}

		public bool IsRestrictedCulture(string culture)
		{
			return !WeatherConfigurationCache.SupportedCultures.Contains(culture);
		}

		protected override ServiceEndpoint CreateOnCacheMiss(string serviceEndpointId, ref bool shouldAdd)
		{
			WeatherConfigurationCache.Tracer.TraceDebug<string>((long)this.GetHashCode(), "WeatherConfigurationCache.CreateOnCacheMiss called for service endpoint id: {0}", serviceEndpointId);
			ServiceEndpoint endpoint = null;
			try
			{
				ADNotificationAdapter.TryRunADOperation(delegate()
				{
					ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 259, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\WeatherConfigurationCache.cs");
					ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
					endpoint = endpointContainer.GetEndpoint(serviceEndpointId);
				});
			}
			catch (EndpointContainerNotFoundException)
			{
				WeatherConfigurationCache.Tracer.TraceDebug(0L, "WeatherConfigurationCache: Endpoint Container doesn't exist.");
			}
			catch (ServiceEndpointNotFoundException)
			{
				WeatherConfigurationCache.Tracer.TraceDebug<string>(0L, "WeatherConfigurationCache: Endpoint '{0}' doesn't exist.", serviceEndpointId);
			}
			catch (LocalizedException arg)
			{
				WeatherConfigurationCache.Tracer.TraceError<LocalizedException>(0L, "WeatherConfigurationCache: Unable to read service endpoint due to exception: {0}", arg);
			}
			return endpoint;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool IsWeatherEnabledByDefault()
		{
			return VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
		}

		private static string GetEndpointUrl(string serviceEndpointId)
		{
			ServiceEndpoint serviceEndpoint = WeatherConfigurationCache.Instance.Get(serviceEndpointId);
			if (serviceEndpoint != null && serviceEndpoint.Uri != null)
			{
				return serviceEndpoint.Uri.OriginalString;
			}
			if (WeatherConfigurationCache.IsWeatherEnabledByDefault())
			{
				return "http://api.weather.msn.com/data.aspx";
			}
			return null;
		}

		private const string WeatherServiceUrlDefault = "http://api.weather.msn.com/data.aspx";

		private static readonly Trace Tracer = ExTraceGlobals.SystemConfigurationCacheTracer;

		public static readonly WeatherConfigurationCache Instance = new WeatherConfigurationCache();

		private static readonly HashSet<string> SupportedCultures = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"af-ZA",
			"am-ET",
			"ar-AE",
			"ar-BH",
			"ar-DZ",
			"ar-EG",
			"ar-IQ",
			"ar-JO",
			"ar-KW",
			"ar-LB",
			"ar-LY",
			"ar-MA",
			"ar-OM",
			"ar-QA",
			"ar-SA",
			"ar-SY",
			"ar-TN",
			"ar-XA",
			"ar-YE",
			"as-IN",
			"az-Latn-AZ",
			"bg-BG",
			"bn-BD",
			"bn-IN",
			"bs-Cyrl-BA",
			"bs-Latn-BA",
			"ca-ES",
			"cs-CZ",
			"cy-GB",
			"da-DK",
			"de-AT",
			"de-CH",
			"de-DE",
			"el-GR",
			"en-AE",
			"en-AU",
			"en-CA",
			"en-GB",
			"en-HK",
			"en-ID",
			"en-IE",
			"en-IN",
			"en-MY",
			"en-NZ",
			"en-PH",
			"en-SG",
			"en-US",
			"en-VI",
			"en-VN",
			"en-XA",
			"en-XU",
			"en-ZA",
			"es-AR",
			"es-CL",
			"es-CO",
			"es-ES",
			"es-MX",
			"es-PE",
			"es-US",
			"es-VE",
			"es-XL",
			"et-EE",
			"eu-ES",
			"fa-IR",
			"fi-FI",
			"fil-PH",
			"fr-BE",
			"fr-CA",
			"fr-CH",
			"fr-FR",
			"fr-MA",
			"ga-IE",
			"gl-ES",
			"gu-IN",
			"ha-Latn-NG",
			"he-IL",
			"hi-IN",
			"hr-HR",
			"hu-HU",
			"hy-AM",
			"id-ID",
			"ig-NG",
			"is-IS",
			"it-IT",
			"iu-Latn-CA",
			"ja-JP",
			"ka-GE",
			"kk-KZ",
			"km-KH",
			"kn-IN",
			"kok-IN",
			"ko-KR",
			"ky-KG",
			"lb-LU",
			"lo-LA",
			"lt-LT",
			"lv-LV",
			"mi-NZ",
			"mk-MK",
			"ml-IN",
			"mn-MN",
			"mr-IN",
			"ms-BN",
			"ms-MY",
			"mt-MT",
			"nb-NO",
			"ne-NP",
			"nl-BE",
			"nl-NL",
			"nn-NO",
			"nso-ZA",
			"or-IN",
			"pa-IN",
			"pl-PL",
			"ps-AF",
			"ps-PS",
			"pt-BR",
			"pt-PT",
			"quz-PE",
			"ro-RO",
			"ru-RU",
			"rw-RW",
			"si-LK",
			"sk-SK",
			"sl-SI",
			"sq-AL",
			"sr-Cyrl-RS",
			"sr-Latn-RS",
			"sv-SE",
			"sw-KE",
			"ta-IN",
			"te-IN",
			"th-TH",
			"tk-TM",
			"tn-ZA",
			"tr-TR",
			"tt-RU",
			"uk-UA",
			"ur-PK",
			"uz-Latn-UZ",
			"vi-VN",
			"wo-SN",
			"xh-ZA",
			"yo-NG",
			"zh-HK",
			"zh-SG",
			"zu-ZA",
			"az-Latn",
			"bs-Cyrl",
			"bs-Latn",
			"ha-Latn",
			"iu-Latn",
			"sr-Cyrl",
			"sr-Latn",
			"uz-Latn"
		};
	}
}
