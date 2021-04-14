using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class TargetForestAnchorMailbox : DatabaseBasedAnchorMailbox
	{
		public TargetForestAnchorMailbox(IRequestContext requestContext, string domain, bool supportCookieBasedAffinity) : base(AnchorSource.Domain, domain, requestContext)
		{
			this.supportCookieBasedAffinity = supportCookieBasedAffinity;
		}

		public override ADObjectId GetDatabase()
		{
			if (TargetForestAnchorMailbox.RandomDatabaseInTargetForestEnabled.Value)
			{
				string domain = (string)base.SourceObject;
				ADObjectId randomDatabaseFromDomain = this.GetRandomDatabaseFromDomain(domain);
				if (randomDatabaseFromDomain != null)
				{
					base.RequestContext.Logger.AppendGenericInfo("Database", randomDatabaseFromDomain.ObjectGuid);
				}
				return randomDatabaseFromDomain;
			}
			return base.GetDatabase();
		}

		public override BackEndServer TryDirectBackEndCalculation()
		{
			if (TargetForestAnchorMailbox.RandomBackEndInSameForestEnabled.Value)
			{
				base.RequestContext.Logger.AppendString(HttpProxyMetadata.RoutingHint, "-SameForestRandomBackend");
				if (TargetForestAnchorMailbox.PreferServersCacheForRandomBackEnd.Value)
				{
					try
					{
						return HttpProxyBackEndHelper.GetAnyBackEndServer(true);
					}
					catch (ServerHasNotBeenFoundException ex)
					{
						base.RequestContext.Logger.AppendGenericError("ServersCacheErr", ex.ToString());
					}
				}
				return HttpProxyBackEndHelper.GetAnyBackEndServer(false);
			}
			return base.TryDirectBackEndCalculation();
		}

		public override BackEndCookieEntryBase BuildCookieEntryForTarget(BackEndServer routingTarget, bool proxyToDownLevel, bool useResourceForest)
		{
			if (this.supportCookieBasedAffinity)
			{
				return base.BuildCookieEntryForTarget(routingTarget, proxyToDownLevel, useResourceForest);
			}
			return null;
		}

		public override BackEndServer AcceptBackEndCookie(HttpCookie backEndCookie)
		{
			if (this.supportCookieBasedAffinity)
			{
				return base.AcceptBackEndCookie(backEndCookie);
			}
			return null;
		}

		protected override AnchorMailboxCacheEntry LoadCacheEntryFromIncomingCookie()
		{
			if (this.supportCookieBasedAffinity)
			{
				return base.LoadCacheEntryFromIncomingCookie();
			}
			return null;
		}

		private string GetResourceForestFqdnByAcceptedDomainName(string tenantAcceptedDomain)
		{
			string resourceForestFqdn;
			if (!TargetForestAnchorMailbox.domainToResourceForestMap.TryGetValue(tenantAcceptedDomain, out resourceForestFqdn))
			{
				long latency = 0L;
				resourceForestFqdn = LatencyTracker.GetLatency<string>(delegate()
				{
					resourceForestFqdn = ADAccountPartitionLocator.GetResourceForestFqdnByAcceptedDomainName(tenantAcceptedDomain);
					return resourceForestFqdn;
				}, out latency);
				TargetForestAnchorMailbox.domainToResourceForestMap.TryInsertAbsolute(tenantAcceptedDomain, resourceForestFqdn, TargetForestAnchorMailbox.DomainForestAbsoluteTimeout.Value);
				base.RequestContext.LatencyTracker.HandleGlsLatency(latency);
			}
			return resourceForestFqdn;
		}

		private ADObjectId GetRandomDatabasesFromForest(string resourceForestFqdn)
		{
			List<ADObjectId> list = null;
			bool flag = TargetForestAnchorMailbox.resourceForestToDatabaseMap.TryGetValue(resourceForestFqdn, out list);
			if (!flag || list == null || list.Count <= 0)
			{
				lock (TargetForestAnchorMailbox.forestDatabaseLock)
				{
					flag = TargetForestAnchorMailbox.resourceForestToDatabaseMap.TryGetValue(resourceForestFqdn, out list);
					if (!flag || list == null || list.Count <= 0)
					{
						list = new List<ADObjectId>();
						PartitionId partitionId = new PartitionId(resourceForestFqdn);
						ITopologyConfigurationSession resourceForestSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId), 318, "GetRandomDatabasesFromForest", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\AnchorMailbox\\TargetForestAnchorMailbox.cs");
						SortBy sortBy = new SortBy(ADObjectSchema.WhenCreatedUTC, SortOrder.Ascending);
						List<PropertyDefinition> databaseSchema = new List<PropertyDefinition>
						{
							ADObjectSchema.Id
						};
						long latency = 0L;
						ADPagedReader<ADRawEntry> latency2 = LatencyTracker.GetLatency<ADPagedReader<ADRawEntry>>(() => resourceForestSession.FindPagedADRawEntry(resourceForestSession.ConfigurationNamingContext, QueryScope.SubTree, TargetForestAnchorMailbox.DatabaseQueryFilter, sortBy, TargetForestAnchorMailbox.DatabasesToLoadPerForest.Value, databaseSchema), out latency);
						base.RequestContext.LatencyTracker.HandleResourceLatency(latency);
						if (latency2 != null)
						{
							foreach (ADRawEntry adrawEntry in latency2)
							{
								list.Add(adrawEntry.Id);
							}
						}
						if (list.Count > 0)
						{
							TargetForestAnchorMailbox.resourceForestToDatabaseMap[resourceForestFqdn] = list;
							if (list.Count < TargetForestAnchorMailbox.MinimumDatabasesForEffectiveLoadBalancing.Value)
							{
								base.RequestContext.Logger.AppendGenericError("TooFewDbsForLoadBalancing", string.Format("DbCount:{0}/Forest:{1}", list.Count, resourceForestFqdn));
							}
						}
					}
				}
			}
			if (list != null && list.Count > 0)
			{
				int index = TargetForestAnchorMailbox.seededRand.Next(0, list.Count);
				return list[index];
			}
			return null;
		}

		private ADObjectId GetRandomDatabaseFromDomain(string domain)
		{
			string resourceForestFqdnByAcceptedDomainName = this.GetResourceForestFqdnByAcceptedDomainName(domain);
			base.RequestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "TargetForest-RandomDatabase");
			return this.GetRandomDatabasesFromForest(resourceForestFqdnByAcceptedDomainName);
		}

		private const string PrivateDatabaseObjectClass = "msExchPrivateMDB";

		private const string PublicDatabaseObjectClass = "msExchPublicMDB";

		private static readonly BoolAppSettingsEntry RandomBackEndInSameForestEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("RandomBackEndInSameForestEnabled"), true, ExTraceGlobals.VerboseTracer);

		private static readonly BoolAppSettingsEntry RandomDatabaseInTargetForestEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("RandomDatabaseInTargetForestEnabled"), false, ExTraceGlobals.VerboseTracer);

		private static readonly BoolAppSettingsEntry PreferServersCacheForRandomBackEnd = new BoolAppSettingsEntry(HttpProxySettings.Prefix("PreferServersCacheForRandomBackEnd"), VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.PreferServersCacheForRandomBackEnd.Enabled, ExTraceGlobals.VerboseTracer);

		private static readonly TimeSpanAppSettingsEntry DomainForestAbsoluteTimeout = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("DomainForestAbsoluteTimeout"), TimeSpanUnit.Minutes, TimeSpan.FromMinutes(1440.0), ExTraceGlobals.VerboseTracer);

		private static readonly IntAppSettingsEntry MinimumDatabasesForEffectiveLoadBalancing = new IntAppSettingsEntry(HttpProxySettings.Prefix("MinimumDbsForEffectiveLoadBalancing"), 100, ExTraceGlobals.VerboseTracer);

		private static readonly IntAppSettingsEntry DatabasesToLoadPerForest = new IntAppSettingsEntry(HttpProxySettings.Prefix("DatabasesToLoadPerForest"), 1000, ExTraceGlobals.VerboseTracer);

		private static readonly IntAppSettingsEntry DomainToForestMapSize = new IntAppSettingsEntry(HttpProxySettings.Prefix("DomainToForestMapSize"), 100, ExTraceGlobals.VerboseTracer);

		private static readonly DateTime MaximumE14DatabaseCreationDate = new DateTime(2013, 6, 1);

		private static readonly QueryFilter DatabaseQueryFilter = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.GreaterThan, ADObjectSchema.WhenCreatedUTC, TargetForestAnchorMailbox.MaximumE14DatabaseCreationDate),
			new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "msExchPrivateMDB"),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "msExchPublicMDB")
			})
		});

		private static ExactTimeoutCache<string, string> domainToResourceForestMap = new ExactTimeoutCache<string, string>(null, null, null, TargetForestAnchorMailbox.DomainToForestMapSize.Value, false);

		private static object forestDatabaseLock = new object();

		private static ConcurrentDictionary<string, List<ADObjectId>> resourceForestToDatabaseMap = new ConcurrentDictionary<string, List<ADObjectId>>();

		private static Random seededRand = new Random(DateTime.Now.Millisecond);

		private bool supportCookieBasedAffinity;
	}
}
