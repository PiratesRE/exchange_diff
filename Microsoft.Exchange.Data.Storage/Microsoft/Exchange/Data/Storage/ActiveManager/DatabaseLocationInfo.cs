using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[Serializable]
	public class DatabaseLocationInfo
	{
		internal static DatabaseLocationInfo CloneDatabaseLocationInfo(DatabaseLocationInfo right, DatabaseLocationInfoResult requestResult)
		{
			return new DatabaseLocationInfo(right.ServerFqdn, right.ServerLegacyDN, right.LastMountedServerFqdn, right.LastMountedServerLegacyDN, right.DatabaseLegacyDN, right.RpcClientAccessServerLegacyDN, right.DatabaseName, right.DatabaseIsPublic, right.DatabaseIsRestored, right.HomePublicFolderDatabaseGuid, right.MountedTime, right.serverGuid, right.ServerSite, right.AdminDisplayVersion, right.MailboxRelease, requestResult, right.IsDatabaseHighlyAvailable);
		}

		public string ServerFqdn
		{
			get
			{
				return this.serverFqdn;
			}
			private set
			{
				if (value != null)
				{
					this.serverFqdn = string.Intern(value);
					return;
				}
				this.serverFqdn = null;
			}
		}

		public string ServerLegacyDN
		{
			get
			{
				return this.serverLegacyDN;
			}
			private set
			{
				if (value != null)
				{
					this.serverLegacyDN = string.Intern(value);
					return;
				}
				this.serverLegacyDN = null;
			}
		}

		public string DatabaseLegacyDN { get; private set; }

		public string RpcClientAccessServerLegacyDN { get; private set; }

		public ADObjectId ServerSite { get; private set; }

		public ServerVersion AdminDisplayVersion { get; private set; }

		public int ServerVersion
		{
			get
			{
				if (this.AdminDisplayVersion == null)
				{
					return 0;
				}
				return this.AdminDisplayVersion.ToInt();
			}
		}

		public MailboxRelease MailboxRelease { get; private set; }

		public bool IsDatabaseHighlyAvailable { get; private set; }

		public string LastMountedServerFqdn
		{
			get
			{
				return this.lastMountedServerFqdn;
			}
			private set
			{
				if (value != null)
				{
					this.lastMountedServerFqdn = string.Intern(value);
					return;
				}
				this.lastMountedServerFqdn = null;
			}
		}

		public string LastMountedServerLegacyDN
		{
			get
			{
				return this.lastMountedServerLegacyDN;
			}
			private set
			{
				if (value != null)
				{
					this.lastMountedServerLegacyDN = string.Intern(value);
					return;
				}
				this.lastMountedServerLegacyDN = null;
			}
		}

		public Guid HomePublicFolderDatabaseGuid { get; private set; }

		public DateTime MountedTime { get; private set; }

		public string DatabaseName { get; private set; }

		public bool DatabaseIsPublic { get; private set; }

		public bool DatabaseIsRestored { get; private set; }

		public DatabaseLocationInfoResult RequestResult { get; private set; }

		internal DatabaseLocationInfo(Server server, bool isDatabaseHighlyAvailable) : this(server.Fqdn, server.ExchangeLegacyDN, ActiveManagerUtil.GetServerSiteFromServer(server), server.AdminDisplayVersion, isDatabaseHighlyAvailable)
		{
			this.serverGuid = new Guid?(server.Guid);
		}

		internal DatabaseLocationInfo(string serverFqdn, string serverLegacyDN, ADObjectId serverSite, ServerVersion serverVersion, bool isDatabaseHighlyAvailable) : this(serverFqdn, serverLegacyDN, serverFqdn, serverLegacyDN, string.Empty, string.Empty, string.Empty, false, false, Guid.Empty, DateTime.MinValue, null, serverSite, serverVersion, MailboxRelease.None, DatabaseLocationInfoResult.Success, isDatabaseHighlyAvailable)
		{
		}

		internal DatabaseLocationInfo(string serverFqdn, string serverLegacyDN, string lastMountedServerFqdn, string lastMountedServerLegacyDN, string databaseLegacyDN, string rpcClientAcessServerLegacyDN, string databaseName, bool databaseIsPublic, bool databaseIsRestored, Guid homePublicFolderDatabaseGuid, DateTime mountedTime, Guid? serverGuid, ADObjectId serverSite, ServerVersion serverVersion, MailboxRelease mailboxRelease, DatabaseLocationInfoResult requestResult, bool isDatabaseHighlyAvailable)
		{
			this.UpdateInPlace(serverFqdn, serverLegacyDN, lastMountedServerFqdn, lastMountedServerLegacyDN, databaseLegacyDN, rpcClientAcessServerLegacyDN, databaseName, databaseIsPublic, databaseIsRestored, homePublicFolderDatabaseGuid, mountedTime, serverGuid, serverSite, serverVersion, mailboxRelease, requestResult, isDatabaseHighlyAvailable);
		}

		internal void UpdateInPlace(string serverFqdn, string serverLegacyDN, string lastMountedServerFqdn, string lastMountedServerLegacyDN, string databaseLegacyDN, string rpcClientAcessServerLegacyDN, string databaseName, bool databaseIsPublic, bool databaseIsRestored, Guid homePublicFolderDatabaseGuid, DateTime mountedTime, Guid? serverGuid, ADObjectId serverSite, ServerVersion serverVersion, MailboxRelease mailboxRelease, DatabaseLocationInfoResult requestResult, bool isDatabaseHighlyAvailable)
		{
			if (serverFqdn != null && serverFqdn.Length == 0)
			{
				throw new ArgumentException("serverFqdn is empty", "serverFqdn");
			}
			if (serverLegacyDN != null && serverLegacyDN.Length == 0)
			{
				throw new ArgumentException("serverLegacyDN is empty", "serverLegacyDN");
			}
			if (lastMountedServerFqdn != null && lastMountedServerFqdn.Length == 0)
			{
				throw new ArgumentException("lastMountedServerFqdn is empty", "lastMountedServerFqdn");
			}
			if (lastMountedServerLegacyDN != null && lastMountedServerLegacyDN.Length == 0)
			{
				throw new ArgumentException("lastMountedServerLegacyDN is empty", "lastMountedServerLegacyDN");
			}
			this.ServerFqdn = serverFqdn;
			this.ServerLegacyDN = serverLegacyDN;
			this.DatabaseLegacyDN = databaseLegacyDN;
			this.RpcClientAccessServerLegacyDN = rpcClientAcessServerLegacyDN;
			this.DatabaseName = databaseName;
			this.DatabaseIsPublic = databaseIsPublic;
			this.DatabaseIsRestored = databaseIsRestored;
			this.HomePublicFolderDatabaseGuid = homePublicFolderDatabaseGuid;
			this.serverGuid = serverGuid;
			this.ServerSite = serverSite;
			this.AdminDisplayVersion = serverVersion;
			this.MailboxRelease = mailboxRelease;
			this.IsDatabaseHighlyAvailable = isDatabaseHighlyAvailable;
			this.LastMountedServerFqdn = (lastMountedServerFqdn ?? serverFqdn);
			this.LastMountedServerLegacyDN = (lastMountedServerLegacyDN ?? serverLegacyDN);
			this.MountedTime = mountedTime;
			this.RequestResult = requestResult;
		}

		internal bool DetectFailover(DatabaseLocationInfo oldDatabaseLocationInfo)
		{
			return this.ServerSite != oldDatabaseLocationInfo.ServerSite || this.ServerLegacyDN != oldDatabaseLocationInfo.ServerLegacyDN;
		}

		internal virtual bool Equals(DatabaseLocationInfo cmpObj)
		{
			return cmpObj != null && (this.DatabaseIsPublic == cmpObj.DatabaseIsPublic && this.DatabaseIsRestored == cmpObj.DatabaseIsRestored && this.IsDatabaseHighlyAvailable == cmpObj.IsDatabaseHighlyAvailable && this.HomePublicFolderDatabaseGuid == cmpObj.HomePublicFolderDatabaseGuid && this.MailboxRelease == cmpObj.MailboxRelease && this.RequestResult == cmpObj.RequestResult && string.Equals(this.DatabaseLegacyDN, cmpObj.DatabaseLegacyDN) && string.Equals(this.DatabaseName, cmpObj.DatabaseName) && string.Equals(this.LastMountedServerFqdn, cmpObj.LastMountedServerFqdn) && string.Equals(this.LastMountedServerLegacyDN, cmpObj.LastMountedServerLegacyDN) && string.Equals(this.RpcClientAccessServerLegacyDN, cmpObj.RpcClientAccessServerLegacyDN) && string.Equals(this.ServerFqdn, cmpObj.ServerFqdn) && string.Equals(this.ServerLegacyDN, cmpObj.ServerLegacyDN) && this.MountedTime.Equals(cmpObj.MountedTime) && (object.ReferenceEquals(this.ServerSite, cmpObj.ServerSite) || (this.ServerSite != null && this.ServerSite.Equals(cmpObj.ServerSite))));
		}

		public override string ToString()
		{
			return string.Format("({0};{1};{2};{3};{4};{5};{6};{7})", new object[]
			{
				this.ServerFqdn,
				this.ServerLegacyDN,
				this.ServerVersion,
				this.ServerSite,
				this.IsDatabaseHighlyAvailable,
				this.LastMountedServerFqdn ?? string.Empty,
				this.LastMountedServerLegacyDN ?? string.Empty,
				this.MountedTime
			});
		}

		public Guid ServerGuid
		{
			get
			{
				if (this.serverGuid == null)
				{
					IADToplogyConfigurationSession adSession = ADSessionFactory.CreateFullyConsistentRootOrgSession(true);
					ADObjectId adobjectId = ActiveManagerImplementation.TryGetServerIdByFqdn(new SimpleMiniServerLookup(adSession), this.ServerFqdn);
					this.serverGuid = new Guid?((adobjectId != null) ? adobjectId.ObjectGuid : Guid.Empty);
				}
				return this.serverGuid.Value;
			}
		}

		private Guid? serverGuid;

		private string serverFqdn;

		private string serverLegacyDN;

		private string lastMountedServerFqdn;

		private string lastMountedServerLegacyDN;
	}
}
