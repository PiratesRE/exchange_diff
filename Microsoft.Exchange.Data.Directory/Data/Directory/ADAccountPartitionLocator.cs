using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class ADAccountPartitionLocator
	{
		static ADAccountPartitionLocator()
		{
			ADAccountPartitionLocator.lastRefreshTick = -1;
			ADAccountPartitionLocator.ResourcePartitionGuid = ADObjectId.ResourcePartitionGuid;
			ADAccountPartitionLocator.modifySyncObject = new object();
			ADAccountPartitionLocator.externalOrgIdPartitionCache = new Cache<Guid, TenantPartitionCacheItem>(ADAccountPartitionLocator.TenantCacheSizeInBytes, ADAccountPartitionLocator.TenantCacheExpiry, TimeSpan.Zero);
			ADAccountPartitionLocator.acceptedDomainPartitionCache = new Cache<string, TenantPartitionCacheItem>(ADAccountPartitionLocator.TenantCacheSizeInBytes, ADAccountPartitionLocator.TenantCacheExpiry, TimeSpan.Zero);
			ADAccountPartitionLocator.tenantNamePartitionCache = new Cache<string, TenantPartitionCacheItem>(ADAccountPartitionLocator.TenantCacheSizeInBytes, ADAccountPartitionLocator.TenantCacheExpiry, TimeSpan.Zero);
			ADAccountPartitionLocator.msaUserNetIdPartitionCache = new Cache<string, TenantPartitionCacheItem>(ADAccountPartitionLocator.TenantCacheSizeInBytes, ADAccountPartitionLocator.TenantCacheExpiry, TimeSpan.Zero);
		}

		public static PartitionId[] GetAllAccountPartitionIdsEnabledForProvisioning()
		{
			ADAccountPartitionLocator.LoadPartitionCacheIfNecessary();
			return ADAccountPartitionLocator.allAccountPartitionIdsEnabledForProvisioning;
		}

		public static PartitionId SelectAccountPartitionForNewTenant(string name)
		{
			PartitionId[] array = ADAccountPartitionLocator.GetAllAccountPartitionIdsEnabledForProvisioning();
			if (array == null || array.Length == 0)
			{
				return null;
			}
			if (array.Length == 1)
			{
				return array[0];
			}
			int num = Math.Abs(name.ToUpperInvariant().GetHashCode());
			int num2 = num % array.Length;
			return array[num2];
		}

		public static bool IsKnownPartition(PartitionId partitionId)
		{
			if (TopologyProvider.IsAdamTopology())
			{
				return true;
			}
			if (!Globals.IsMicrosoftHostedOnly && partitionId.PartitionObjectId == null)
			{
				return false;
			}
			ADAccountPartitionLocator.LoadPartitionCacheIfNecessary();
			return partitionId.IsLocalForestPartition() || ADAccountPartitionLocator.partitionsFQDNToGuid.ContainsKey(partitionId.ForestFQDN);
		}

		public static PartitionId GetPartitionIdByAcceptedDomainName(string acceptedDomainName)
		{
			string text;
			Guid guid;
			return ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(acceptedDomainName, out text, out guid);
		}

		public static PartitionId GetPartitionIdByAcceptedDomainName(string acceptedDomainName, out string tenantContainerCN, out Guid externalDirectoryOrganizationId)
		{
			tenantContainerCN = null;
			externalDirectoryOrganizationId = Guid.Empty;
			if (!Globals.IsMicrosoftHostedOnly)
			{
				return PartitionId.LocalForest;
			}
			TenantPartitionCacheItem tenantPartitionCacheItem = ADAccountPartitionLocator.FindTenantInfoByAcceptedDomain(acceptedDomainName);
			if (tenantPartitionCacheItem != null)
			{
				ADAccountPartitionLocator.EnsureAllowedCallerForUnregisteredAccountPartition(tenantPartitionCacheItem);
				tenantContainerCN = tenantPartitionCacheItem.TenantName;
				externalDirectoryOrganizationId = tenantPartitionCacheItem.ExternalOrgId;
				return tenantPartitionCacheItem.AccountPartitionId;
			}
			throw new CannotResolveTenantNameException(DirectoryStrings.CannotResolveTenantNameByAcceptedDomain(acceptedDomainName));
		}

		public static string GetResourceForestFqdnByAcceptedDomainName(string acceptedDomainName)
		{
			if (!Globals.IsMicrosoftHostedOnly)
			{
				return PartitionId.LocalForest.ForestFQDN;
			}
			TenantPartitionCacheItem tenantPartitionCacheItem = ADAccountPartitionLocator.FindTenantInfoByAcceptedDomain(acceptedDomainName);
			if (tenantPartitionCacheItem != null)
			{
				ADAccountPartitionLocator.EnsureAllowedCallerForUnregisteredAccountPartition(tenantPartitionCacheItem);
				return tenantPartitionCacheItem.ResourceForestFqdn;
			}
			throw new CannotResolveTenantNameException(DirectoryStrings.CannotResolveTenantNameByAcceptedDomain(acceptedDomainName));
		}

		public static string GetResourceForestFqdnByExternalDirectoryOrganizationId(Guid externalDirectoryOrganizationId)
		{
			if (!Globals.IsMicrosoftHostedOnly)
			{
				return PartitionId.LocalForest.ForestFQDN;
			}
			TenantPartitionCacheItem tenantPartitionCacheItem = ADAccountPartitionLocator.FindTenantInfoByExternalOrgId(externalDirectoryOrganizationId);
			if (tenantPartitionCacheItem != null)
			{
				ADAccountPartitionLocator.EnsureAllowedCallerForUnregisteredAccountPartition(tenantPartitionCacheItem);
				return tenantPartitionCacheItem.ResourceForestFqdn;
			}
			throw new CannotResolveExternalDirectoryOrganizationIdException(DirectoryStrings.CannotResolveTenantNameByExternalDirectoryId(externalDirectoryOrganizationId.ToString()));
		}

		public static PartitionId GetPartitionIdByExternalDirectoryOrganizationId(Guid externalDirectoryOrganizationId)
		{
			string text;
			return ADAccountPartitionLocator.GetPartitionIdByExternalDirectoryOrganizationId(externalDirectoryOrganizationId, out text);
		}

		public static PartitionId GetPartitionIdByExternalDirectoryOrganizationId(Guid externalDirectoryOrganizationId, out string tenantContainerCN)
		{
			tenantContainerCN = null;
			TenantPartitionCacheItem tenantPartitionCacheItem = ADAccountPartitionLocator.FindTenantInfoByExternalOrgId(externalDirectoryOrganizationId);
			if (tenantPartitionCacheItem != null)
			{
				ADAccountPartitionLocator.EnsureAllowedCallerForUnregisteredAccountPartition(tenantPartitionCacheItem);
				tenantContainerCN = tenantPartitionCacheItem.TenantName;
				return tenantPartitionCacheItem.AccountPartitionId;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_CannotResolveExternalDirectoryOrganizationId, externalDirectoryOrganizationId.ToString(), new object[]
			{
				DirectoryStrings.CannotResolveTenantNameByExternalDirectoryId(externalDirectoryOrganizationId.ToString()),
				Environment.StackTrace
			});
			throw new CannotResolveExternalDirectoryOrganizationIdException(DirectoryStrings.CannotResolveTenantNameByExternalDirectoryId(externalDirectoryOrganizationId.ToString()));
		}

		public static PartitionId GetPartitionIdByMSAUserNetID(string msaUserNetID, out string tenantContainerCN, out Guid externalDirectoryOrganizationId)
		{
			tenantContainerCN = null;
			externalDirectoryOrganizationId = Guid.Empty;
			TenantPartitionCacheItem tenantPartitionCacheItem = ADAccountPartitionLocator.FindTenantInfoByMSAUserNetID(msaUserNetID);
			if (tenantPartitionCacheItem != null)
			{
				ADAccountPartitionLocator.EnsureAllowedCallerForUnregisteredAccountPartition(tenantPartitionCacheItem);
				tenantContainerCN = tenantPartitionCacheItem.TenantName;
				externalDirectoryOrganizationId = tenantPartitionCacheItem.ExternalOrgId;
				return tenantPartitionCacheItem.AccountPartitionId;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_CannotResolveMSAUserNetID, msaUserNetID, new object[]
			{
				DirectoryStrings.CannotFindTenantByMSAUserNetID(msaUserNetID),
				Environment.StackTrace
			});
			throw new CannotResolveMSAUserNetIDException(DirectoryStrings.CannotFindTenantByMSAUserNetID(msaUserNetID));
		}

		public static Guid GetExternalDirectoryOrganizationIdByTenantName(string tenantName, PartitionId partitionId)
		{
			if (string.IsNullOrEmpty(tenantName))
			{
				throw new ArgumentNullException("tenantName");
			}
			if (partitionId == null)
			{
				throw new ArgumentNullException("partitionId");
			}
			ADAccountPartitionLocator.LoadPartitionCacheIfNecessary();
			TenantPartitionCacheItem tenantPartitionCacheItem;
			if (!ADAccountPartitionLocator.tenantNamePartitionCache.TryGetValue(tenantName, out tenantPartitionCacheItem))
			{
				tenantPartitionCacheItem = ADAccountPartitionLocator.SearchForTenantInfoByOrganizationName(tenantName, partitionId);
				if (tenantPartitionCacheItem != null)
				{
					ADAccountPartitionLocator.InsertCacheMaps(tenantPartitionCacheItem, null, null);
				}
			}
			if (tenantPartitionCacheItem != null)
			{
				ADAccountPartitionLocator.EnsureRegisteredAccountPartition(tenantPartitionCacheItem);
				return tenantPartitionCacheItem.ExternalOrgId;
			}
			throw new CannotResolveTenantNameException(DirectoryStrings.CannotResolveTenantNameByAcceptedDomain(tenantName));
		}

		public static bool ValidateDomainName(AcceptedDomain domain, Exception duplicateAcceptedDomainException, Exception conflictingAcceptedDomainException, out Exception error)
		{
			IConfigurationSession session;
			if (Globals.IsDatacenter)
			{
				try
				{
					session = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromTenantAcceptedDomain(domain.DomainName.Domain), 432, "ValidateDomainName", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADAccountPartitionLocator.cs");
					goto IL_5C;
				}
				catch (CannotResolveTenantNameException)
				{
					error = null;
					return true;
				}
			}
			session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 442, "ValidateDomainName", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADAccountPartitionLocator.cs");
			IL_5C:
			return ADAccountPartitionLocator.ValidateDomainName(domain, session, duplicateAcceptedDomainException, conflictingAcceptedDomainException, out error);
		}

		public static void AddTenantDataToCache(Guid externalDirectoryOrganizationId, string resourceForestFqdn, string accountForestFqdn, string domainName, string tenantContainerCN)
		{
			Guid accountPartitionGuid;
			ADAccountPartitionLocator.partitionsFQDNToGuid.TryGetValue(accountForestFqdn, out accountPartitionGuid);
			TenantPartitionCacheItem tenantPartitionCacheItem = new TenantPartitionCacheItem(accountPartitionGuid, accountForestFqdn, resourceForestFqdn, externalDirectoryOrganizationId, tenantContainerCN, false);
			ADAccountPartitionLocator.EnsureRegisteredAccountPartition(tenantPartitionCacheItem);
			ADAccountPartitionLocator.InsertCacheMaps(tenantPartitionCacheItem, domainName, null);
		}

		internal static PartitionId[] GetAllAccountPartitionIds()
		{
			return ADAccountPartitionLocator.GetAllAccountPartitionIds(false);
		}

		internal static PartitionId[] GetAllAccountPartitionIds(bool includeSecondaryPartitions)
		{
			ADAccountPartitionLocator.LoadPartitionCacheIfNecessary();
			if (!includeSecondaryPartitions)
			{
				return ADAccountPartitionLocator.allExplicitlyConfiguredPrimaryAccountPartitionIds;
			}
			return ADAccountPartitionLocator.allExplicitlyConfiguredAccountPartitionIds;
		}

		internal static Guid GetAccountPartitionGuidByPartitionId(PartitionId partitionId)
		{
			if (partitionId == null)
			{
				throw new ArgumentNullException("partitionId");
			}
			if (partitionId.PartitionObjectId != null)
			{
				return partitionId.PartitionObjectId.Value;
			}
			if (string.IsNullOrEmpty(partitionId.ForestFQDN))
			{
				throw new ArgumentNullException("ForestFQDN");
			}
			ADAccountPartitionLocator.LoadPartitionCacheIfNecessary();
			Guid result;
			if (!ADAccountPartitionLocator.partitionsFQDNToGuid.TryGetValue(partitionId.ForestFQDN, out result))
			{
				throw new CannotResolvePartitionException(DirectoryStrings.CannotResolvePartitionFqdnError(partitionId.ForestFQDN));
			}
			return result;
		}

		internal static string GetAccountPartitionFqdnByPartitionGuid(Guid partitionGuid)
		{
			ADAccountPartitionLocator.LoadPartitionCacheIfNecessary();
			string result;
			if (!ADAccountPartitionLocator.partitionsGuidToFQDN.TryGetValue(partitionGuid, out result))
			{
				throw new CannotResolvePartitionException(DirectoryStrings.CannotResolvePartitionGuidError(partitionGuid.ToString()));
			}
			return result;
		}

		internal static bool IsSingleForestTopology(out PartitionId singleForestPartition)
		{
			ADAccountPartitionLocator.LoadPartitionCacheIfNecessary();
			singleForestPartition = null;
			PartitionId[] array = ADAccountPartitionLocator.allExplicitlyConfiguredAccountPartitionIds;
			bool result = false;
			if (array.Length == 1)
			{
				singleForestPartition = array[0];
				result = true;
			}
			return result;
		}

		private static bool ValidateDomainName(AcceptedDomain domain, IConfigurationSession session, Exception duplicateAcceptedDomainException, Exception conflictingAcceptedDomainException, out Exception error)
		{
			error = null;
			string domain2 = domain.DomainName.Domain;
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Guid, domain.Guid),
				new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.DomainName, domain2),
					new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.DomainName, "*." + domain2)
				})
			});
			AcceptedDomain[] array = session.Find<AcceptedDomain>(null, QueryScope.SubTree, filter, null, 1);
			if (array.Length != 0)
			{
				error = duplicateAcceptedDomainException;
				return false;
			}
			List<QueryFilter> list = new List<QueryFilter>(AcceptedDomain.ConflictingDomainFilters(domain, false));
			QueryFilter filter2 = new OrFilter(list.ToArray());
			foreach (AcceptedDomain acceptedDomain in session.FindPaged<AcceptedDomain>(null, QueryScope.SubTree, filter2, null, 0))
			{
				if (!ADObjectId.Equals(domain.Id.Parent, acceptedDomain.Id.Parent))
				{
					error = conflictingAcceptedDomainException;
					return false;
				}
			}
			return true;
		}

		private static TenantPartitionCacheItem SearchForTenantInfoByOrganizationName(string organizationName, PartitionId partitionId)
		{
			if (string.IsNullOrEmpty(organizationName))
			{
				throw new ArgumentNullException("organizationName");
			}
			if (partitionId == null)
			{
				throw new ArgumentNullException("partitionId");
			}
			ADSessionSettings adsessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
			adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, adsessionSettings, 643, "SearchForTenantInfoByOrganizationName", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADAccountPartitionLocator.cs");
			ExchangeConfigurationUnit exchangeConfigurationUnitByNameOrAcceptedDomain = tenantConfigurationSession.GetExchangeConfigurationUnitByNameOrAcceptedDomain(organizationName);
			if (exchangeConfigurationUnitByNameOrAcceptedDomain != null)
			{
				if (ExchangeConfigurationUnit.IsInactiveRelocationNode(exchangeConfigurationUnitByNameOrAcceptedDomain))
				{
					ExTraceGlobals.GLSTracer.TraceDebug<string>(0L, "SearchForTenantInfoByOrganizationName() found organization in partition {0} - inactive due to relocation, skipping.", partitionId.ForestFQDN);
				}
				else
				{
					string externalDirectoryOrganizationId = exchangeConfigurationUnitByNameOrAcceptedDomain.ExternalDirectoryOrganizationId;
					string name = exchangeConfigurationUnitByNameOrAcceptedDomain.Id.Parent.Name;
					if (!string.IsNullOrEmpty(name))
					{
						ExTraceGlobals.GLSTracer.TraceDebug<string>(0L, "SearchForTenantInfoByOrganizationName() found organization in partition {0}.", partitionId.ForestFQDN);
						return new TenantPartitionCacheItem((partitionId.PartitionObjectId != null) ? partitionId.PartitionObjectId.Value : Guid.Empty, partitionId.ForestFQDN, PartitionId.LocalForest.ForestFQDN, new Guid(externalDirectoryOrganizationId), name, false);
					}
					ExTraceGlobals.GLSTracer.TraceDebug<string>(0L, "SearchForTenantInfoByOrganizationName() found organization in partition {0} - not fully provisioned, ExternalDirectoryOrganizationId or partitionGuid is not populated, skipping.", partitionId.ForestFQDN);
				}
			}
			return null;
		}

		private static TenantPartitionCacheItem SearchForTenantInfoByUserNetID(string userNetID, PartitionId partitionId)
		{
			GlsDirectorySession.ThrowIfInvalidNetID(userNetID, "userNetID");
			if (partitionId == null)
			{
				throw new ArgumentNullException("partitionId");
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 711, "SearchForTenantInfoByUserNetID", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADAccountPartitionLocator.cs");
			ExchangeConfigurationUnit exchangeConfigurationUnitByUserNetID = tenantConfigurationSession.GetExchangeConfigurationUnitByUserNetID(userNetID);
			if (exchangeConfigurationUnitByUserNetID != null)
			{
				string externalDirectoryOrganizationId = exchangeConfigurationUnitByUserNetID.ExternalDirectoryOrganizationId;
				string name = exchangeConfigurationUnitByUserNetID.Id.Parent.Name;
				ExTraceGlobals.GLSTracer.TraceDebug<string>(0L, "SearchForTenantInfoByUserNetID() found organization in partition {0}.", partitionId.ForestFQDN);
				return new TenantPartitionCacheItem((partitionId.PartitionObjectId != null) ? partitionId.PartitionObjectId.Value : Guid.Empty, partitionId.ForestFQDN, PartitionId.LocalForest.ForestFQDN, new Guid(externalDirectoryOrganizationId), name, false);
			}
			return null;
		}

		private static TenantPartitionCacheItem FindTenantInfoByExternalOrgId(Guid externalOrgId)
		{
			TenantPartitionCacheItem tenantPartitionCacheItem = null;
			if (externalOrgId == TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationIdGuid)
			{
				ExchangeConfigurationUnit localTemplateTenant = TemplateTenantConfiguration.GetLocalTemplateTenant();
				if (localTemplateTenant != null)
				{
					ExTraceGlobals.GLSTracer.TraceDebug<string>(0L, "FindTenantInfoByExternalOrgId() asked for Consumer Template tenant, returning {0}", localTemplateTenant.Id.Parent.DistinguishedName);
					string name = localTemplateTenant.Id.Parent.Name;
					tenantPartitionCacheItem = new TenantPartitionCacheItem(Guid.Empty, localTemplateTenant.OrganizationId.PartitionId.ForestFQDN, PartitionId.LocalForest.ForestFQDN, externalOrgId, name, false);
				}
				return tenantPartitionCacheItem;
			}
			PartitionId partitionId;
			if (!ADSessionSettings.IsGlsDisabled)
			{
				if (ADAccountPartitionLocator.TryLookUpAccountForest(externalOrgId, null, null, out tenantPartitionCacheItem))
				{
					return tenantPartitionCacheItem;
				}
			}
			else if (ADAccountPartitionLocator.IsSingleForestTopology(out partitionId))
			{
				ExTraceGlobals.GLSTracer.TraceDebug<Guid>(0L, "GLS is disabled, performing the only partition lookup for {0}", externalOrgId);
				return ADAccountPartitionLocator.SearchForTenantInfoByExternalOrgId(externalOrgId, partitionId);
			}
			if (GlsMServDirectorySession.ShouldScanAllForests)
			{
				ADAccountPartitionLocator.LoadPartitionCacheIfNecessary();
				PartitionId[] array = ADAccountPartitionLocator.allAccountPartitionIds;
				foreach (PartitionId partitionId2 in array)
				{
					tenantPartitionCacheItem = ADAccountPartitionLocator.SearchForTenantInfoByExternalOrgId(externalOrgId, partitionId2);
					if (tenantPartitionCacheItem != null)
					{
						ADAccountPartitionLocator.InsertCacheMaps(tenantPartitionCacheItem, null, null);
						return tenantPartitionCacheItem;
					}
				}
			}
			return null;
		}

		private static bool TryLookUpAccountForest(Guid externalDirectoryOrganizationId, string acceptedDomain, string msaUserNetID, out TenantPartitionCacheItem cacheItem)
		{
			cacheItem = null;
			Exception ex = null;
			if (acceptedDomain == string.Empty)
			{
				throw new ArgumentException(acceptedDomain);
			}
			if (msaUserNetID == string.Empty)
			{
				throw new ArgumentException(msaUserNetID);
			}
			try
			{
				IGlobalDirectorySession globalSession = DirectorySessionFactory.GetGlobalSession(null);
				bool dataFromOfflineService = false;
				string resourceForestFqdn;
				string text;
				string tenantName;
				bool flag;
				if (acceptedDomain != null)
				{
					if (ADAccountPartitionLocator.acceptedDomainPartitionCache.TryGetValue(acceptedDomain, out cacheItem))
					{
						ADProviderPerf.UpdateGlsCacheHitRatio(GlsLookupKey.AcceptedDomain, true);
						return true;
					}
					ADProviderPerf.UpdateGlsCacheHitRatio(GlsLookupKey.AcceptedDomain, false);
					string text2;
					flag = globalSession.TryGetTenantForestsByDomain(acceptedDomain, out externalDirectoryOrganizationId, out resourceForestFqdn, out text, out text2, out tenantName, out dataFromOfflineService);
				}
				else if (externalDirectoryOrganizationId != Guid.Empty)
				{
					if (ADAccountPartitionLocator.externalOrgIdPartitionCache.TryGetValue(externalDirectoryOrganizationId, out cacheItem))
					{
						ADProviderPerf.UpdateGlsCacheHitRatio(GlsLookupKey.ExternalDirectoryObjectId, true);
						return true;
					}
					ADProviderPerf.UpdateGlsCacheHitRatio(GlsLookupKey.ExternalDirectoryObjectId, false);
					flag = globalSession.TryGetTenantForestsByOrgGuid(externalDirectoryOrganizationId, out resourceForestFqdn, out text, out tenantName, out dataFromOfflineService);
				}
				else
				{
					if (ADAccountPartitionLocator.msaUserNetIdPartitionCache.TryGetValue(msaUserNetID, out cacheItem))
					{
						ADProviderPerf.UpdateGlsCacheHitRatio(GlsLookupKey.MSAUserNetID, true);
						return true;
					}
					ADProviderPerf.UpdateGlsCacheHitRatio(GlsLookupKey.MSAUserNetID, false);
					flag = globalSession.TryGetTenantForestsByMSAUserNetID(msaUserNetID, out externalDirectoryOrganizationId, out resourceForestFqdn, out text, out tenantName);
				}
				if (flag)
				{
					ADAccountPartitionLocator.LoadPartitionCacheIfNecessary();
					Guid accountPartitionGuid;
					ADAccountPartitionLocator.partitionsFQDNToGuid.TryGetValue(text, out accountPartitionGuid);
					cacheItem = new TenantPartitionCacheItem(accountPartitionGuid, text, resourceForestFqdn, externalDirectoryOrganizationId, tenantName, dataFromOfflineService);
					ADAccountPartitionLocator.InsertCacheMaps(cacheItem, acceptedDomain, msaUserNetID);
					return true;
				}
			}
			catch (MServTransientException ex2)
			{
				ex = ex2;
				if (!GlsMServDirectorySession.ShouldScanAllForests)
				{
					throw;
				}
			}
			catch (MServPermanentException ex3)
			{
				ex = ex3;
				if (!GlsMServDirectorySession.ShouldScanAllForests)
				{
					throw;
				}
			}
			catch (TransientException ex4)
			{
				ex = ex4;
				if (!GlsMServDirectorySession.ShouldScanAllForests)
				{
					throw;
				}
			}
			catch (GlsPermanentException ex5)
			{
				ex = ex5;
				if (!GlsMServDirectorySession.ShouldScanAllForests)
				{
					throw;
				}
			}
			catch (GlsTenantNotFoundException ex6)
			{
				ex = ex6;
				if (!GlsMServDirectorySession.ShouldScanAllForests)
				{
					throw;
				}
			}
			if (ex != null)
			{
				ExTraceGlobals.GLSTracer.TraceWarning<Exception>(0L, "Got exception while doing GLS lookup in ADAccountPartitionLocator, ignoring it until 2605034 is fixed {0}", ex);
			}
			return false;
		}

		private static TenantPartitionCacheItem SearchForTenantInfoByExternalOrgId(Guid externalOrgId, PartitionId partitionId)
		{
			if (partitionId == null)
			{
				throw new ArgumentNullException("partitionId");
			}
			if (partitionId.PartitionObjectId == null || partitionId.PartitionObjectId.Value == Guid.Empty)
			{
				throw new ArgumentNullException("partitionId.PartitionObjectId");
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 985, "SearchForTenantInfoByExternalOrgId", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADAccountPartitionLocator.cs");
			ExchangeConfigurationUnit exchangeConfigurationUnitByExternalId = tenantConfigurationSession.GetExchangeConfigurationUnitByExternalId(externalOrgId);
			if (exchangeConfigurationUnitByExternalId != null)
			{
				string name = exchangeConfigurationUnitByExternalId.Id.Parent.Name;
				if (ExchangeConfigurationUnit.IsInactiveRelocationNode(exchangeConfigurationUnitByExternalId))
				{
					ExTraceGlobals.GLSTracer.TraceDebug<string>(0L, "SearchForTenantInfoByExternalOrgId() found organization in partition {0} - inactive due to relocation, skipping.", partitionId.ForestFQDN);
				}
				else
				{
					if (!string.IsNullOrEmpty(name))
					{
						ExTraceGlobals.GLSTracer.TraceDebug<string>(0L, "SearchForTenantInfoByExternalOrgId() found organization in partition {0}.", partitionId.ForestFQDN);
						return new TenantPartitionCacheItem(partitionId.PartitionObjectId.Value, partitionId.ForestFQDN, PartitionId.LocalForest.ForestFQDN, externalOrgId, name, false);
					}
					ExTraceGlobals.GLSTracer.TraceDebug<string>(0L, "SearchForTenantInfoByExternalOrgId() found organization in partition {0} - not fully provisioned, ExternalDirectoryOrganizationId or partitionGuid is not populated, skipping.", partitionId.ForestFQDN);
				}
			}
			return null;
		}

		private static TenantPartitionCacheItem FindTenantInfoByAcceptedDomain(string acceptedDomain)
		{
			PartitionId partitionId;
			if (!ADSessionSettings.IsGlsDisabled)
			{
				TenantPartitionCacheItem tenantPartitionCacheItem;
				if (ADAccountPartitionLocator.TryLookUpAccountForest(Guid.Empty, acceptedDomain, null, out tenantPartitionCacheItem))
				{
					return tenantPartitionCacheItem;
				}
			}
			else if (ADAccountPartitionLocator.IsSingleForestTopology(out partitionId))
			{
				ExTraceGlobals.GLSTracer.TraceDebug<string>(0L, "GLS is disabled, performing the only partition lookup for {0}", acceptedDomain);
				return ADAccountPartitionLocator.SearchForTenantInfoByOrganizationName(acceptedDomain, partitionId);
			}
			if (GlsMServDirectorySession.ShouldScanAllForests)
			{
				ADAccountPartitionLocator.LoadPartitionCacheIfNecessary();
				PartitionId[] array = ADAccountPartitionLocator.allAccountPartitionIds;
				foreach (PartitionId partitionId2 in array)
				{
					TenantPartitionCacheItem tenantPartitionCacheItem = ADAccountPartitionLocator.SearchForTenantInfoByOrganizationName(acceptedDomain, partitionId2);
					if (tenantPartitionCacheItem != null)
					{
						ADAccountPartitionLocator.InsertCacheMaps(tenantPartitionCacheItem, acceptedDomain, null);
						return tenantPartitionCacheItem;
					}
				}
			}
			return null;
		}

		private static TenantPartitionCacheItem FindTenantInfoByMSAUserNetID(string msaUserNetID)
		{
			PartitionId partitionId;
			if (!ADSessionSettings.IsGlsDisabled)
			{
				TenantPartitionCacheItem result;
				if (ADAccountPartitionLocator.TryLookUpAccountForest(Guid.Empty, null, msaUserNetID, out result))
				{
					return result;
				}
			}
			else if (ADAccountPartitionLocator.IsSingleForestTopology(out partitionId))
			{
				ExTraceGlobals.GLSTracer.TraceDebug<string>(0L, "GLS is disabled, performing the only partition lookup for a Microsoft Account (MSA, formerly known as Windows Live ID) user with NetID {0}", msaUserNetID);
				return ADAccountPartitionLocator.SearchForTenantInfoByUserNetID(msaUserNetID, partitionId);
			}
			return null;
		}

		private static Guid? GetPartitionGuid(PartitionId partitionId)
		{
			if (partitionId == null)
			{
				throw new ArgumentNullException("partitionId");
			}
			Guid guid = (partitionId.PartitionObjectId == null) ? Guid.Empty : partitionId.PartitionObjectId.Value;
			if (guid == Guid.Empty)
			{
				ADAccountPartitionLocator.LoadPartitionCacheIfNecessary();
				if (!ADAccountPartitionLocator.partitionsFQDNToGuid.TryGetValue(partitionId.ForestFQDN, out guid))
				{
					return null;
				}
			}
			return new Guid?(guid);
		}

		private static void InsertCacheMaps(TenantPartitionCacheItem cacheItem, string acceptedDomain, string msaUserNetID)
		{
			bool flag = string.IsNullOrEmpty(msaUserNetID);
			if (flag)
			{
				ADAccountPartitionLocator.externalOrgIdPartitionCache.Add(cacheItem.ExternalOrgId, cacheItem);
			}
			if (!string.IsNullOrEmpty(cacheItem.TenantName))
			{
				ADAccountPartitionLocator.tenantNamePartitionCache.Add(cacheItem.TenantName, cacheItem);
			}
			if (!string.IsNullOrEmpty(acceptedDomain))
			{
				ADAccountPartitionLocator.acceptedDomainPartitionCache.Add(acceptedDomain, cacheItem);
			}
			if (!flag)
			{
				ADAccountPartitionLocator.msaUserNetIdPartitionCache.Add(msaUserNetID, cacheItem);
			}
		}

		private static AccountPartition[] ReadAccountPartitionsFromAD()
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			adsessionSettings.IncludeCNFObject = false;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, adsessionSettings, 1187, "ReadAccountPartitionsFromAD", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADAccountPartitionLocator.cs");
			return topologyConfigurationSession.FindAllAccountPartitions();
		}

		private static void RefreshAllAccountPartitions()
		{
			if (!Globals.IsMicrosoftHostedOnly)
			{
				ADAccountPartitionLocator.allAccountPartitionIds = new PartitionId[0];
				ADAccountPartitionLocator.allExplicitlyConfiguredAccountPartitionIds = new PartitionId[0];
				ADAccountPartitionLocator.allExplicitlyConfiguredPrimaryAccountPartitionIds = new PartitionId[0];
				return;
			}
			AccountPartition[] array = ADAccountPartitionLocator.ReadAccountPartitionsFromAD();
			IOrderedEnumerable<AccountPartition> orderedEnumerable = from x in array
			orderby x.WhenCreatedUTC descending
			select x;
			Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
			Dictionary<string, Guid> dictionary2 = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
			List<PartitionId> list = new List<PartitionId>(array.Length);
			List<PartitionId> list2 = new List<PartitionId>(array.Length);
			List<PartitionId> list3 = new List<PartitionId>(array.Length);
			bool flag = false;
			PartitionId partitionId = null;
			foreach (AccountPartition accountPartition in orderedEnumerable)
			{
				if (accountPartition.TryGetPartitionId(out partitionId))
				{
					list.Add(partitionId);
					dictionary.Add(partitionId.PartitionObjectId.Value, partitionId.ForestFQDN);
					dictionary2.Add(partitionId.ForestFQDN, partitionId.PartitionObjectId.Value);
					if (accountPartition.EnabledForProvisioning)
					{
						list3.Add(partitionId);
					}
					if (!accountPartition.IsSecondary)
					{
						list2.Add(partitionId);
					}
				}
				if (accountPartition.IsLocalForest)
				{
					flag = true;
				}
			}
			if (list.Count == 0)
			{
				partitionId = new PartitionId(PartitionId.LocalForest.ForestFQDN, ADObjectId.ResourcePartitionGuid);
				list.Add(partitionId);
				list3.Add(partitionId);
			}
			lock (ADAccountPartitionLocator.modifySyncObject)
			{
				ADAccountPartitionLocator.allAccountPartitionIds = list.ToArray();
				ADAccountPartitionLocator.allExplicitlyConfiguredAccountPartitionIds = list.ToArray();
				ADAccountPartitionLocator.allExplicitlyConfiguredPrimaryAccountPartitionIds = list2.ToArray();
				ADAccountPartitionLocator.allAccountPartitionIdsEnabledForProvisioning = list3.ToArray();
				ADAccountPartitionLocator.partitionsGuidToFQDN = dictionary;
				ADAccountPartitionLocator.partitionsFQDNToGuid = dictionary2;
				if (!flag)
				{
					partitionId = new PartitionId(PartitionId.LocalForest.ForestFQDN, ADObjectId.ResourcePartitionGuid);
					PartitionId[] array2 = new PartitionId[ADAccountPartitionLocator.allAccountPartitionIds.Length + 1];
					array2[0] = partitionId;
					Array.Copy(ADAccountPartitionLocator.allAccountPartitionIds, 0, array2, 1, ADAccountPartitionLocator.allAccountPartitionIds.Length);
					ADAccountPartitionLocator.allAccountPartitionIds = array2;
					ADAccountPartitionLocator.partitionsFQDNToGuid.Add(partitionId.ForestFQDN, partitionId.PartitionObjectId.Value);
					ADAccountPartitionLocator.partitionsGuidToFQDN.Add(partitionId.PartitionObjectId.Value, partitionId.ForestFQDN);
				}
			}
			ADAccountPartitionLocator.crossRFLookupCheckEnabled = ADAccountPartitionLocator.UpdateCrossRFLookupCheckEnabled();
		}

		private static void LoadPartitionCacheIfNecessary()
		{
			if (ADAccountPartitionLocator.partitionsFQDNToGuid == null)
			{
				ADAccountPartitionLocator.RefreshAllAccountPartitions();
				Interlocked.Exchange(ref ADAccountPartitionLocator.lastRefreshTick, Environment.TickCount);
				return;
			}
			int num = ADAccountPartitionLocator.lastRefreshTick;
			if (Globals.GetTickDifference(num, Environment.TickCount) > (ulong)ADAccountPartitionLocator.PartitionCacheRefreshEveryNMilliseconds)
			{
				int num2 = Interlocked.CompareExchange(ref ADAccountPartitionLocator.lastRefreshTick, Environment.TickCount, num);
				if (num == num2)
				{
					ADAccountPartitionLocator.RefreshAllAccountPartitions();
				}
			}
		}

		private static void CheckAccountPartitionIsRegistered(string accountForestFqdn, out Guid accountPartitionGuid)
		{
			ADAccountPartitionLocator.LoadPartitionCacheIfNecessary();
			if (!ADAccountPartitionLocator.partitionsFQDNToGuid.TryGetValue(accountForestFqdn, out accountPartitionGuid))
			{
				throw new CannotResolvePartitionException(DirectoryStrings.CannotResolvePartitionFqdnError(accountForestFqdn));
			}
		}

		private static void EnsureRegisteredAccountPartition(TenantPartitionCacheItem item)
		{
			if (!item.IsRegisteredAccountPartition)
			{
				Guid guid;
				ADAccountPartitionLocator.CheckAccountPartitionIsRegistered(item.AccountPartitionId.ForestFQDN, out guid);
			}
		}

		private static void EnsureAllowedCallerForUnregisteredAccountPartition(TenantPartitionCacheItem item)
		{
			if (ADAccountPartitionLocator.crossRFLookupCheckEnabled && !item.IsRegisteredAccountPartition)
			{
				ADAccountPartitionLocator.CheckCallStackForCrossRFLookup(item.AccountPartitionId.ForestFQDN);
			}
		}

		[Conditional("DEBUG")]
		private static void CheckCallStack()
		{
			StackTrace stackTrace = new StackTrace(2);
			bool flag = false;
			foreach (StackFrame stackFrame in stackTrace.GetFrames())
			{
				MethodBase method = stackFrame.GetMethod();
				if (!(method.DeclaringType == null))
				{
					string text = string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
					if (!text.StartsWith("Microsoft.Exchange") || text.StartsWith(ADAccountPartitionLocator.dedicatedClassForFanOuts) || text.StartsWith(ADAccountPartitionLocator.thisClass) || text == "Microsoft.Exchange.Configuration.Authorization.ExchangeAuthorizationPlugin.FindUserEntry" || text == "Microsoft.Exchange.Configuration.Tasks.ADIdParameter.GetConfigurationUnit" || text == "Microsoft.Exchange.Data.Directory.ADRunspaceServerSettingsProvider.GetPartitionList" || text == "Microsoft.Exchange.Configuration.Authorization.ExchangeRunspaceConfiguration.FindUserByIIdentity" || text == "Microsoft.Exchange.Directory.TopologyService.AccountForestDiscoveryMonitorWorkItem.DoWork" || text == "Microsoft.Exchange.Security.X509CertAuth.X509CertUserCache.CreateOnCacheMiss" || text == "Microsoft.Exchange.Monitoring.TestTopologyServiceTask.InternalValidate" || text == "Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning.ProvisioningDiscovery.CreateForwardSyncCompanyProbeDefinitions" || text == "Microsoft.Exchange.ProvisioningAgent.ReadOnlyOrganizationProvisioningHandler.ReadOnlyOrganizationCache.FindImmutableOrReadOnlyOrganizations" || text == "Microsoft.Exchange.RpcClientAccess.Server.UserManager.FindExchangePrincipal")
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				throw new NotSupportedException("ADAccountPartitionLocator.GetAllAccountPartitionIds() can only be called by methods of Microsoft.Exchange.Data.Directory.PartitionDataAggregator class. Current callstack: " + Environment.StackTrace);
			}
		}

		private static bool UpdateCrossRFLookupCheckEnabled()
		{
			int intValueFromRegistry = Globals.GetIntValueFromRegistry("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess", "CrossRFLookupCheckEnabled", 0, 0);
			return intValueFromRegistry != 0;
		}

		private static void CheckCallStackForCrossRFLookup(string partitionFqdn)
		{
		}

		private static readonly Guid ResourcePartitionGuid;

		private static readonly uint PartitionCacheRefreshEveryNMilliseconds = 1800000U;

		private static readonly long TenantCacheSizeInBytes = 67108864L;

		private static readonly TimeSpan TenantCacheExpiry = TimeSpan.FromSeconds((double)RegistrySettings.ExchangeServerCurrentVersion.GLSTenantCacheExpiry);

		private static PartitionId[] allAccountPartitionIdsEnabledForProvisioning;

		private static PartitionId[] allAccountPartitionIds;

		private static PartitionId[] allExplicitlyConfiguredAccountPartitionIds;

		private static PartitionId[] allExplicitlyConfiguredPrimaryAccountPartitionIds;

		private static Dictionary<Guid, string> partitionsGuidToFQDN;

		private static Dictionary<string, Guid> partitionsFQDNToGuid;

		private static object modifySyncObject;

		private static int lastRefreshTick;

		private static Cache<Guid, TenantPartitionCacheItem> externalOrgIdPartitionCache;

		private static Cache<string, TenantPartitionCacheItem> acceptedDomainPartitionCache;

		private static Cache<string, TenantPartitionCacheItem> tenantNamePartitionCache;

		private static Cache<string, TenantPartitionCacheItem> msaUserNetIdPartitionCache;

		private static bool crossRFLookupCheckEnabled = false;

		private static string dedicatedClassForFanOuts = typeof(PartitionDataAggregator).FullName;

		private static string thisClass = typeof(ADAccountPartitionLocator).FullName;
	}
}
