using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class OrganizationConfigCache : LazyLookupTimeoutCacheWithDiagnostics<OrganizationId, OrganizationConfigCache.Item>
	{
		public OrganizationConfigCache(bool multiTenancyEnabled) : base(2, 100, false, multiTenancyEnabled ? TimeSpan.FromMinutes(10.0) : TimeSpan.FromHours(5.0), multiTenancyEnabled ? TimeSpan.FromMinutes(30.0) : TimeSpan.FromHours(5.0))
		{
			if (multiTenancyEnabled)
			{
				this.domainToOrganizationIdMap = new SynchronizedDictionary<string, OrganizationId>(400, StringComparer.OrdinalIgnoreCase);
			}
		}

		protected override OrganizationConfigCache.Item Create(OrganizationId key, ref bool shouldAdd)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug<OrganizationId>(this.GetHashCode(), "OrganizationConfigCache miss, searching for {0}", key);
			shouldAdd = true;
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), key, null, false), 143, "Create", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\MessageTracking\\Caching\\OrganizationConfigCache.cs");
			OrganizationConfigCache.Item orgSettings = OrganizationConfigCache.GetOrgSettings(tenantOrTopologyConfigurationSession, ref shouldAdd);
			if (this.domainToOrganizationIdMap == null)
			{
				shouldAdd = true;
			}
			TraceWrapper.SearchLibraryTracer.TraceDebug<string, bool>(0, "Default Domain: {0}, Read Tracking Enabled Setting: {1}", orgSettings.DefaultDomain, orgSettings.ReadTrackingEnabled);
			if (this.domainToOrganizationIdMap != null)
			{
				foreach (string key2 in orgSettings.AuthoritativeDomains)
				{
					this.domainToOrganizationIdMap[key2] = key;
				}
			}
			return orgSettings;
		}

		protected override void HandleRemove(OrganizationId key, OrganizationConfigCache.Item value, RemoveReason reason)
		{
			base.HandleRemove(key, value, reason);
			if (this.domainToOrganizationIdMap != null)
			{
				this.domainToOrganizationIdMap.RemoveAll((OrganizationId organizationId) => key == organizationId);
			}
		}

		private static OrganizationConfigCache.Item GetOrgSettings(IConfigurationSession tenantConfigSession, ref bool shouldAdd)
		{
			Organization orgContainer = tenantConfigSession.GetOrgContainer();
			if (orgContainer == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError(0, "ADSystemConfigurationSession.GetOrgContainer returned null", new object[0]);
				TrackingFatalException.RaiseED(ErrorCode.InvalidADData, "msExchOrganizationContainer object not found", new object[0]);
			}
			bool readTrackingEnabled = orgContainer.ReadTrackingEnabled;
			QueryFilter filter = new NotFilter(new BitMaskAndFilter(AcceptedDomainSchema.AcceptedDomainFlags, 1UL));
			ADPagedReader<AcceptedDomain> adpagedReader = tenantConfigSession.FindPaged<AcceptedDomain>(tenantConfigSession.GetOrgContainerId(), QueryScope.SubTree, filter, null, 0);
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			HashSet<string> hashSet2 = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			string text = null;
			int num = 0;
			int num2 = 0;
			foreach (AcceptedDomain acceptedDomain in adpagedReader)
			{
				num2++;
				if (acceptedDomain.Default)
				{
					text = acceptedDomain.DomainName.Domain;
				}
				if (acceptedDomain.DomainType == AcceptedDomainType.Authoritative)
				{
					if (++num > 200)
					{
						shouldAdd = false;
					}
					hashSet.Add(acceptedDomain.DomainName.Domain);
				}
				else if (acceptedDomain.DomainType == AcceptedDomainType.InternalRelay)
				{
					if (++num > 200)
					{
						shouldAdd = false;
					}
					hashSet2.Add(acceptedDomain.DomainName.Domain);
				}
			}
			if (num2 == 0)
			{
				TraceWrapper.SearchLibraryTracer.TraceError(0, "No AcceptedDomain objects returned", new object[0]);
				TrackingFatalException.RaiseED(ErrorCode.InvalidADData, "msExchAcceptedDomain object not found in Organization {0}", new object[]
				{
					orgContainer
				});
			}
			if (string.IsNullOrEmpty(text))
			{
				TraceWrapper.SearchLibraryTracer.TraceError(0, "Null/Empty AcceptedDomainFqdn returned", new object[0]);
				TrackingFatalException.RaiseED(ErrorCode.InvalidADData, "DefaultDomain not found in Organization {0}", new object[]
				{
					orgContainer
				});
			}
			return new OrganizationConfigCache.Item(text, readTrackingEnabled, hashSet, hashSet2);
		}

		public bool TryGetOrganizationId(string domain, out OrganizationId organizationId)
		{
			if (this.domainToOrganizationIdMap != null)
			{
				return this.domainToOrganizationIdMap.TryGetValue(domain, out organizationId);
			}
			organizationId = OrganizationId.ForestWideOrgId;
			return true;
		}

		private const int BucketCount = 2;

		private const int OrganizationsPerBucket = 100;

		private const int EstimatedDomainsPerOrganization = 2;

		private const int MaxDomainsPerOrganization = 200;

		private SynchronizedDictionary<string, OrganizationId> domainToOrganizationIdMap;

		internal sealed class Item
		{
			public Item(string defaultDomain, bool readTrackingEnabled, HashSet<string> authoritativeDomains, HashSet<string> internalRelayDomains)
			{
				this.DefaultDomain = defaultDomain;
				this.ReadTrackingEnabled = readTrackingEnabled;
				this.AuthoritativeDomains = authoritativeDomains;
				this.InternalRelayDomains = internalRelayDomains;
			}

			public bool ReadTrackingEnabled { get; private set; }

			public string DefaultDomain { get; private set; }

			public HashSet<string> AuthoritativeDomains { get; private set; }

			public HashSet<string> InternalRelayDomains { get; private set; }
		}
	}
}
