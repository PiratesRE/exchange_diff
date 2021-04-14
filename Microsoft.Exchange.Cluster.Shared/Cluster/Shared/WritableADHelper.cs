using System;
using System.Linq;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal class WritableADHelper : IWritableAD
	{
		bool IWritableAD.SetDatabaseLegacyDnAndOwningServer(Guid mdbGuid, AmServerName lastMountedServerName, AmServerName masterServerName, bool isForceUpdate)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 43, "SetDatabaseLegacyDnAndOwningServer", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Shared\\WritableADHelper.cs");
			Database database = topologyConfigurationSession.FindDatabaseByGuid<Database>(mdbGuid);
			if (database == null)
			{
				AmTrace.Error("Failed to find Db using dbGuid {0}", new object[]
				{
					mdbGuid
				});
				return false;
			}
			MiniServer miniServer = topologyConfigurationSession.FindMiniServerByName(masterServerName.NetbiosName, WritableADHelper.s_propertiesNeededFromServer);
			if (miniServer == null)
			{
				AmTrace.Error("Failed to find Server using {0}", new object[]
				{
					masterServerName
				});
				return false;
			}
			if (isForceUpdate || !database.Server.Equals(miniServer.Id))
			{
				MiniServer sourceServer = null;
				if (!AmServerName.IsNullOrEmpty(lastMountedServerName))
				{
					sourceServer = topologyConfigurationSession.FindMiniServerByName(lastMountedServerName.NetbiosName, WritableADHelper.s_propertiesNeededFromServer);
				}
				AmTrace.Debug("SetDatabaseLegacyDnAndOwningServer. Database '{0}', legdn='{1}'. Setting owning server: '{2}'.", new object[]
				{
					database.Name,
					database.ExchangeLegacyDN,
					miniServer.Id
				});
				database.Server = miniServer.Id;
				WritableADHelper.SaveDatabasePropertiesOnMultipleServerSites(topologyConfigurationSession, database, sourceServer, miniServer, true);
				return true;
			}
			AmTrace.Debug("Skipped to update legacy dn and owning server for database '{0}' since they are up to date. (owningserver={1})", new object[]
			{
				database.Name,
				database.Server
			});
			return false;
		}

		void IWritableAD.ResetAllowFileRestoreDsFlag(Guid mdbGuid, AmServerName lastMountedServerName, AmServerName masterServerName)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 108, "ResetAllowFileRestoreDsFlag", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Shared\\WritableADHelper.cs");
			Database database = topologyConfigurationSession.FindDatabaseByGuid<Database>(mdbGuid);
			if (database == null)
			{
				AmTrace.Error("Failed to find Db using dbGuid {0}", new object[]
				{
					mdbGuid
				});
				return;
			}
			if (!database.AllowFileRestore)
			{
				AmTrace.Debug("Skipped to update legacy AllowFileRestore database '{0}'.)", new object[]
				{
					database.Name
				});
				return;
			}
			MiniServer miniServer = topologyConfigurationSession.FindMiniServerByName(masterServerName.NetbiosName, WritableADHelper.s_propertiesNeededFromServer);
			if (miniServer == null)
			{
				AmTrace.Error("Failed to find Server using {0}", new object[]
				{
					masterServerName
				});
				return;
			}
			MiniServer sourceServer = null;
			if (!AmServerName.IsNullOrEmpty(lastMountedServerName))
			{
				sourceServer = topologyConfigurationSession.FindMiniServerByName(lastMountedServerName.NetbiosName, WritableADHelper.s_propertiesNeededFromServer);
			}
			AmTrace.Debug("Reset AllowFileRestore. Database '{0}', legdn='{1}'. Setting owning server: '{2}'.", new object[]
			{
				database.Name,
				database.ExchangeLegacyDN,
				miniServer.Id
			});
			database.AllowFileRestore = false;
			WritableADHelper.SaveDatabasePropertiesOnMultipleServerSites(topologyConfigurationSession, database, sourceServer, miniServer, true);
		}

		private static void SaveDatabasePropertiesOnMultipleServerSites(ITopologyConfigurationSession rwSession, Database database, MiniServer sourceServer, MiniServer targetServer, bool isBestEffort)
		{
			Server server = rwSession.FindServerByName(Environment.MachineName);
			ADObjectId localServerSite = server.ServerSite;
			Exception ex = SharedHelper.RunADOperationEx(delegate(object param0, EventArgs param1)
			{
				rwSession.Save(database);
			});
			if (ex != null)
			{
				AmTrace.Error("Failed to update properties on the default local site (error={0})", new object[]
				{
					ex
				});
				ReplayCrimsonEvents.DatabasePropertyLocalSiteUpdateFailed.Log<string, Guid, string, string, string>(database.Name, database.Guid, targetServer.Name, localServerSite.DistinguishedName, ex.Message);
				if (!isBestEffort)
				{
					throw ex;
				}
			}
			ADObjectId adobjectId = (sourceServer != null) ? sourceServer.ServerSite : null;
			ADObjectId adobjectId2 = (targetServer != null) ? targetServer.ServerSite : null;
			ADObjectId[] source = new ADObjectId[]
			{
				adobjectId,
				adobjectId2
			};
			ADObjectId[] sites = (from s in source
			where s != null && !s.Equals(localServerSite)
			select s).Distinct<ADObjectId>().ToArray<ADObjectId>();
			if (sites.Length > 0)
			{
				string[] array = (from s in sites
				select s.DistinguishedName).ToArray<string>();
				string[] dcsContacted = null;
				ex = SharedHelper.RunADOperationEx(delegate(object param0, EventArgs param1)
				{
					dcsContacted = rwSession.ReplicateSingleObject(database, sites);
				});
				if (ex != null)
				{
					AmTrace.Error("Failed to initiate replication for remote sites (error={0})", new object[]
					{
						ex
					});
				}
				for (int i = 0; i < sites.Length; i++)
				{
					if (dcsContacted == null || string.IsNullOrEmpty(dcsContacted[i]))
					{
						AmTrace.Error("Replication request for site '{0}' was not submitted to any DC", new object[]
						{
							array[i]
						});
					}
					else
					{
						AmTrace.Debug("Replication request for site '{0}' was submitted to DC '{1}'", new object[]
						{
							array[i],
							dcsContacted[i]
						});
					}
				}
				if (ex != null)
				{
					ReplayCrimsonEvents.DatabasePropertyRemoteSiteReplicationFailed.Log<string, Guid, string, string, string, string>(database.Name, database.Guid, targetServer.Name, string.Join(",", array), (dcsContacted != null) ? string.Join(",", dcsContacted) : string.Empty, ex.Message);
					if (!isBestEffort)
					{
						throw ex;
					}
				}
				else
				{
					ReplayCrimsonEvents.DatabasePropertyRemoteSiteReplicationSucceeded.Log<string, Guid, string, string, string>(database.Name, database.Guid, targetServer.Name, string.Join(",", array), (dcsContacted != null) ? string.Join(",", dcsContacted) : string.Empty);
				}
			}
		}

		private static readonly PropertyDefinition[] s_propertiesNeededFromServer = new PropertyDefinition[]
		{
			ServerSchema.ExchangeLegacyDN,
			ServerSchema.IsMailboxServer,
			ServerSchema.IsClientAccessServer,
			ServerSchema.ServerSite
		};
	}
}
