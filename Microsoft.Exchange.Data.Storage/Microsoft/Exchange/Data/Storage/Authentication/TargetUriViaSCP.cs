using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security.ExternalAuthentication;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Data.Storage.Authentication
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class TargetUriViaSCP : LazyLookupTimeoutCache<string, TokenTarget>
	{
		private TargetUriViaSCP() : base(1, 1000, false, CacheTimeToLive.FederatedCacheTimeToLive)
		{
		}

		public static TargetUriViaSCP Singleton
		{
			get
			{
				return TargetUriViaSCP.singleton;
			}
		}

		protected override TokenTarget CreateOnCacheMiss(string key, ref bool shouldAdd)
		{
			TargetUriViaSCP.Tracer.TraceDebug<string>((long)this.GetHashCode(), "TargetUriViaSCP: cache miss for: {0}", key);
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				ExchangeScpObjects.DomainToApplicationUriKeyword.Filter,
				new TextFilter(ADServiceConnectionPointSchema.Keywords, "Domain=" + key, MatchOptions.FullString, MatchFlags.IgnoreCase)
			});
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 76, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\Authentication\\TargetUriViaSCP.cs");
			ADServiceConnectionPoint[] array = topologyConfigurationSession.Find<ADServiceConnectionPoint>(topologyConfigurationSession.GetAutoDiscoverGlobalContainerId(), QueryScope.SubTree, filter, null, 0);
			if (array == null || array.Length == 0)
			{
				TargetUriViaSCP.Tracer.TraceError<string>((long)this.GetHashCode(), "TargetUriViaSCP: found no SCP object for domain: {0}", key);
				return null;
			}
			if (array.Length > 1)
			{
				TargetUriViaSCP.Tracer.TraceError<string>((long)this.GetHashCode(), "TargetUriViaSCP: found more than one SCP object for: {0}", key);
				return null;
			}
			ADServiceConnectionPoint adserviceConnectionPoint = array[0];
			if (adserviceConnectionPoint.ServiceBindingInformation == null || adserviceConnectionPoint.ServiceBindingInformation.Count == 0)
			{
				TargetUriViaSCP.Tracer.TraceError<ADObjectId>((long)this.GetHashCode(), "TargetUriViaSCP: found no value in ServiceBindingInformation in: {0}", adserviceConnectionPoint.Id);
				return null;
			}
			if (adserviceConnectionPoint.ServiceBindingInformation.Count > 1)
			{
				TargetUriViaSCP.Tracer.TraceError<ADObjectId>((long)this.GetHashCode(), "TargetUriViaSCP: found more than one value in ServiceBindingInformation in: {0}", adserviceConnectionPoint.Id);
				return null;
			}
			string text = adserviceConnectionPoint.ServiceBindingInformation[0];
			if (!Uri.IsWellFormedUriString(text, UriKind.Absolute))
			{
				TargetUriViaSCP.Tracer.TraceError<string>((long)this.GetHashCode(), "TargetUriViaSCP: value in ServiceBindingInformation is not well-formed URI: {0}", text);
				return null;
			}
			TargetUriViaSCP.Tracer.TraceError<string, string>((long)this.GetHashCode(), "TargetUriViaSCP: ApplicationUri for domain {0} is {1}", key, text);
			return new TokenTarget(new Uri(text, UriKind.Absolute));
		}

		private static readonly Trace Tracer = ExTraceGlobals.TargetUriResolverTracer;

		private static TargetUriViaSCP singleton = new TargetUriViaSCP();
	}
}
