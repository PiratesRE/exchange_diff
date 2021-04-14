using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Provisioning.LoadBalancing
{
	internal static class PhysicalResourceLoadBalancing
	{
		internal static ADObjectId FindDatabase(LogMessageDelegate logger)
		{
			LoadBalancingReport loadBalancingReport = null;
			MailboxDatabaseWithLocationInfo mailboxDatabaseWithLocationInfo = PhysicalResourceLoadBalancing.FindDatabaseAndLocation(null, logger, null, true, false, null, ref loadBalancingReport);
			if (mailboxDatabaseWithLocationInfo != null)
			{
				return mailboxDatabaseWithLocationInfo.MailboxDatabase.Id;
			}
			return null;
		}

		internal static List<MailboxDatabase> GetAllCachedDatabasesForProvisioning(string domainController, LogMessageDelegate logger)
		{
			ITopologyConfigurationSession configSession = PhysicalResourceLoadBalancing.CreateGlobalConfigSession(domainController);
			return PhysicalResourceLoadBalancing.GetDatabasesForProvisioningCached(configSession, false, logger);
		}

		private static MailboxDatabaseWithLocationInfo FindDatabaseAndLocation(IList<MailboxDatabase> allDatabases, ActiveManager activeManager, bool isInitialProvisioning, bool localSiteDatabasesOnly, LogMessageDelegate logger, ref LoadBalancingReport loadBalancingReport)
		{
			return PhysicalResourceLoadBalancing.FindDatabaseAndLocation(allDatabases, activeManager, isInitialProvisioning, localSiteDatabasesOnly, logger, null, ref loadBalancingReport);
		}

		private static MailboxDatabaseWithLocationInfo FindDatabaseAndLocation(IList<MailboxDatabase> allDatabases, ActiveManager activeManager, bool isInitialProvisioning, bool localSiteDatabasesOnly, LogMessageDelegate logger, int? qualifiedMinServerVersion, ref LoadBalancingReport loadBalancingReport)
		{
			int num = (int)Math.Round((double)allDatabases.Count * 0.1);
			num = ((num < 10) ? Math.Min(allDatabases.Count, 10) : num);
			List<MailboxDatabaseWithLocationInfo> list = new List<MailboxDatabaseWithLocationInfo>();
			List<MailboxDatabaseWithLocationInfo> list2 = new List<MailboxDatabaseWithLocationInfo>();
			Random random = new Random();
			while (allDatabases.Count > 0 && list.Count + list2.Count <= num)
			{
				int index = random.Next(allDatabases.Count);
				MailboxDatabase mailboxDatabase = allDatabases[index];
				if (isInitialProvisioning && mailboxDatabase.IsExcludedFromInitialProvisioning)
				{
					loadBalancingReport.databasesExcludedFromInitialProvisioning++;
					PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbInitialProvisioningDatabaseExcluded(mailboxDatabase.Name), logger);
				}
				else
				{
					DatabaseLocationInfo serverForActiveDatabaseCopy = PhysicalResourceLoadBalancing.GetServerForActiveDatabaseCopy(mailboxDatabase, activeManager, logger);
					if (serverForActiveDatabaseCopy != null && (qualifiedMinServerVersion == null || serverForActiveDatabaseCopy.ServerVersion >= qualifiedMinServerVersion.Value))
					{
						if (!localSiteDatabasesOnly || PhysicalResourceLoadBalancing.IsDatabaseInLocalSite(serverForActiveDatabaseCopy, logger))
						{
							if (list2.Count < 3)
							{
								return new MailboxDatabaseWithLocationInfo(mailboxDatabase, serverForActiveDatabaseCopy);
							}
							list.Add(new MailboxDatabaseWithLocationInfo(mailboxDatabase, serverForActiveDatabaseCopy));
						}
						else
						{
							list2.Add(new MailboxDatabaseWithLocationInfo(mailboxDatabase, serverForActiveDatabaseCopy));
						}
					}
				}
				allDatabases.RemoveAt(index);
			}
			loadBalancingReport.databasesAndLocationCount = list.Count + list2.Count;
			loadBalancingReport.firstPreferenceDatabasesCount = list.Count;
			loadBalancingReport.secondPreferenceDatabasesCount = list2.Count;
			if (list.Count + list2.Count <= 0)
			{
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbNoAvailableDatabase, logger);
				return null;
			}
			int maxValue = (int)Math.Ceiling((double)(list.Count + list2.Count) * 0.3);
			int num2 = random.Next(maxValue);
			if (num2 < list.Count)
			{
				return list[num2];
			}
			MailboxDatabaseWithLocationInfo mailboxDatabaseWithLocationInfo = list2[num2 - list.Count];
			PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbRemoteSiteDatabaseReturned(mailboxDatabaseWithLocationInfo.MailboxDatabase.Name, mailboxDatabaseWithLocationInfo.DatabaseLocationInfo.ServerFqdn), logger);
			return mailboxDatabaseWithLocationInfo;
		}

		private static DatabaseLocationInfo GetServerForActiveDatabaseCopy(MailboxDatabase database, ActiveManager activeManager, LogMessageDelegate logger)
		{
			Exception ex = null;
			DatabaseLocationInfo result = null;
			try
			{
				result = activeManager.GetServerForDatabase(database.Id.ObjectGuid);
			}
			catch (DatabaseNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (ObjectNotFoundException ex3)
			{
				ex = ex3;
			}
			catch (StoragePermanentException ex4)
			{
				ex = ex4;
			}
			catch (StorageTransientException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseDatabaseNotFound(database.Id.ObjectGuid.ToString(), ex.Message), logger);
			}
			return result;
		}

		internal static MailboxDatabaseWithLocationInfo FindDatabaseAndLocation(string domainController, LogMessageDelegate logger, ScopeSet scopeSet, bool isInitialProvisioning, bool localSiteDatabasesOnly, IMailboxProvisioningConstraint mailboxProvisioningConstraint, ref LoadBalancingReport loadBalancingReport)
		{
			return PhysicalResourceLoadBalancing.FindDatabaseAndLocation(domainController, logger, scopeSet, isInitialProvisioning, localSiteDatabasesOnly, null, mailboxProvisioningConstraint, null, ref loadBalancingReport);
		}

		internal static MailboxDatabaseWithLocationInfo FindDatabaseAndLocation(string domainController, LogMessageDelegate logger, ScopeSet scopeSet, bool isInitialProvisioning, bool localSiteDatabasesOnly, int? qualifiedMinServerVersion, IMailboxProvisioningConstraint mailboxProvisioningConstraint, IEnumerable<ADObjectId> excludedDatabaseIds, ref LoadBalancingReport loadBalancingReport)
		{
			ITopologyConfigurationSession configSession = PhysicalResourceLoadBalancing.CreateGlobalConfigSession(domainController);
			List<MailboxDatabase> databasesForProvisioningCached = PhysicalResourceLoadBalancing.GetDatabasesForProvisioningCached(configSession, localSiteDatabasesOnly, logger);
			List<MailboxDatabase> list;
			if (mailboxProvisioningConstraint != null || excludedDatabaseIds != null)
			{
				list = PhysicalResourceLoadBalancing.FilterEligibleDatabase(logger, databasesForProvisioningCached.GetRange(0, databasesForProvisioningCached.Count), mailboxProvisioningConstraint, excludedDatabaseIds);
			}
			else
			{
				list = databasesForProvisioningCached.GetRange(0, databasesForProvisioningCached.Count);
			}
			if (list.Count == 0)
			{
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbNoAvailableDatabase, logger);
				return null;
			}
			LoadBalancingReport loadBalancingReport2 = loadBalancingReport ?? new LoadBalancingReport();
			loadBalancingReport2.enabledDatabasesWithLocalCopyCount = list.Count;
			ActiveManager cachingActiveManagerInstance = ActiveManager.GetCachingActiveManagerInstance();
			if (PhysicalResourceLoadBalancing.IsDatacenter)
			{
				return PhysicalResourceLoadBalancing.FindDatabaseAndLocation(list, cachingActiveManagerInstance, isInitialProvisioning, localSiteDatabasesOnly, logger, ref loadBalancingReport2);
			}
			List<MailboxDatabaseWithLocationInfo> list2 = new List<MailboxDatabaseWithLocationInfo>(list.Count);
			foreach (MailboxDatabase mailboxDatabase in list)
			{
				DatabaseLocationInfo serverForActiveDatabaseCopy = PhysicalResourceLoadBalancing.GetServerForActiveDatabaseCopy(mailboxDatabase, cachingActiveManagerInstance, logger);
				if (serverForActiveDatabaseCopy != null && PhysicalResourceLoadBalancing.IsDatabaseInLocalSite(serverForActiveDatabaseCopy, logger) && (qualifiedMinServerVersion == null || serverForActiveDatabaseCopy.ServerVersion >= qualifiedMinServerVersion.Value))
				{
					list2.Add(new MailboxDatabaseWithLocationInfo(mailboxDatabase, serverForActiveDatabaseCopy));
				}
			}
			return PhysicalResourceLoadBalancing.VerifyStatusAndSelectDB(list2, domainController, logger, scopeSet);
		}

		private static List<MailboxDatabase> FilterEligibleDatabase(LogMessageDelegate logger, IEnumerable<MailboxDatabase> cachedDatabases, IMailboxProvisioningConstraint mailboxProvisioningConstraint, IEnumerable<ADObjectId> excludedDatabaseIds)
		{
			List<MailboxDatabase> list = new List<MailboxDatabase>();
			int num = 0;
			foreach (MailboxDatabase mailboxDatabase in cachedDatabases)
			{
				num++;
				bool flag = mailboxProvisioningConstraint == null || (mailboxDatabase.MailboxProvisioningAttributes != null && mailboxProvisioningConstraint.IsMatch(mailboxDatabase.MailboxProvisioningAttributes));
				bool flag2 = excludedDatabaseIds != null && excludedDatabaseIds.Contains(mailboxDatabase.Id);
				if (flag && !flag2)
				{
					list.Add(mailboxDatabase);
				}
			}
			PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbGeneralTrace(string.Format("{0}/{1} valid database(s) match constraints required by mailbox.", list.Count, num)), logger);
			return list;
		}

		internal static MailboxDatabaseWithLocationInfo FindDatabaseAndLocationForEnterpriseSiteMailbox(string domainController, LogMessageDelegate logger, ScopeSet scopeSet)
		{
			PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbEnterSiteMailboxEnterprise, logger);
			ITopologyConfigurationSession configSession = PhysicalResourceLoadBalancing.CreateGlobalConfigSession(domainController);
			List<MailboxDatabase> databasesForProvisioningCached = PhysicalResourceLoadBalancing.GetDatabasesForProvisioningCached(configSession, false, logger);
			if (databasesForProvisioningCached.Count == 0)
			{
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbNoAvailableDatabase, logger);
				return null;
			}
			List<MailboxDatabase> list = new List<MailboxDatabase>();
			foreach (MailboxDatabase mailboxDatabase in databasesForProvisioningCached)
			{
				if (mailboxDatabase.AdminDisplayVersion == null)
				{
					mailboxDatabase.AdminDisplayVersion = Server.GetServerVersion(mailboxDatabase.ServerName);
				}
				if (mailboxDatabase.AdminDisplayVersion.Major >= PhysicalResourceLoadBalancing.MajorVersionE15)
				{
					list.Add(mailboxDatabase);
				}
			}
			if (list.Count == 0)
			{
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbNoAvailableE15Database(databasesForProvisioningCached.Count), logger);
				return null;
			}
			ActiveManager cachingActiveManagerInstance = ActiveManager.GetCachingActiveManagerInstance();
			Random random = new Random();
			while (list.Count != 0)
			{
				int index = random.Next(list.Count);
				MailboxDatabase mailboxDatabase2 = list[index];
				list.RemoveAt(index);
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbGetServerForActiveDatabaseCopy(mailboxDatabase2.Name), logger);
				DatabaseLocationInfo serverForActiveDatabaseCopy = PhysicalResourceLoadBalancing.GetServerForActiveDatabaseCopy(mailboxDatabase2, cachingActiveManagerInstance, logger);
				if (serverForActiveDatabaseCopy != null)
				{
					MailboxDatabaseWithLocationInfo mailboxDatabaseWithLocationInfo = new MailboxDatabaseWithLocationInfo(mailboxDatabase2, serverForActiveDatabaseCopy);
					if (mailboxDatabase2.MasterType == MasterType.DatabaseAvailabilityGroup)
					{
						PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbRemoteSiteDatabaseReturned(mailboxDatabaseWithLocationInfo.MailboxDatabase.Name, mailboxDatabaseWithLocationInfo.DatabaseLocationInfo.ServerFqdn), logger);
						return mailboxDatabaseWithLocationInfo;
					}
					mailboxDatabaseWithLocationInfo = PhysicalResourceLoadBalancing.VerifyStatusAndSelectDB(new List<MailboxDatabaseWithLocationInfo>
					{
						mailboxDatabaseWithLocationInfo
					}, domainController, logger, scopeSet);
					if (mailboxDatabaseWithLocationInfo != null)
					{
						return mailboxDatabaseWithLocationInfo;
					}
				}
			}
			return null;
		}

		internal static Server FindMailboxServer(string domainController, OfflineAddressBook currentOABSettings, LogMessageDelegate logger)
		{
			ITopologyConfigurationSession topologyConfigurationSession = PhysicalResourceLoadBalancing.CreateGlobalConfigSession(domainController);
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, Server.E14MinVersion),
				new BitMaskAndFilter(ServerSchema.CurrentServerRole, 2UL),
				new ComparisonFilter(ComparisonOperator.Equal, ActiveDirectoryServerSchema.IsExcludedFromProvisioning, false)
			});
			PhysicalResourceLoadBalancing.LogVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(topologyConfigurationSession, typeof(Server), filter, null, true), logger);
			ADPagedReader<Server> adpagedReader = topologyConfigurationSession.FindPaged<Server>(null, QueryScope.SubTree, filter, null, 0);
			List<Server> list = new List<Server>();
			foreach (Server server in adpagedReader)
			{
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbFoundMailboxServer(server.Identity.ToString()), logger);
				list.Add(server);
			}
			if (currentOABSettings.Server != null)
			{
				Server currentOABServer = topologyConfigurationSession.Read<Server>(currentOABSettings.Server);
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbOABIsCurrentlyOnServer((currentOABServer == null) ? Strings.VerboseLbDeletedServer : currentOABServer.Identity.ToString()), logger);
				if (currentOABServer != null && list.Find((Server s) => ADObjectId.Equals(s.Id, currentOABServer.Id)) != null)
				{
					if (list.Count == 1)
					{
						PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbOnlyOneEligibleServer(currentOABServer.Identity.ToString()), logger);
						return currentOABServer;
					}
					list.RemoveAll((Server s) => ADObjectId.Equals(s.Id, currentOABServer.Id));
				}
			}
			if (list.Count == 0)
			{
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbNoEligibleServers, logger);
				return null;
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			Random random = new Random();
			Server server2 = list[random.Next(0, list.Count)];
			PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbReturningServer(server2.Identity.ToString()), logger);
			return server2;
		}

		internal static IEnumerable FindVirtualDirectories(string domainController, LogMessageDelegate logger)
		{
			ITopologyConfigurationSession topologyConfigurationSession = PhysicalResourceLoadBalancing.CreateGlobalConfigSession(domainController);
			PhysicalResourceLoadBalancing.LogVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(topologyConfigurationSession, typeof(ADOabVirtualDirectory), null, null, true), logger);
			ADPagedReader<ADOabVirtualDirectory> adpagedReader = topologyConfigurationSession.FindPaged<ADOabVirtualDirectory>(null, QueryScope.SubTree, null, null, 0);
			List<ComparableEntry<ADOabVirtualDirectory>> list = new List<ComparableEntry<ADOabVirtualDirectory>>();
			foreach (ADOabVirtualDirectory adoabVirtualDirectory in adpagedReader)
			{
				ServerVersion serverVersion = null;
				if (adoabVirtualDirectory.Server != null)
				{
					serverVersion = Server.GetServerVersion(adoabVirtualDirectory.Server.Name);
				}
				if (!(serverVersion == null) && ServerVersion.Compare(serverVersion, PhysicalResourceLoadBalancing.E15MinVersion) >= 0)
				{
					list.Add(new ComparableEntry<ADOabVirtualDirectory>(adoabVirtualDirectory)
					{
						Count = adoabVirtualDirectory.OfflineAddressBooks.Count
					});
					PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbFoundOabVDir(adoabVirtualDirectory.Id.ToDNString(), adoabVirtualDirectory.OfflineAddressBooks.Count), logger);
				}
			}
			list.Sort();
			int num = PhysicalResourceLoadBalancing.MaxNumOfVdirs;
			if (num > list.Count)
			{
				num = list.Count;
			}
			if (num == 0)
			{
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbNoOabVDirReturned, logger);
			}
			List<ADObjectId> list2 = new List<ADObjectId>(num);
			for (int i = 0; i < num; i++)
			{
				list2.Add(list[i].Entry.Id);
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbOabVDirSelected(list[i].Entry.Id.ToDNString()), logger);
			}
			return list2;
		}

		private static MailboxDatabaseWithLocationInfo VerifyStatusAndSelectDB(List<MailboxDatabaseWithLocationInfo> databases, string domainController, LogMessageDelegate logger, ScopeSet scopeSet)
		{
			Guid[] array = new Guid[1];
			MdbStatus[] array2 = new MdbStatus[1];
			Dictionary<string, ExRpcAdmin> dictionary = new Dictionary<string, ExRpcAdmin>(100);
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>(100);
			ITopologyConfigurationSession topologyConfigurationSession = PhysicalResourceLoadBalancing.CreateGlobalConfigSession(domainController);
			IConfigurationSession configurationSession = null;
			if (scopeSet != null && !PhysicalResourceLoadBalancing.IsDatacenter)
			{
				configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromCustomScopeSet(scopeSet, topologyConfigurationSession.SessionSettings.RootOrgId, topologyConfigurationSession.SessionSettings.CurrentOrganizationId, topologyConfigurationSession.SessionSettings.ExecutingUserOrganizationId, true), 801, "VerifyStatusAndSelectDB", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\Provisioning\\LoadBalancing\\PhysicalResourceLoadBalancing.cs");
			}
			try
			{
				Random random = new Random();
				for (int i = databases.Count - 1; i >= 0; i--)
				{
					if (i != 0)
					{
						int index = random.Next(i + 1);
						MailboxDatabaseWithLocationInfo value = databases[index];
						databases[index] = databases[i];
						databases[i] = value;
					}
					MailboxDatabase mailboxDatabase = databases[i].MailboxDatabase;
					string text = string.Empty;
					try
					{
						if (configurationSession != null)
						{
							PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbCheckingDatabaseIsAllowedOnScope(mailboxDatabase.Id.ToDNString()), logger);
							ADScopeException ex;
							if (!configurationSession.TryVerifyIsWithinScopes(mailboxDatabase, false, out ex))
							{
								PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbDatabaseNotInUserScope(mailboxDatabase.Id.ToDNString(), ex.ToString()), logger);
								goto IL_2C6;
							}
						}
						DatabaseLocationInfo databaseLocationInfo = databases[i].DatabaseLocationInfo;
						text = databaseLocationInfo.ServerFqdn;
						ADObjectId serverSite = databaseLocationInfo.ServerSite;
						PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbDatabaseAndServerTry(mailboxDatabase.Id.ToDNString(), text ?? "null"), logger);
						if (!string.IsNullOrEmpty(text) && !dictionary2.ContainsKey(text))
						{
							ExRpcAdmin exRpcAdmin;
							if (dictionary.ContainsKey(text))
							{
								exRpcAdmin = dictionary[text];
								PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbExRpcAdminExists, logger);
							}
							else
							{
								exRpcAdmin = ExRpcAdmin.Create("Client=Management", text, null, null, null);
								dictionary.Add(text, exRpcAdmin);
								PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbCreateNewExRpcAdmin, logger);
							}
							array[0] = mailboxDatabase.Id.ObjectGuid;
							array2 = exRpcAdmin.ListMdbStatus(array);
							PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbTryRetrieveDatabaseStatus, logger);
							if ((array2[0].Status & MdbStatusFlags.Online) != MdbStatusFlags.Offline)
							{
								PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbDatabaseFound, logger);
								return new MailboxDatabaseWithLocationInfo(mailboxDatabase, databaseLocationInfo);
							}
							PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbDatabaseIsNotOnline((int)array2[0].Status), logger);
						}
						else
						{
							PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbServerDownSoMarkDatabaseDown, logger);
						}
					}
					catch (DatabaseNotFoundException ex2)
					{
						PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbDatabaseNotFoundException(ex2.Message), logger);
					}
					catch (ObjectNotFoundException ex3)
					{
						PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbNoServerForDatabaseException(ex3.Message), logger);
					}
					catch (MapiExceptionNetworkError mapiExceptionNetworkError)
					{
						PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbNetworkError(mapiExceptionNetworkError.Message), logger);
						dictionary2.Add(text, text);
					}
					catch (MapiPermanentException ex4)
					{
						PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbPermanentException(ex4.Message), logger);
						dictionary2.Add(text, text);
					}
					catch (MapiRetryableException ex5)
					{
						PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbRetryableException(ex5.Message), logger);
					}
					IL_2C6:;
				}
			}
			finally
			{
				foreach (string text2 in dictionary.Keys)
				{
					PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbDisposeExRpcAdmin(text2), logger);
					dictionary[text2].Dispose();
				}
			}
			PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbNoAvailableDatabase, logger);
			return null;
		}

		private static ITopologyConfigurationSession CreateGlobalConfigSession(string domainController)
		{
			bool flag = TaskHelper.ShouldPassDomainControllerToSession(domainController, ADSessionSettings.FromRootOrgScopeSet());
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(flag ? domainController : null, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 972, "CreateGlobalConfigSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\Provisioning\\LoadBalancing\\PhysicalResourceLoadBalancing.cs");
		}

		internal static bool IsDatabaseInLocalSite(DatabaseLocationInfo databaseLocationInfo, LogMessageDelegate logger)
		{
			return PhysicalResourceLoadBalancing.IsDatabaseInLocalSite(databaseLocationInfo.ServerSite, databaseLocationInfo.ServerFqdn, databaseLocationInfo.DatabaseName, logger);
		}

		internal static bool IsDatabaseInLocalSite(ADObjectId siteId, string serverFqdn, string databaseName, LogMessageDelegate logger)
		{
			if (PhysicalResourceLoadBalancing.localSiteName == null)
			{
				PhysicalResourceLoadBalancing.localSiteName = NativeHelpers.GetSiteName(false);
			}
			if (siteId != null && !string.IsNullOrEmpty(siteId.Name) && !string.IsNullOrEmpty(PhysicalResourceLoadBalancing.localSiteName))
			{
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbIsDatabaseLocal(databaseName, serverFqdn, PhysicalResourceLoadBalancing.localSiteName), logger);
				return string.Equals(PhysicalResourceLoadBalancing.localSiteName, siteId.Name, StringComparison.OrdinalIgnoreCase);
			}
			PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbIsLocalSiteNotEnoughInformation(databaseName, (siteId != null) ? siteId.Name : "", PhysicalResourceLoadBalancing.localSiteName), logger);
			return false;
		}

		private static void LogVerbose(LocalizedString lString, LogMessageDelegate logger)
		{
			if (logger != null)
			{
				logger(lString);
			}
		}

		private static List<MailboxDatabase> GetDatabasesForProvisioningCached(ITopologyConfigurationSession configSession, bool localSiteDatabasesOnly, LogMessageDelegate logger)
		{
			List<MailboxDatabase> result;
			using (new MonitoredScope("ProvisioningLayerLatency", "PhysicalResourceLoadBalancing.GetDatabasesForProvisioningCached", LoggerHelper.CmdletPerfMonitors))
			{
				Guid key = localSiteDatabasesOnly ? CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnLocalSite : CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnAllSites;
				List<MailboxDatabase> list = ProvisioningCache.Instance.TryAddAndGetGlobalData<List<MailboxDatabase>>(key, () => PhysicalResourceLoadBalancing.GetDatabasesForProvisioning(configSession, localSiteDatabasesOnly, logger));
				if (list.Count == 0)
				{
					ProvisioningCache.Instance.RemoveGlobalData(key);
				}
				result = new List<MailboxDatabase>(list);
			}
			return result;
		}

		private static List<MailboxDatabase> GetDatabasesForProvisioning(ITopologyConfigurationSession configSession, bool localSiteDatabasesOnly, LogMessageDelegate logger)
		{
			Server[] mailboxServers = PhysicalResourceLoadBalancing.GetMailboxServers(configSession, localSiteDatabasesOnly, logger);
			List<MailboxDatabase> result = new List<MailboxDatabase>();
			if (mailboxServers != null && mailboxServers.Length > 0)
			{
				result = PhysicalResourceLoadBalancing.GetDatabasesOnMailboxServers(configSession, mailboxServers, logger);
			}
			return result;
		}

		private static List<MailboxDatabase> GetDatabasesOnMailboxServers(ITopologyConfigurationSession configSession, Server[] mailboxServers, LogMessageDelegate logger)
		{
			Dictionary<string, MailboxDatabase> allEnabledDatabases = PhysicalResourceLoadBalancing.GetAllEnabledDatabases(configSession, logger);
			List<MailboxDatabase> list = new List<MailboxDatabase>();
			if (allEnabledDatabases.Count > 0)
			{
				HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (Server server in mailboxServers)
				{
					hashSet.Add(server.Name);
				}
				QueryFilter filter = new OrFilter(new QueryFilter[]
				{
					new NotFilter(new ExistsFilter(DatabaseCopySchema.ParentObjectClass)),
					new TextFilter(DatabaseCopySchema.ParentObjectClass, MailboxDatabase.MostDerivedClass, MatchOptions.FullString, MatchFlags.IgnoreCase)
				});
				ADObjectId rootId = ProvisioningCache.Instance.TryAddAndGetGlobalData<ADObjectId>(CannedProvisioningCacheKeys.DatabaseContainerId, new ProvisioningCache.CacheObjectGetterDelegate(configSession.GetDatabasesContainerId));
				PhysicalResourceLoadBalancing.LogVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(configSession, typeof(DatabaseCopy), filter, rootId, false), logger);
				ADPagedReader<DatabaseCopy> adpagedReader = configSession.FindPaged<DatabaseCopy>(rootId, QueryScope.SubTree, filter, null, 0);
				int num = 0;
				foreach (DatabaseCopy databaseCopy in adpagedReader)
				{
					MailboxDatabase mailboxDatabase;
					if (databaseCopy.IsValid && allEnabledDatabases.TryGetValue(databaseCopy.DatabaseName, out mailboxDatabase) && !string.IsNullOrEmpty(mailboxDatabase.ServerName) && hashSet.Contains(mailboxDatabase.ServerName))
					{
						allEnabledDatabases.Remove(databaseCopy.DatabaseName);
						list.Add(mailboxDatabase);
					}
					num++;
				}
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbGeneralTrace(string.Format("Retrieved '{0}' mailbox database copies. '{1}' databases have copies on selected mailbox servers.", num, list.Count)), logger);
			}
			if (list.Count <= 0)
			{
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbNoDatabaseFoundInAD, logger);
			}
			return list;
		}

		private static Dictionary<string, MailboxDatabase> GetAllEnabledDatabases(ITopologyConfigurationSession configSession, LogMessageDelegate logger)
		{
			ADObjectId adobjectId = ProvisioningCache.Instance.TryAddAndGetGlobalData<ADObjectId>(CannedProvisioningCacheKeys.DatabaseContainerId, new ProvisioningCache.CacheObjectGetterDelegate(configSession.GetDatabasesContainerId));
			PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbDatabaseContainer(adobjectId.ToDNString()), logger);
			List<QueryFilter> list = new List<QueryFilter>
			{
				new ComparisonFilter(ComparisonOperator.Equal, MailboxDatabaseSchema.IsExcludedFromProvisioning, false),
				new ComparisonFilter(ComparisonOperator.Equal, MailboxDatabaseSchema.IsSuspendedFromProvisioning, false),
				new ComparisonFilter(ComparisonOperator.Equal, DatabaseSchema.Recovery, false)
			};
			QueryFilter filter = QueryFilter.AndTogether(list.ToArray());
			PhysicalResourceLoadBalancing.LogVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(configSession, typeof(MailboxDatabase), filter, adobjectId, false), logger);
			ADPagedReader<MailboxDatabase> adpagedReader = configSession.FindPaged<MailboxDatabase>(adobjectId, QueryScope.OneLevel, filter, null, 0);
			Dictionary<string, MailboxDatabase> dictionary = new Dictionary<string, MailboxDatabase>();
			foreach (MailboxDatabase mailboxDatabase in adpagedReader)
			{
				dictionary[mailboxDatabase.Name] = mailboxDatabase;
			}
			PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbGeneralTrace(string.Format("Retrieved {0} mailbox databases enabled for provisioning in the forest.", dictionary.Count)), logger);
			return dictionary;
		}

		private static Server[] GetMailboxServers(ITopologyConfigurationSession configSession, bool localSiteOnly, LogMessageDelegate logger)
		{
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				new BitMaskAndFilter(ServerSchema.CurrentServerRole, 2UL),
				new ComparisonFilter(ComparisonOperator.NotEqual, ActiveDirectoryServerSchema.IsOutOfService, true),
				new ExistsFilter(ActiveDirectoryServerSchema.HostedDatabaseCopies)
			});
			QueryFilter filter;
			if (PhysicalResourceLoadBalancing.IsDatacenter)
			{
				if (localSiteOnly)
				{
					ADSite localSite = configSession.GetLocalSite();
					queryFilter = QueryFilter.AndTogether(new QueryFilter[]
					{
						queryFilter,
						new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, localSite.Id)
					});
				}
				QueryFilter mailboxServerVersionFilter = PhysicalResourceLoadBalancing.GetMailboxServerVersionFilter();
				filter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					mailboxServerVersionFilter
				});
			}
			else
			{
				filter = queryFilter;
			}
			PhysicalResourceLoadBalancing.LogVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(configSession, typeof(Server), filter, null, false), logger);
			Server[] array = configSession.Find<Server>(null, QueryScope.SubTree, filter, null, 0);
			if (array == null || array.Length == 0)
			{
				PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbNoServerForDatabaseException("Can not find any available mailbox server."), logger);
				throw new NoServersForDatabaseException("Can not find any mailbox server in local site");
			}
			PhysicalResourceLoadBalancing.LogVerbose(Strings.VerboseLbGeneralTrace(string.Format("Retrieved {0} mailbox servers.", array.Length)), logger);
			return array;
		}

		private static QueryFilter GetMailboxServerVersionFilter()
		{
			int num;
			int num2;
			if (OrganizationSchema.OrgConfigurationVersion >= 15000)
			{
				num = PhysicalResourceLoadBalancing.VersionE15;
				num2 = int.MaxValue;
			}
			else if (OrganizationSchema.OrgConfigurationVersion >= 14000)
			{
				num = PhysicalResourceLoadBalancing.VersionR6;
				num2 = PhysicalResourceLoadBalancing.VersionE15;
			}
			else
			{
				if (OrganizationSchema.OrgConfigurationVersion < 13000)
				{
					throw new ArgumentException(string.Format("Not supported OrganizationSchema ObjectVersion {0}", OrganizationSchema.ObjectVersion));
				}
				num = PhysicalResourceLoadBalancing.VersionR5;
				num2 = PhysicalResourceLoadBalancing.VersionR6;
			}
			return new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, num),
				new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, num2)
			});
		}

		private static string localSiteName = null;

		private static readonly bool IsDatacenter = !Datacenter.ExchangeSku.Enterprise.Equals(Datacenter.GetExchangeSku());

		private static readonly int VersionR5 = new ServerVersion(14, 1, 225, 0).ToInt();

		private static readonly int VersionR6 = new ServerVersion(14, 16, 0, 0).ToInt();

		private static readonly int VersionE15 = new ServerVersion(15, 0, 0, 0).ToInt();

		private static readonly ServerVersion E15MinVersion = new ServerVersion(Server.E15MinVersion);

		private static readonly int MajorVersionE15 = 15;

		private static int MaxNumOfVdirs = 3;
	}
}
