using System;
using System.Linq;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AmBestCopySelectionHelper
	{
		public static bool IsAutoActivationAllowed(IADServer sourceServer, IADServer targetServer, out LocalizedString error)
		{
			string name = sourceServer.Name;
			string name2 = targetServer.Name;
			error = LocalizedString.Empty;
			if (SharedHelper.StringIEquals(name, name2))
			{
				AmTrace.Debug("IsAutoActivationAllowed: Skipping check since source == target. TargetServer: {0}", new object[]
				{
					name2
				});
				return true;
			}
			switch (targetServer.DatabaseCopyAutoActivationPolicy)
			{
			case DatabaseCopyAutoActivationPolicyType.Unrestricted:
				return true;
			case DatabaseCopyAutoActivationPolicyType.IntrasiteOnly:
				if (!targetServer.ServerSite.Equals(sourceServer.ServerSite))
				{
					AmTrace.Debug("IsAutoActivationAllowed: Rejecting server '{0}' (Site={1}) because it is in a different site from source server '{2}' (Site={2}). ", new object[]
					{
						name2,
						targetServer.ServerSite.Name,
						name,
						sourceServer.ServerSite.Name
					});
					error = ReplayStrings.AmBcsTargetServerActivationIntraSite(targetServer.Fqdn, sourceServer.Fqdn, targetServer.ServerSite.Name, sourceServer.ServerSite.Name);
					return false;
				}
				return true;
			case DatabaseCopyAutoActivationPolicyType.Blocked:
				AmTrace.Debug("IsAutoActivationAllowed: Rejecting server '{0}' because it is activation policy Blocked.", new object[]
				{
					name2
				});
				error = ReplayStrings.AmBcsTargetServerActivationBlocked(targetServer.Fqdn);
				return false;
			default:
				DiagCore.RetailAssert(false, "Unhandled case for DatabaseCopyAutoActivationPolicyType: {0}", new object[]
				{
					targetServer.DatabaseCopyAutoActivationPolicy
				});
				return false;
			}
		}

		public static bool HasDatabaseBeenMounted(Guid dbGuid, AmConfig amConfig)
		{
			AmServerName amServerName;
			AmServerName amServerName2;
			return AmBestCopySelectionHelper.HasDatabaseBeenMounted(dbGuid, amConfig, out amServerName, out amServerName2);
		}

		public static bool HasDatabaseBeenMounted(Guid dbGuid, AmConfig amConfig, out AmServerName masterServerName, out AmServerName lastMountedServerName)
		{
			AmDbStateInfo amDbStateInfo = amConfig.DbState.Read(dbGuid);
			masterServerName = amDbStateInfo.ActiveServer;
			lastMountedServerName = amDbStateInfo.LastMountedServer;
			return amDbStateInfo.IsActiveServerValid;
		}

		public static IADDatabaseCopy[] GetDatabaseCopies(Guid dbGuid, ref IADDatabase database)
		{
			Exception ex = null;
			IADDatabaseCopy[] array = null;
			LocalizedString value = LocalizedString.Empty;
			try
			{
				if (database == null)
				{
					database = AmHelper.FindDatabaseByGuid(dbGuid);
				}
				array = database.DatabaseCopies;
				if (array == null || array.Length == 0)
				{
					value = ReplayStrings.AmBcsDatabaseHasNoCopies;
					AmTrace.Error("GetDatabaseCopies(): Database has 0 copies in the AD.", new object[0]);
				}
			}
			catch (AmDatabaseNotFoundException ex2)
			{
				AmTrace.Error("GetDatabaseCopies(): AmDatabaseNotFoundException occurred (dbGuid={0}, exception={1})", new object[]
				{
					dbGuid,
					ex2
				});
				ex = ex2;
			}
			catch (ADTransientException ex3)
			{
				AmTrace.Error("GetDatabaseCopies(): ADTransientException occurred for GetDatabaseCopies() (database={0}, dbGuid={1}, exception={2})", new object[]
				{
					database.Name,
					dbGuid,
					ex3
				});
				ex = ex3;
			}
			catch (ADExternalException ex4)
			{
				AmTrace.Error("GetDatabaseCopies(): ADExternalException occurred for GetDatabaseCopies() (database={0}, dbGuid={1}, exception={2})", new object[]
				{
					database.Name,
					dbGuid,
					ex4
				});
				ex = ex4;
			}
			catch (ADOperationException ex5)
			{
				AmTrace.Error("GetDatabaseCopies(): ADOperationException occurred for GetDatabaseCopies() (database={0}, dbGuid={1}, exception={2})", new object[]
				{
					database.Name,
					dbGuid,
					ex5
				});
				ex = ex5;
			}
			if (array == null || array.Length == 0)
			{
				throw new AmBcsFailedToQueryCopiesException((database == null) ? dbGuid.ToString() : database.Name, (ex != null) ? ex.Message : value, ex);
			}
			return array;
		}

		public static IADServer GetMiniServer(AmServerName serverName, out Exception exception)
		{
			Exception ex = null;
			exception = null;
			IADServer iadserver = Dependencies.ReplayAdObjectLookup.MiniServerLookup.FindMiniServerByShortNameEx(serverName.NetbiosName, out ex);
			if (iadserver == null)
			{
				exception = new FailedToFindServerException(serverName.Fqdn, ex);
				AmTrace.Error("GetMiniServer got back 'null' from FindMiniServerByName for server '{0}'. Returning Exception: {1}", new object[]
				{
					serverName,
					exception
				});
				ReplayCrimsonEvents.ADObjectLookupError.LogPeriodic<string, Exception>(serverName.NetbiosName, DiagCore.DefaultEventSuppressionInterval, exception.Message, ex);
			}
			return iadserver;
		}

		public static bool IsActivationDisabled(AmServerName srv)
		{
			Exception ex;
			IADServer miniServer = AmBestCopySelectionHelper.GetMiniServer(srv, out ex);
			return AmBestCopySelectionHelper.IsActivationDisabled(miniServer);
		}

		public static bool IsActivationDisabled(IADServer miniServer)
		{
			return miniServer != null && miniServer.DatabaseCopyActivationDisabledAndMoveNow && !RegistryParameters.DisableActivationDisabled;
		}

		public static IADDatabaseAvailabilityGroup GetLocalServerDatabaseAvailabilityGroup(out string errorMessage)
		{
			IADDatabaseAvailabilityGroup iaddatabaseAvailabilityGroup = null;
			Exception ex = null;
			errorMessage = string.Empty;
			try
			{
				IADToplogyConfigurationSession iadtoplogyConfigurationSession = ADSessionFactory.CreateIgnoreInvalidRootOrgSession(true);
				IADServer iadserver = iadtoplogyConfigurationSession.FindServerByName(Dependencies.ManagementClassHelper.LocalMachineName);
				if (iadserver != null)
				{
					iaddatabaseAvailabilityGroup = iadtoplogyConfigurationSession.FindDagByServer(iadserver);
					if (iaddatabaseAvailabilityGroup == null)
					{
						ex = new CouldNotFindDagObjectForServer(iadserver.Name);
					}
				}
				else
				{
					ex = new CouldNotFindServerObject(Environment.MachineName);
				}
			}
			catch (ADTransientException ex2)
			{
				AmTrace.Error("GetLocalServerDatabaseAvailabilityGroup got exception: {0}", new object[]
				{
					ex2
				});
				ex = ex2;
			}
			catch (ADExternalException ex3)
			{
				AmTrace.Error("GetLocalServerDatabaseAvailabilityGroup got exception: {0}", new object[]
				{
					ex3
				});
				ex = ex3;
			}
			catch (ADOperationException ex4)
			{
				AmTrace.Error("GetLocalServerDatabaseAvailabilityGroup got exception: {0}", new object[]
				{
					ex4
				});
				ex = ex4;
			}
			if (iaddatabaseAvailabilityGroup == null)
			{
				errorMessage = ex.Message;
				if (string.IsNullOrEmpty(errorMessage))
				{
					errorMessage = ex.ToString();
				}
			}
			return iaddatabaseAvailabilityGroup;
		}

		public static bool IsServerInDacAndStopped(IADDatabaseAvailabilityGroup dag, AmServerName serverName)
		{
			bool result = false;
			if (dag.DatacenterActivationMode == DatacenterActivationModeOption.DagOnly)
			{
				AmTrace.Debug("Database availability group {0} is in DAC mode", new object[]
				{
					dag.Name
				});
				MultiValuedProperty<string> stoppedMailboxServers = dag.StoppedMailboxServers;
				if (stoppedMailboxServers != null && stoppedMailboxServers.Contains(serverName.Fqdn, StringComparer.OrdinalIgnoreCase))
				{
					result = true;
				}
			}
			return result;
		}

		public static bool UpdateActiveIfMaxActivesNotExceeded(Guid databaseGuid, AmServerName serverName, Func<IADServer, int?> getMaxActiveDbsLimit, out int? maxActiveDatabases)
		{
			bool result = true;
			int? num = null;
			IADServer iadserver = Dependencies.ReplayAdObjectLookup.ServerLookup.FindServerByFqdn(serverName.Fqdn);
			if (iadserver != null)
			{
				num = getMaxActiveDbsLimit(iadserver);
				AmDatabaseStateTracker databaseStateTracker = AmSystemManager.Instance.DatabaseStateTracker;
				if (databaseStateTracker != null)
				{
					result = databaseStateTracker.UpdateActiveIfMaxActivesNotExceeded(databaseGuid, serverName, num);
				}
			}
			else
			{
				AmTrace.Error("Failed to find server {0}", new object[]
				{
					serverName
				});
				FailedToFindServerException ex = new FailedToFindServerException(serverName.Fqdn);
				ReplayCrimsonEvents.ADObjectLookupError.LogPeriodic<string, FailedToFindServerException>(serverName.NetbiosName, DiagCore.DefaultEventSuppressionInterval, ex.Message, ex);
			}
			maxActiveDatabases = num;
			return result;
		}

		public static bool IsMaxActivesUnderHighestLimit(AmServerName serverName, out int? maxActiveDatabases)
		{
			return AmBestCopySelectionHelper.IsMaxActivesUnderThreshold(serverName, (IADServer server) => server.MaximumActiveDatabases, out maxActiveDatabases);
		}

		public static bool IsMaxActivesUnderPreferredLimit(AmServerName serverName, out int? maxActiveDatabases)
		{
			return AmBestCopySelectionHelper.IsMaxActivesUnderThreshold(serverName, (IADServer server) => server.MaximumPreferredActiveDatabases, out maxActiveDatabases);
		}

		private static bool IsMaxActivesUnderThreshold(AmServerName serverName, Func<IADServer, int?> getMaxActiveDbsLimit, out int? maxActiveDatabases)
		{
			bool result = true;
			int? num = null;
			IADServer iadserver = Dependencies.ReplayAdObjectLookup.ServerLookup.FindServerByFqdn(serverName.Fqdn);
			if (iadserver != null)
			{
				num = getMaxActiveDbsLimit(iadserver);
				AmDatabaseStateTracker databaseStateTracker = AmSystemManager.Instance.DatabaseStateTracker;
				if (databaseStateTracker != null && databaseStateTracker.IsMaxActivesExceeded(serverName, num))
				{
					result = false;
				}
			}
			else
			{
				AmTrace.Error("Failed to find server {0}", new object[]
				{
					serverName
				});
				FailedToFindServerException ex = new FailedToFindServerException(serverName.Fqdn);
				ReplayCrimsonEvents.ADObjectLookupError.LogPeriodic<string, FailedToFindServerException>(serverName.NetbiosName, DiagCore.DefaultEventSuppressionInterval, ex.Message, ex);
			}
			maxActiveDatabases = num;
			return result;
		}
	}
}
