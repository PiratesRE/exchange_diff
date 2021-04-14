using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security.ExternalAuthentication;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.Data.Storage.Authentication
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class TargetUriViaGetFederationInformation : LazyLookupTimeoutCache<string, TokenTarget>
	{
		private TargetUriViaGetFederationInformation() : base(1, 1000, false, CacheTimeToLive.FederatedCacheTimeToLive)
		{
		}

		public static TargetUriViaGetFederationInformation Singleton
		{
			get
			{
				return TargetUriViaGetFederationInformation.singleton;
			}
		}

		protected override TokenTarget CreateOnCacheMiss(string key, ref bool shouldAdd)
		{
			TargetUriViaGetFederationInformation.Tracer.TraceDebug<string>((long)this.GetHashCode(), "UriCache: cache miss for: {0}", key);
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 68, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\Authentication\\TargetUriViaGetFederationInformation.cs");
			string[] autodiscoverTrustedHosters = topologyConfigurationSession.GetAutodiscoverTrustedHosters();
			IEnumerable<GetFederationInformationResult> enumerable;
			using (AutodiscoverClient autodiscoverClient = new AutodiscoverClient())
			{
				Server localServer = LocalServerCache.LocalServer;
				if (localServer != null && localServer.InternetWebProxy != null)
				{
					TargetUriViaGetFederationInformation.Tracer.TraceDebug<Uri>((long)this.GetHashCode(), "Using custom InternetWebProxy {0}.", localServer.InternetWebProxy);
					autodiscoverClient.Proxy = new WebProxy(localServer.InternetWebProxy);
				}
				if (autodiscoverTrustedHosters != null)
				{
					TargetUriViaGetFederationInformation.Tracer.TraceDebug<ArrayTracer<string>>((long)this.GetHashCode(), "Using trusted hostnames: {0}.", new ArrayTracer<string>(autodiscoverTrustedHosters));
					autodiscoverClient.AllowedHostnames.AddRange(autodiscoverTrustedHosters);
				}
				enumerable = GetFederationInformationClient.Discover(autodiscoverClient, key);
			}
			foreach (GetFederationInformationResult getFederationInformationResult in enumerable)
			{
				if (getFederationInformationResult.Type == AutodiscoverResult.Success)
				{
					TargetUriViaGetFederationInformation.Tracer.TraceDebug<string, string, Uri>(0L, "Autodiscover's GetFederationInformation returned ApplicationUri {0} for domain {1} using {2} URL", getFederationInformationResult.ApplicationUri, key, getFederationInformationResult.Url);
					Uri uri = new Uri("http://" + getFederationInformationResult.ApplicationUri, UriKind.Absolute);
					if (getFederationInformationResult.TokenIssuerUris != null && getFederationInformationResult.TokenIssuerUris.Length > 0)
					{
						Uri[] tokenIssuerUris = Array.ConvertAll<string, Uri>(getFederationInformationResult.TokenIssuerUris, (string tokenIssuerUri) => new Uri(tokenIssuerUri, UriKind.RelativeOrAbsolute));
						return new TokenTarget(uri, tokenIssuerUris);
					}
					return new TokenTarget(uri);
				}
			}
			return null;
		}

		private static readonly Trace Tracer = ExTraceGlobals.TargetUriResolverTracer;

		private static TargetUriViaGetFederationInformation singleton = new TargetUriViaGetFederationInformation();
	}
}
