using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ActiveManagerImplementation
	{
		public static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ActiveManagerClientTracer;
			}
		}

		internal static int TestHookHangGsfdForMilliseconds
		{
			get
			{
				return ActiveManagerImplementation.m_hangGsfdForMilliseconds;
			}
			set
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<int>(0L, "TestHook to hang GetServerForDatabase call is set to {0}", value);
				ActiveManagerImplementation.m_hangGsfdForMilliseconds = value;
			}
		}

		internal static DatabaseLocationInfo GetServerNameForDatabaseInternal(IADDatabase database, NetworkCredential networkCredential, IFindAdObject<IADDatabaseAvailabilityGroup> dagLookup, IFindMiniServer findMiniServer, ActiveManagerClientPerfmonInstance perfCounters, bool throwOnErrors, bool isService)
		{
			Guid guid = database.Guid;
			string serverFqdn = null;
			string lastMountedServerFqdn = null;
			bool isDatabaseHighlyAvailable = false;
			DateTime mountedTime = DateTime.MinValue;
			DatabaseLocationInfoResult requestResult = DatabaseLocationInfoResult.Unknown;
			ServerVersion serverVersion = null;
			if (ActiveManagerImplementation.TestHookHangGsfdForMilliseconds > 0)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<int>(0L, "TestHook is hanging GetServerNameForDatabaseInternal for {0} milliseconds", ActiveManagerImplementation.TestHookHangGsfdForMilliseconds);
				Thread.Sleep(ActiveManagerImplementation.TestHookHangGsfdForMilliseconds);
			}
			if (Interlocked.Increment(ref ActiveManagerImplementation.NumberOfConcurrentRPCthreads) <= 3000)
			{
				try
				{
					if (!ActiveManagerImplementation.IsActiveManagerRpcSupported(database))
					{
						ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string>(0L, "Database {0} doesn't support RPC's for ActiveManager", database.Name);
					}
					else
					{
						try
						{
							ActiveManagerImplementation.s_perfCounters.GetServerForDatabaseClientRpcCalls.Increment();
							AmDbStatusInfo2 amDbStatusInfo = AmRpcClientHelper.RpcchGetServerForDatabase(database, networkCredential, dagLookup, findMiniServer, perfCounters, isService, out serverVersion);
							isDatabaseHighlyAvailable = (amDbStatusInfo.IsHighlyAvailable != 0);
							serverFqdn = amDbStatusInfo.MasterServerFqdn;
							lastMountedServerFqdn = amDbStatusInfo.LastMountedServerFqdn;
							mountedTime = amDbStatusInfo.MountedTime;
							requestResult = DatabaseLocationInfoResult.Success;
						}
						catch (AmDatabaseNeverMountedException innerException)
						{
							ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string>(0L, "Database {0} was never mounted on any of the servers (defaulting to owner server)", string.Empty);
							if (throwOnErrors)
							{
								throw new ServerForDatabaseNotFoundException(database.Name, guid.ToString(), innerException);
							}
						}
						catch (AmServerException ex)
						{
							ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string>(0L, "Encountered AM Server exception: {0} (defaulting to owner server)", ex.Message);
							if (throwOnErrors)
							{
								throw new ServerForDatabaseNotFoundException(database.Name, guid.ToString(), ex);
							}
						}
						catch (AmServerTransientException ex2)
						{
							ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string>(0L, "Encountered AM Server exception: {0} (defaulting to owner server)", ex2.Message);
							if (throwOnErrors)
							{
								throw new ServerForDatabaseNotFoundException(database.Name, guid.ToString(), ex2);
							}
						}
					}
					goto IL_1CF;
				}
				finally
				{
					Interlocked.Decrement(ref ActiveManagerImplementation.NumberOfConcurrentRPCthreads);
				}
			}
			Interlocked.Decrement(ref ActiveManagerImplementation.NumberOfConcurrentRPCthreads);
			ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<int>(0L, "Maximum number ({0}) of concurrent RPC threads reached.", 3000);
			if (throwOnErrors)
			{
				throw new ServerForDatabaseNotFoundException(database.Name, guid.ToString(), new TooManyActiveManagerClientRPCsException(3000));
			}
			IL_1CF:
			return new DatabaseLocationInfo(serverFqdn, null, lastMountedServerFqdn, null, null, null, database.Name, false, database.Recovery, Guid.Empty, mountedTime, null, null, serverVersion, MailboxRelease.None, requestResult, isDatabaseHighlyAvailable);
		}

		internal static void GetServerInformationForDatabaseInternal(IADDatabase database, DatabaseLocationInfo minimalLocationInfo, IFindMiniServer findMiniServer)
		{
			Guid guid = database.Guid;
			string lastMountedServerFqdn = minimalLocationInfo.LastMountedServerFqdn;
			IADServer iadserver = null;
			ADObjectId adobjectId = null;
			string serverFqdn = minimalLocationInfo.ServerFqdn;
			if (serverFqdn != null)
			{
				iadserver = findMiniServer.FindMiniServerByFqdn(serverFqdn);
			}
			else
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string>(0L, "GetServerInformationForDatabaseInternal({0}) is falling back to database.Server from AD, which may be stale.", database.Name);
				iadserver = findMiniServer.ReadMiniServerByObjectId(database.Server);
			}
			if (iadserver == null)
			{
				throw new UnableToFindServerForDatabaseException(database.Name, guid.ToString());
			}
			ADObjectId serverSiteFromMiniServer = ActiveManagerUtil.GetServerSiteFromMiniServer(iadserver);
			string fqdn = iadserver.Fqdn;
			if (iadserver.IsExchange2007OrLater && (serverSiteFromMiniServer == null || string.IsNullOrEmpty(fqdn) || (iadserver.IsExchange2007OrLater && !iadserver.IsMailboxServer)))
			{
				throw new UnableToFindServerForDatabaseException(database.Name, guid.ToString());
			}
			if (string.Equals(lastMountedServerFqdn, serverFqdn, StringComparison.OrdinalIgnoreCase))
			{
				adobjectId = iadserver.Id;
			}
			else
			{
				adobjectId = ActiveManagerImplementation.TryGetServerIdByFqdn(findMiniServer, lastMountedServerFqdn);
			}
			if (adobjectId == null)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<ADObjectId>(0L, "lastMountedServer was null. Setting to the current server ({0}).", iadserver.Id);
				adobjectId = iadserver.Id;
			}
			DatabaseLocationInfoResult requestResult = DatabaseLocationInfoResult.Success;
			IADServer iadserver2 = null;
			if (iadserver.Id.Equals(adobjectId))
			{
				iadserver2 = iadserver;
			}
			else
			{
				try
				{
					iadserver2 = findMiniServer.ReadMiniServerByObjectId(adobjectId);
				}
				catch (ADTransientException arg)
				{
					ActiveManagerImplementation.Tracer.TraceDebug<ADTransientException>(0L, "ReadMiniServer() threw an ADTransientException: {0}", arg);
				}
			}
			IADToplogyConfigurationSession adSession = findMiniServer.AdSession;
			IADSite iadsite = ActiveManagerImplementation.RetrieveLocalSite(adSession);
			if (iadsite == null)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug(0L, "GetServerForDatabase detected an Unknown state! adSession.GetLocalSite() is null.");
				requestResult = DatabaseLocationInfoResult.Unknown;
			}
			else if (iadserver2 == null)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<ADObjectId>(0L, "GetServerForDatabase detected an Unknown state! lastMountedServerId ({0}) could not be resolved.", adobjectId);
				requestResult = DatabaseLocationInfoResult.Unknown;
			}
			else if (!iadserver.Id.Equals(adobjectId))
			{
				ADObjectId serverSiteFromMiniServer2 = ActiveManagerUtil.GetServerSiteFromMiniServer(iadserver2);
				if (serverSiteFromMiniServer == serverSiteFromMiniServer2 || (serverSiteFromMiniServer != null && serverSiteFromMiniServer.Equals(serverSiteFromMiniServer2)))
				{
					ExTraceGlobals.ActiveManagerClientTracer.TraceDebug(0L, "GetServerForDatabase detected InTransitSameSite! masterServer (name={0},site={1}), lastMountedServer (name={2},site={3}.", new object[]
					{
						iadserver.Id,
						serverSiteFromMiniServer,
						adobjectId,
						serverSiteFromMiniServer2
					});
					requestResult = DatabaseLocationInfoResult.InTransitSameSite;
				}
				else
				{
					ExTraceGlobals.ActiveManagerClientTracer.TraceDebug(0L, "GetServerForDatabase detected InTransitCrossSite! masterServer (name={0},site={1}), lastMountedServer (name={2},site={3}.", new object[]
					{
						iadserver.Id,
						serverSiteFromMiniServer,
						adobjectId,
						serverSiteFromMiniServer2
					});
					requestResult = DatabaseLocationInfoResult.InTransitCrossSite;
				}
			}
			else if (!iadsite.Id.Equals(serverSiteFromMiniServer))
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<ADObjectId, ADObjectId>(0L, "GetServerForDatabase detected a SiteViolation! GetLocalSite().Id != serverSiteId ({0} != {1}).", iadsite.Id, serverSiteFromMiniServer);
				requestResult = DatabaseLocationInfoResult.SiteViolation;
			}
			string exchangeLegacyDN = iadserver.ExchangeLegacyDN;
			ServerVersion adminDisplayVersion = iadserver.AdminDisplayVersion;
			Guid objectGuid = iadserver.Id.ObjectGuid;
			string exchangeLegacyDN2 = iadserver2.ExchangeLegacyDN;
			iadserver2 = null;
			ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<Guid>(0L, "GetServerInformationForDatabaseInternal: Updating the location information for {0} in place.", guid);
			ADObjectId mailboxPublicFolderDatabase = database.MailboxPublicFolderDatabase;
			minimalLocationInfo.UpdateInPlace(fqdn, exchangeLegacyDN, minimalLocationInfo.LastMountedServerFqdn, exchangeLegacyDN2, database.ExchangeLegacyDN, database.RpcClientAccessServerLegacyDN, database.Name, database.IsPublicFolderDatabase, database.Recovery, (mailboxPublicFolderDatabase != null) ? mailboxPublicFolderDatabase.ObjectGuid : Guid.Empty, minimalLocationInfo.MountedTime, new Guid?(objectGuid), serverSiteFromMiniServer, adminDisplayVersion, iadserver.MailboxRelease, requestResult, minimalLocationInfo.IsDatabaseHighlyAvailable);
		}

		internal static void CalculatePreferredHomeServerInternal(ActiveManager activeManager, IADDatabase database, ITopologyConfigurationSession adSession, IFindAdObject<IADDatabaseAvailabilityGroup> dagLookup, IFindAdObject<IADClientAccessArray> findClientAccessArray, IFindMiniClientAccessServerOrArray findMiniClientAccessServer, out LegacyDN preferredRpcClientAccessServerLegacyDN, out ADObjectId preferredServerSite)
		{
			Util.ThrowOnNullArgument(activeManager, "activeManager");
			preferredRpcClientAccessServerLegacyDN = LegacyDN.Parse(database.RpcClientAccessServerLegacyDN);
			ADObjectId masterServerOrAvailabilityGroup = database.MasterServerOrAvailabilityGroup;
			IADDatabaseAvailabilityGroup iaddatabaseAvailabilityGroup = (masterServerOrAvailabilityGroup != null) ? dagLookup.ReadAdObjectByObjectId(masterServerOrAvailabilityGroup) : null;
			if (iaddatabaseAvailabilityGroup == null || !iaddatabaseAvailabilityGroup.AllowCrossSiteRpcClientAccess)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<ADObjectId, ADObjectId>((long)activeManager.GetHashCode(), "CalculatePreferredHomeServerInternal: Cross-site is not allowed. Database = '{0}', DAG = '{1}'", database.Id, masterServerOrAvailabilityGroup);
				DatabaseLocationInfo databaseLocationInfo = null;
				preferredServerSite = null;
				try
				{
					databaseLocationInfo = activeManager.GetServerForDatabase(database.Guid, GetServerForDatabaseFlags.IgnoreAdSiteBoundary, NullPerformanceDataLogger.Instance);
					if (databaseLocationInfo != null)
					{
						preferredServerSite = databaseLocationInfo.ServerSite;
					}
				}
				catch (DatabaseNotFoundException arg)
				{
					ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<ADObjectId, DatabaseNotFoundException>((long)activeManager.GetHashCode(), "CalculatePreferredHomeServerInternal: The database '{0}' does not exist. Exception = {1}", database.Id, arg);
					preferredServerSite = null;
				}
				catch (ObjectNotFoundException arg2)
				{
					ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<ADObjectId, ObjectNotFoundException>((long)activeManager.GetHashCode(), "CalculatePreferredHomeServerInternal: Server hosting the database {0} cannot be found. Exception = {1}", database.Id, arg2);
					preferredServerSite = null;
				}
				if (preferredServerSite == null)
				{
					preferredRpcClientAccessServerLegacyDN = null;
					return;
				}
				IADMiniClientAccessServerOrArray iadminiClientAccessServerOrArray = findMiniClientAccessServer.FindMiniClientAccessServerOrArrayByLegdn(databaseLocationInfo.RpcClientAccessServerLegacyDN);
				if (iadminiClientAccessServerOrArray != null && iadminiClientAccessServerOrArray.ServerSite != null && iadminiClientAccessServerOrArray.ServerSite.Equals(preferredServerSite))
				{
					preferredRpcClientAccessServerLegacyDN = LegacyDN.Parse(databaseLocationInfo.RpcClientAccessServerLegacyDN);
					return;
				}
				preferredRpcClientAccessServerLegacyDN = ActiveManagerImplementation.FindClientAccessArrayOrServerFromSite(preferredServerSite, database.HostServerForPreference1, findClientAccessArray, findMiniClientAccessServer, AdObjectLookupFlags.None);
				return;
			}
			else
			{
				IADMiniClientAccessServerOrArray iadminiClientAccessServerOrArray2 = findMiniClientAccessServer.FindMiniClientAccessServerOrArrayByLegdn(preferredRpcClientAccessServerLegacyDN.ToString());
				ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<LegacyDN, string>((long)activeManager.GetHashCode(), "CalculatePreferredHomeServerInternal. preferredRpcClientAccessServerLegacyDN = {0}, preferredMiniServer = {1}.", preferredRpcClientAccessServerLegacyDN, (iadminiClientAccessServerOrArray2 == null) ? "<null>" : iadminiClientAccessServerOrArray2.Fqdn);
				if (iadminiClientAccessServerOrArray2 == null)
				{
					preferredServerSite = null;
					preferredRpcClientAccessServerLegacyDN = null;
					return;
				}
				preferredRpcClientAccessServerLegacyDN = LegacyDN.Parse(iadminiClientAccessServerOrArray2.ExchangeLegacyDN);
				preferredServerSite = iadminiClientAccessServerOrArray2.ServerSite;
				return;
			}
		}

		public static LegacyDN FindClientAccessArrayOrServerFromSite(ADObjectId site, ADObjectId preferredServer, IFindAdObject<IADClientAccessArray> findClientAccessArray, IFindMiniClientAccessServerOrArray findMiniClientAccessServer, AdObjectLookupFlags adObjectLookupFlags)
		{
			if (site == null)
			{
				return null;
			}
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, site);
			IADClientAccessArray iadclientAccessArray = findClientAccessArray.FindAdObjectByQueryEx(QueryFilter.AndTogether(new QueryFilter[]
			{
				queryFilter,
				ClientAccessArray.PriorTo15ExchangeObjectVersionFilter
			}), adObjectLookupFlags);
			if (iadclientAccessArray != null)
			{
				return LegacyDN.Parse(iadclientAccessArray.ExchangeLegacyDN);
			}
			IADMiniClientAccessServerOrArray iadminiClientAccessServerOrArray = findMiniClientAccessServer.FindMiniClientAccessServerOrArrayWithClientAccess(site, preferredServer);
			if (iadminiClientAccessServerOrArray != null)
			{
				return LegacyDN.Parse(iadminiClientAccessServerOrArray.ExchangeLegacyDN);
			}
			return null;
		}

		internal static ADObjectId TryGetServerIdByFqdn(IFindMiniServer findMiniServer, string fqdn)
		{
			if (!string.IsNullOrEmpty(fqdn))
			{
				IADServer iadserver = findMiniServer.FindMiniServerByFqdn(fqdn);
				if (iadserver != null)
				{
					return iadserver.Id;
				}
			}
			return null;
		}

		private static IADSite RetrieveLocalSite(IADToplogyConfigurationSession session)
		{
			if (ActiveManagerImplementation.cachedLocalSite != null)
			{
				if (!(DateTime.UtcNow > ActiveManagerImplementation.localSiteExpiration))
				{
					goto IL_77;
				}
			}
			try
			{
				ActiveManagerImplementation.Tracer.TraceDebug(0L, "RetrieveLocalSite: either the local site is null, or it's time to refresh the value.");
				ActiveManagerImplementation.cachedLocalSite = session.GetLocalSite();
				ActiveManagerImplementation.localSiteExpiration = DateTime.UtcNow.Add(ActiveManagerImplementation.c_timeSpanSiteExpiration);
			}
			catch (ADTransientException arg)
			{
				ActiveManagerImplementation.Tracer.TraceError<ADTransientException>(0L, "GetLocalSite() threw an ADTransientException: {0}", arg);
			}
			catch (CannotGetSiteInfoException arg2)
			{
				ActiveManagerImplementation.Tracer.TraceError<CannotGetSiteInfoException>(0L, "GetLocalSite() threw a CannotGetSiteInfoException: {0}", arg2);
			}
			IL_77:
			return ActiveManagerImplementation.cachedLocalSite;
		}

		private static bool IsActiveManagerRpcSupported(IADDatabase database)
		{
			return database.IsExchange2009OrLater;
		}

		internal static ActiveManagerClientPerfmonInstance GetPerfCounters()
		{
			Process currentProcess = Process.GetCurrentProcess();
			string instanceName = string.Format("{0} - {1}", currentProcess.ProcessName, currentProcess.Id);
			return ActiveManagerClientPerfmon.GetInstance(instanceName);
		}

		internal const int MaximumMsecInCacheUpdateThread = 120000;

		private const int MaximumConcurrentRpcThreads = 3000;

		private static int m_hangGsfdForMilliseconds = 0;

		internal static int NumberOfConcurrentRPCthreads;

		private static DateTime localSiteExpiration = DateTime.MinValue;

		private static IADSite cachedLocalSite;

		private static readonly TimeSpan c_timeSpanSiteExpiration = new TimeSpan(0, 15, 0);

		private static readonly ActiveManagerClientPerfmonInstance s_perfCounters = ActiveManagerImplementation.GetPerfCounters();
	}
}
