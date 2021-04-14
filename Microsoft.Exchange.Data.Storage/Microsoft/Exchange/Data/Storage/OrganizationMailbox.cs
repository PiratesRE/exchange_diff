using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class OrganizationMailbox
	{
		public static List<ADUser> GetOrganizationMailboxesByCapability(IRecipientSession session, OrganizationCapability capability, QueryFilter optionalFilter)
		{
			return OrganizationMailbox.InternalGetOrganizationMailboxesByCapability(session, capability, optionalFilter);
		}

		public static List<ADUser> GetOrganizationMailboxesByCapability(IRecipientSession session, OrganizationCapability capability)
		{
			return OrganizationMailbox.InternalGetOrganizationMailboxesByCapability(session, capability, null);
		}

		public static void GetOrganizationMailboxesByCapability(IRecipientSession session, OrganizationCapability capability, int minVersion, int maxVersion, out List<ADUser> filteredList, out List<ADUser> completeList)
		{
			completeList = OrganizationMailbox.GetOrganizationMailboxesByCapability(session, capability);
			filteredList = new List<ADUser>();
			foreach (ADUser aduser in completeList)
			{
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(session.SessionSettings, aduser, RemotingOptions.AllowCrossSite);
				if (exchangePrincipal.MailboxInfo.Location.ServerVersion >= minVersion && exchangePrincipal.MailboxInfo.Location.ServerVersion < maxVersion)
				{
					filteredList.Add(aduser);
				}
			}
		}

		public static void GetOrganizationMailboxesByCapability(IRecipientSession session, OrganizationCapability capability, ADObjectId siteId, out List<ADUser> filteredList, out List<ADUser> completeList)
		{
			ExAssert.RetailAssert(!OrganizationMailbox.IsMultiTenantEnvironment(), "GetOrganizationMailboxesByCapability (site-based) should not be used in Datacenter Multitenant environment");
			ExTraceGlobals.StorageTracer.TraceDebug<OrganizationCapability>(0L, "Entering GetOrganizationMailboxByCapability capability='{0}'", capability);
			if (siteId == null)
			{
				throw new ArgumentNullException("siteId");
			}
			completeList = OrganizationMailbox.GetOrganizationMailboxesByCapability(session, capability);
			ExTraceGlobals.StorageTracer.TraceDebug<int, OrganizationCapability>(0L, "GetOrganizationMailboxByCapability -completeList of mailbox, count='{0}', capability='{1}'", completeList.Count, capability);
			filteredList = new List<ADUser>();
			if (completeList.Count > 0)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<OrganizationCapability>(0L, "GetOrganizationMailboxByCapability -GetCurrentServiceTopology capability='{0}'", capability);
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\OrganizationMailbox\\OrganizationMailbox.cs", "GetOrganizationMailboxesByCapability", 137);
				foreach (ADUser aduser in completeList)
				{
					ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(session.SessionSettings, aduser, RemotingOptions.AllowCrossSite);
					string serverFqdn = exchangePrincipal.MailboxInfo.Location.ServerFqdn;
					ExTraceGlobals.StorageTracer.TraceDebug<string, OrganizationCapability>(0L, "GetOrganizationMailboxByCapability calling GetSite with Prinicipal.ServerFQDN='{0}', capability='{1}'", serverFqdn, capability);
					Site site = currentServiceTopology.GetSite(serverFqdn, "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\OrganizationMailbox\\OrganizationMailbox.cs", "GetOrganizationMailboxesByCapability", 151);
					Guid objectGuid = site.Id.ObjectGuid;
					ExTraceGlobals.StorageTracer.TraceDebug<string, Guid, OrganizationCapability>(0L, "GetOrganizationMailboxByCapability  GetSite called with Prinicipal.ServerFQDN='{0}', Site='{1}'capability='{2}'", serverFqdn, objectGuid, capability);
					if (siteId.ObjectGuid.Equals(objectGuid))
					{
						filteredList.Add(aduser);
					}
				}
			}
		}

		public static ADUser GetOrganizationMailboxByUPNAndCapability(IRecipientSession session, string targetOrgMailbox, OrganizationCapability capability)
		{
			Util.ThrowOnNullOrEmptyArgument(targetOrgMailbox, "targetOrgMailbox");
			QueryFilter optionalFilter = new TextFilter(ADUserSchema.UserPrincipalName, targetOrgMailbox, MatchOptions.FullString, MatchFlags.IgnoreCase);
			List<ADUser> list = OrganizationMailbox.InternalGetOrganizationMailboxesByCapability(session, capability, optionalFilter);
			if (list.Count != 1)
			{
				ExTraceGlobals.StorageTracer.TraceError<string, OrganizationCapability, OrganizationId>(0L, "[GetOrganizationMailboxByUPNAndCapacity] Unable to find organization mailbox with UPN {0} capability {1} for organization {2}.", targetOrgMailbox, capability, session.SessionSettings.CurrentOrganizationId);
				return null;
			}
			return list[0];
		}

		public static ADUser GetLocalOrganizationMailboxByCapability(OrganizationId orgId, OrganizationCapability capability, bool allowRehoming = true)
		{
			Util.ThrowOnNullArgument(orgId, "orgId");
			EnumValidator.ThrowIfInvalid<OrganizationCapability>(capability, "capability");
			ExTraceGlobals.StorageTracer.TraceDebug<OrganizationId, OrganizationCapability>(0L, "GetLocalOrganizationMailboxByCapability orgId='{0}', capability='{1}'", orgId, capability);
			IRecipientSession session = OrganizationMailbox.GetSession(orgId, allowRehoming);
			List<ADUser> organizationMailboxesByCapability = OrganizationMailbox.GetOrganizationMailboxesByCapability(session, capability);
			if (organizationMailboxesByCapability == null || organizationMailboxesByCapability.Count == 0)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<OrganizationId, OrganizationCapability>(0L, "GetLocalOrganizationMailboxByCapability orgId='{0}', capability='{1}' -- No org mailboxes found", orgId, capability);
				return null;
			}
			Server localServer = LocalServerCache.LocalServer;
			if (localServer == null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<OrganizationId, OrganizationCapability>(0L, "GetLocalOrganizationMailboxByCapability orgId='{0}', capability='{1}' -- No local server found", orgId, capability);
				return null;
			}
			Database[] databases = localServer.GetDatabases();
			if (databases == null || databases.Length == 0)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<OrganizationId, OrganizationCapability>(0L, "GetLocalOrganizationMailboxByCapability orgId='{0}', capability='{1}' -- No local databases found", orgId, capability);
				return null;
			}
			foreach (ADUser aduser in organizationMailboxesByCapability)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<OrganizationId, OrganizationCapability, string>(0L, "GetLocalOrganizationMailboxByCapability orgId='{0}', capability='{1}' -- Processing mbox='{2}'", orgId, capability, aduser.DistinguishedName);
				foreach (Database database in databases)
				{
					if (aduser.Database.Equals(database.Id))
					{
						ExTraceGlobals.StorageTracer.TraceDebug<OrganizationId, OrganizationCapability, string>(0L, "GetLocalOrganizationMailboxByCapability orgId='{0}', capability='{1}' -- returning mbox='{2}'", orgId, capability, aduser.DistinguishedName);
						return aduser;
					}
				}
			}
			return null;
		}

		public static bool IsOrganizationAnchoredOnLocalServer(OrganizationId orgId, bool allowRehoming = true)
		{
			Util.ThrowOnNullArgument(orgId, "orgId");
			ExTraceGlobals.StorageTracer.TraceDebug<OrganizationId>(0L, "IsOrganizationAnchoredOnLocalServer orgId='{0}'", orgId);
			OrganizationCapability organizationCapability = OrganizationCapability.Management;
			List<ADUser> organizationMailboxesByCapability = OrganizationMailbox.GetOrganizationMailboxesByCapability(OrganizationMailbox.GetSession(orgId, allowRehoming), organizationCapability);
			if (organizationMailboxesByCapability == null || organizationMailboxesByCapability.Count == 0)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<OrganizationId, OrganizationCapability>(0L, "IsOrganizationAnchoredOnLocalServer orgId='{0}', capability='{1}' -- No org mailboxes found", orgId, organizationCapability);
				return false;
			}
			ADUser aduser = organizationMailboxesByCapability[0];
			Server localServer = LocalServerCache.LocalServer;
			if (localServer == null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug(0L, "IsOrganizationAnchoredOnLocalServer - No local server found");
				return false;
			}
			List<Guid> list = new List<Guid>(new Guid[]
			{
				aduser.Database.ObjectGuid
			});
			ExTraceGlobals.StorageTracer.TraceDebug<OrganizationId, string, int>(0L, "IsOrganizationAnchoredOnLocalServer orgId='{0}' -- invoking MailboxAdminHelper.GetOnlineDatabase() for server {1} and list of {2} guids", orgId, localServer.Fqdn, list.Count);
			List<Guid> onlineDatabase = MailboxAdminHelper.GetOnlineDatabase(localServer.Fqdn, list);
			ExTraceGlobals.StorageTracer.TraceDebug<OrganizationId, int>(0L, "IsOrganizationAnchoredOnLocalServer orgId='{0}' -- MailboxAdminHelper.GetOnlineDatabase() returned list of {1} guids", orgId, onlineDatabase.Count);
			return onlineDatabase.Count > 0;
		}

		public static ADUser[] FindByDatabaseId(OrganizationCapability capability, ADObjectId databaseId)
		{
			EnumValidator.ThrowIfInvalid<OrganizationCapability>(capability, "capability");
			if (!OrganizationMailbox.IsMultiTenantEnvironment())
			{
				return OrganizationMailbox.InternalFindEnterprise(capability, databaseId);
			}
			return OrganizationMailbox.InternalFindMultiTenant(capability, databaseId);
		}

		public static ADUser[] FindByLocalDatabaseAvailablityGroup(OrganizationCapability capability)
		{
			return OrganizationMailbox.FindByDatabaseId(capability, null);
		}

		public static ADUser[] FindByOrganizationId(OrganizationId organizationId, OrganizationCapability capability)
		{
			return OrganizationMailbox.GetOrganizationMailboxesByCapability(OrganizationMailbox.GetSession(organizationId, true), capability).ToArray();
		}

		public static bool TryFindByOrganizationId(OrganizationId organizationId, ADObjectId siteId, OrganizationCapability capability, out List<ADUser> siteUsers, out List<ADUser> allUsers)
		{
			ExAssert.RetailAssert(!OrganizationMailbox.IsMultiTenantEnvironment(), "TryFindByOrganizationId (site-based) should not be used in Datacenter Multitenant environment");
			OrganizationMailbox.GetOrganizationMailboxesByCapability(OrganizationMailbox.GetSession(organizationId, true), capability, siteId, out siteUsers, out allUsers);
			return allUsers.Count > 0;
		}

		private static IRecipientSession GetSession(OrganizationId organizationId, bool allowRehoming = true)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), organizationId, null, false, allowRehoming);
			if (!allowRehoming)
			{
				adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			}
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, LcidMapper.DefaultLcid, true, ConsistencyMode.IgnoreInvalid, null, adsessionSettings, 449, "GetSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\OrganizationMailbox\\OrganizationMailbox.cs");
		}

		public static string GetActiveServerFqdn(ADObjectId organizationMailboxId)
		{
			return OrganizationMailbox.GetExchangePrincipal(organizationMailboxId).MailboxInfo.Location.ServerFqdn;
		}

		public static ADObjectId GetActiveServerSite(ADObjectId organizationMailboxId)
		{
			return OrganizationMailbox.GetExchangePrincipal(organizationMailboxId).MailboxInfo.Location.ServerSite;
		}

		public static ExchangeConfigurationUnit GetExchangeConfigurationUnit(ADObjectId organizationMailboxId)
		{
			if (!ADSession.IsTenantIdentity(organizationMailboxId, organizationMailboxId.GetPartitionId().ForestFQDN))
			{
				throw new InvalidOperationException();
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsObjectId(organizationMailboxId);
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 495, "GetExchangeConfigurationUnit", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\OrganizationMailbox\\OrganizationMailbox.cs");
			ExchangePrincipal exchangePrincipal = OrganizationMailbox.GetExchangePrincipal(organizationMailboxId);
			return tenantConfigurationSession.Read<ExchangeConfigurationUnit>(exchangePrincipal.MailboxInfo.OrganizationId.ConfigurationUnit);
		}

		private static ExchangePrincipal GetExchangePrincipal(ADObjectId organizationMailboxId)
		{
			IRecipientSession recipientSession;
			if (ADSession.IsTenantIdentity(organizationMailboxId, organizationMailboxId.GetPartitionId().ForestFQDN))
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsObjectId(organizationMailboxId);
				recipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(null, null, LcidMapper.DefaultLcid, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 515, "GetExchangePrincipal", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\OrganizationMailbox\\OrganizationMailbox.cs");
			}
			else
			{
				ADSessionSettings sessionSettings2 = ADSessionSettings.FromRootOrgScopeSet();
				recipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(null, null, LcidMapper.DefaultLcid, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings2, 528, "GetExchangePrincipal", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\OrganizationMailbox\\OrganizationMailbox.cs");
			}
			ADUser aduser = recipientSession.Read(organizationMailboxId) as ADUser;
			if (aduser == null)
			{
				throw new ADNoSuchObjectException(DirectoryStrings.OrganizationMailboxNotFound(organizationMailboxId.ToString()));
			}
			return ExchangePrincipal.FromADUser(aduser, RemotingOptions.AllowCrossSite);
		}

		private static ADUser[] InternalFindEnterprise(OrganizationCapability capability, ADObjectId databaseId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			IRootOrganizationRecipientSession session = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(null, null, LcidMapper.DefaultLcid, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 558, "InternalFindEnterprise", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\OrganizationMailbox\\OrganizationMailbox.cs");
			QueryFilter optionalFilter;
			if (databaseId != null)
			{
				optionalFilter = new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.Database, databaseId);
			}
			else
			{
				optionalFilter = OrganizationMailbox.GetOrganizationMailboxQueryFilterForLocalServer();
			}
			List<ADUser> organizationMailboxesByCapability = OrganizationMailbox.GetOrganizationMailboxesByCapability(session, capability, optionalFilter);
			List<ADUser> list = new List<ADUser>(1);
			if (organizationMailboxesByCapability.Count >= 1)
			{
				list.Add(organizationMailboxesByCapability[0]);
			}
			return list.ToArray();
		}

		private static int UserComparer(ADUser user1, ADUser user2)
		{
			DateTime t = (user1.WhenCreatedUTC != null) ? user1.WhenCreatedUTC.Value : DateTime.MinValue;
			DateTime t2 = (user2.WhenCreatedUTC != null) ? user2.WhenCreatedUTC.Value : DateTime.MinValue;
			return DateTime.Compare(t, t2);
		}

		private static QueryFilter GetOrganizationMailboxQueryFilterForLocalServer()
		{
			Server server = LocalServer.GetServer();
			Database[] databases = server.GetDatabases();
			if (databases.Length > 0)
			{
				QueryFilter[] array = new QueryFilter[databases.Length];
				for (int i = 0; i < databases.Length; i++)
				{
					array[i] = new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.Database, databases[i].Id);
				}
				return QueryFilter.OrTogether(array);
			}
			return new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.ServerName, server.Name);
		}

		private static ADUser[] InternalFindMultiTenant(OrganizationCapability capability, ADObjectId databaseId)
		{
			QueryFilter additionalfilter = null;
			if (databaseId != null)
			{
				additionalfilter = new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.Database, databaseId);
			}
			else
			{
				additionalfilter = OrganizationMailbox.GetOrganizationMailboxQueryFilterForLocalServer();
			}
			Dictionary<OrganizationId, ADUser> orgMailboxesDictionary = new Dictionary<OrganizationId, ADUser>();
			PartitionDataAggregator.RunOperationOnAllAccountPartitions(true, delegate(IRecipientSession recipientSession)
			{
				List<ADUser> organizationMailboxesByCapability = OrganizationMailbox.GetOrganizationMailboxesByCapability(recipientSession, capability, additionalfilter);
				foreach (ADUser aduser in organizationMailboxesByCapability)
				{
					ADUser user = null;
					if (orgMailboxesDictionary.TryGetValue(aduser.OrganizationId, out user))
					{
						if (OrganizationMailbox.UserComparer(user, aduser) > 0)
						{
							orgMailboxesDictionary[aduser.OrganizationId] = aduser;
						}
					}
					else
					{
						orgMailboxesDictionary[aduser.OrganizationId] = aduser;
					}
				}
			});
			ADUser[] array = new ADUser[orgMailboxesDictionary.Count];
			if (orgMailboxesDictionary.Count > 0)
			{
				orgMailboxesDictionary.Values.CopyTo(array, 0);
			}
			return array;
		}

		private static List<ADUser> InternalGetOrganizationMailboxesByCapability(IRecipientSession session, OrganizationCapability capability, QueryFilter optionalFilter)
		{
			EnumValidator.ThrowIfInvalid<OrganizationCapability>(capability, "capability");
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			List<ADUser> list = new List<ADUser>();
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				OrganizationMailbox.OrganizationMailboxFilterBase,
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RawCapabilities, capability)
			});
			if (optionalFilter != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					optionalFilter
				});
			}
			ADObjectId rootId;
			QueryScope scope;
			OrganizationMailbox.GetQueryParameters(session, out rootId, out scope);
			ADPagedReader<ADRecipient> adpagedReader = session.FindPaged(rootId, scope, queryFilter, new SortBy(ADObjectSchema.WhenCreated, SortOrder.Ascending), 0);
			if (adpagedReader != null)
			{
				foreach (ADRecipient adrecipient in adpagedReader)
				{
					ADUser aduser = adrecipient as ADUser;
					if (aduser != null)
					{
						list.Add(aduser);
					}
				}
			}
			return list;
		}

		private static void GetQueryParameters(IRecipientSession session, out ADObjectId rootId, out QueryScope queryScope)
		{
			rootId = null;
			queryScope = QueryScope.SubTree;
			OrganizationId currentOrganizationId = session.SessionSettings.CurrentOrganizationId;
			ConfigScopes configScopes = session.SessionSettings.ConfigScopes;
			if (currentOrganizationId == OrganizationId.ForestWideOrgId && OrganizationMailbox.IsScopeLimitedToFirstOrg(configScopes) && OrganizationMailbox.IsMultiTenantEnvironment())
			{
				ExTraceGlobals.StorageTracer.TraceDebug(0L, "Scoping search to First Org Users container in datacenter");
				rootId = ADSystemConfigurationSession.GetFirstOrgUsersContainerId();
				queryScope = QueryScope.OneLevel;
				return;
			}
			ExTraceGlobals.StorageTracer.TraceDebug(0L, "Scoping search to rootId null");
		}

		private static bool IsScopeLimitedToFirstOrg(ConfigScopes configScope)
		{
			return configScope == ConfigScopes.RootOrg || configScope == ConfigScopes.TenantLocal || configScope == ConfigScopes.TenantSubTree || configScope == ConfigScopes.Global;
		}

		private static bool IsMultiTenantEnvironment()
		{
			bool result = false;
			try
			{
				result = (VariantConfiguration.InvariantNoFlightingSnapshot.DataStorage.FindOrgMailboxInMultiTenantEnvironment.Enabled || Datacenter.IsPartnerHostedOnly(true));
			}
			catch (CannotDetermineExchangeModeException)
			{
				ExTraceGlobals.StorageTracer.TraceWarning(0L, "Could not determine Exchange mode in IsMultiTenantEnvironment");
			}
			return result;
		}

		public static readonly QueryFilter OrganizationMailboxFilterBase = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox),
			new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.ArbitrationMailbox)
		});
	}
}
