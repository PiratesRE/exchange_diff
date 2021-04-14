using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.Cluster;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class RemoteReplayConfiguration : ReplayConfiguration
	{
		public static RemoteReplayConfiguration TaskGetReplayConfig(IADDatabaseAvailabilityGroup dag, IADDatabase db, IADServer server)
		{
			DatabaseLocationInfo databaseLocationInfo;
			bool flag = RemoteReplayConfiguration.IsServerRcrSource(db, server, out databaseLocationInfo);
			if (flag)
			{
				return new RemoteReplayConfiguration(dag, db, server, server.Fqdn, LockType.Remote, ReplayConfigType.RemoteCopySource);
			}
			return new RemoteReplayConfiguration(dag, db, server, databaseLocationInfo.ServerFqdn, LockType.Remote, ReplayConfigType.RemoteCopyTarget);
		}

		public static bool IsServerRcrSource(IADDatabase db, IADServer server, out DatabaseLocationInfo activeLocation)
		{
			ActiveManager noncachingActiveManagerInstance = ActiveManager.GetNoncachingActiveManagerInstance();
			activeLocation = noncachingActiveManagerInstance.GetServerForDatabase(db.Guid, GetServerForDatabaseFlags.BasicQuery);
			return Cluster.StringIEquals(activeLocation.ServerFqdn, server.Fqdn);
		}

		public static bool IsServerRcrSource(IADDatabase db, string serverName, ITopologyConfigurationSession adSession, out DatabaseLocationInfo activeLocation)
		{
			bool result;
			using (ActiveManager activeManager = ActiveManager.CreateCustomActiveManager(false, null, null, null, null, null, null, adSession, true))
			{
				activeLocation = activeManager.GetServerForDatabase(db.Guid, GetServerForDatabaseFlags.BasicQuery);
				result = Cluster.StringIEquals(new AmServerName(activeLocation.ServerFqdn).NetbiosName, serverName);
			}
			return result;
		}

		public static RemoteReplayConfiguration ServiceGetReplayConfig(IADDatabaseAvailabilityGroup dag, IADDatabase db, IADServer server, string activeFqdn, ReplayConfigType type)
		{
			return new RemoteReplayConfiguration(dag, db, server, activeFqdn, LockType.ReplayService, type);
		}

		public static bool IsServerValidRcrTarget(IADDatabase database, IADServer server, out int maxDB, string sourceDomain, bool fThrow)
		{
			maxDB = 0;
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (server.IsMailboxServer)
			{
				if (server.Edition == ServerEditionType.Enterprise || server.Edition == ServerEditionType.EnterpriseEvaluation)
				{
					maxDB = 100;
				}
				else
				{
					maxDB = 5;
				}
				IADDatabaseCopy[] databaseCopies = database.DatabaseCopies;
				int i = 0;
				while (i < databaseCopies.Length)
				{
					IADDatabaseCopy iaddatabaseCopy = databaseCopies[i];
					if (string.Equals(iaddatabaseCopy.HostServerName, server.Name))
					{
						if (fThrow)
						{
							throw new InvalidRcrConfigAlreadyHostsDb(server.Name, database.Name);
						}
						return false;
					}
					else
					{
						i++;
					}
				}
				return true;
			}
			if (fThrow)
			{
				throw new InvalidRcrConfigOnNonMailboxException(server.Name);
			}
			return false;
		}

		private RemoteReplayConfiguration(IADDatabaseAvailabilityGroup dag, IADDatabase database, IADServer server, string activeFqdn, LockType lockType, ReplayConfigType type)
		{
			try
			{
				if (database == null)
				{
					throw new NullDatabaseException();
				}
				if (server == null)
				{
					throw new ErrorNullServerFromDb(database.Name);
				}
				if (activeFqdn == null)
				{
					throw new ArgumentException("Caller must provide the active node");
				}
				IADDatabaseCopy databaseCopy = database.GetDatabaseCopy(server.Name);
				if (databaseCopy == null)
				{
					throw new NullDbCopyException();
				}
				this.m_server = server;
				this.m_database = database;
				this.m_targetNodeFqdn = server.Fqdn;
				this.m_sourceNodeFqdn = activeFqdn;
				this.m_type = type;
				this.m_autoDatabaseMountDial = this.m_server.AutoDatabaseMountDial;
				if (type == ReplayConfigType.RemoteCopyTarget)
				{
					this.m_replayState = ReplayState.GetReplayState(this.m_targetNodeFqdn, this.m_sourceNodeFqdn, lockType, this.Identity, this.Database.Name);
				}
				else
				{
					this.m_replayState = ReplayState.GetReplayState(this.m_sourceNodeFqdn, this.m_sourceNodeFqdn, lockType, this.Identity, this.Database.Name);
				}
				this.m_replayLagTime = databaseCopy.ReplayLagTime;
				this.m_truncationLagTime = databaseCopy.TruncationLagTime;
				this.m_activationPreference = databaseCopy.ActivationPreference;
				base.PopulatePropertiesFromDag(dag);
			}
			finally
			{
				this.BuildDebugString();
			}
		}

		public virtual AutoDatabaseMountDial AutoDatabaseMountDial
		{
			get
			{
				return this.m_autoDatabaseMountDial;
			}
		}

		public override EnhancedTimeSpan ReplayLagTime
		{
			get
			{
				return this.m_replayLagTime;
			}
		}

		public override EnhancedTimeSpan TruncationLagTime
		{
			get
			{
				return this.m_truncationLagTime;
			}
		}

		public ReplayConfiguration ConfigurationPathConflict(Dictionary<string, ReplayConfiguration> currentConfigurations, out string field)
		{
			field = string.Empty;
			foreach (ReplayConfiguration replayConfiguration in currentConfigurations.Values)
			{
				if (replayConfiguration.Type == ReplayConfigType.RemoteCopyTarget)
				{
					if (Cluster.StringIEquals(this.DestinationLogPath, replayConfiguration.DestinationLogPath))
					{
						field = "DestinationLogPath:" + replayConfiguration.DestinationLogPath;
						return replayConfiguration;
					}
					if (Cluster.StringIEquals(this.DestinationEdbPath, replayConfiguration.DestinationEdbPath))
					{
						field = "DestinationEdbPath:" + replayConfiguration.DestinationEdbPath;
						return replayConfiguration;
					}
					if (Cluster.StringIEquals(this.DestinationSystemPath, replayConfiguration.DestinationSystemPath))
					{
						field = "DestinationSystemPath:" + replayConfiguration.DestinationSystemPath;
						return replayConfiguration;
					}
				}
			}
			return null;
		}

		public bool ConfigurationPathConflict(IADDatabase[] databases, out string field)
		{
			field = string.Empty;
			if (databases != null)
			{
				int i = 0;
				while (i < databases.Length)
				{
					IADDatabase iaddatabase = databases[i];
					bool result;
					if (Cluster.StringIEquals(this.DestinationLogPath, iaddatabase.LogFolderPath.PathName))
					{
						field = "DestinationLogPath:" + this.DestinationLogPath;
						result = true;
					}
					else if (Cluster.StringIEquals(this.DestinationEdbPath, iaddatabase.EdbFilePath.PathName))
					{
						field = "DestinationEdbPath:" + this.DestinationEdbPath;
						result = true;
					}
					else
					{
						if (!Cluster.StringIEquals(this.DestinationSystemPath, iaddatabase.SystemFolderPath.PathName))
						{
							i++;
							continue;
						}
						field = "DestinationSystemPath:" + this.DestinationSystemPath;
						result = true;
					}
					return result;
				}
			}
			return false;
		}

		private AutoDatabaseMountDial m_autoDatabaseMountDial;
	}
}
