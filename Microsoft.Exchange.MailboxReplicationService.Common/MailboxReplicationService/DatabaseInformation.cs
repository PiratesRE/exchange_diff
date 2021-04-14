using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal struct DatabaseInformation
	{
		public Guid MdbGuid { get; private set; }

		public string DatabaseName { get; private set; }

		public string ServerDN { get; private set; }

		public string ServerFqdn { get; private set; }

		public Guid ServerGuid { get; private set; }

		public ADObjectId ServerSite { get; private set; }

		public int ServerVersion { get; private set; }

		public Guid SystemMailboxGuid { get; private set; }

		public MailboxRelease MailboxRelease { get; private set; }

		public string ForestFqdn { get; private set; }

		public bool IsMissing
		{
			get
			{
				return string.IsNullOrEmpty(this.DatabaseName);
			}
		}

		public bool IsOnThisServer
		{
			get
			{
				if (Guid.Empty.Equals(this.ServerGuid))
				{
					return StringComparer.OrdinalIgnoreCase.Equals(this.ServerFqdn, CommonUtils.LocalComputerName);
				}
				return this.ServerGuid == CommonUtils.LocalServerGuid;
			}
		}

		public bool IsInLocalSite
		{
			get
			{
				return ADObjectId.Equals(this.ServerSite, CommonUtils.LocalSiteId);
			}
		}

		public static DatabaseInformation FromDatabaseLocationInfo(Guid mdbGuid, DatabaseLocationInfo location, Guid systemMailboxGuid)
		{
			return new DatabaseInformation
			{
				MdbGuid = mdbGuid,
				DatabaseName = location.DatabaseName,
				ServerDN = location.ServerLegacyDN,
				ServerFqdn = location.ServerFqdn,
				ServerGuid = location.ServerGuid,
				ServerSite = location.ServerSite,
				ServerVersion = location.ServerVersion,
				MailboxRelease = location.MailboxRelease,
				SystemMailboxGuid = systemMailboxGuid
			};
		}

		public static DatabaseInformation FromBackEndServer(ADObjectId database, BackEndServer backend)
		{
			return new DatabaseInformation
			{
				MdbGuid = database.ObjectGuid,
				DatabaseName = database.Name,
				ServerFqdn = backend.Fqdn,
				ServerVersion = backend.Version
			};
		}

		public static DatabaseInformation Missing(Guid mdbGuid, string forestFqdn)
		{
			return new DatabaseInformation
			{
				MdbGuid = mdbGuid,
				ForestFqdn = (forestFqdn ?? "Local Resource Forest")
			};
		}

		public static DatabaseInformation FromAD(Database db, MiniServer server, Guid systemMailboxGuid)
		{
			MailboxRelease mailboxRelease;
			return new DatabaseInformation
			{
				MdbGuid = db.Guid,
				DatabaseName = db.Name,
				ServerDN = server.ExchangeLegacyDN,
				ServerFqdn = server.Fqdn,
				ServerGuid = server.Guid,
				ServerSite = server.ServerSite,
				ServerVersion = server.VersionNumber,
				SystemMailboxGuid = systemMailboxGuid,
				MailboxRelease = (Enum.TryParse<MailboxRelease>((string)server[ActiveDirectoryServerSchema.MailboxRelease], true, out mailboxRelease) ? mailboxRelease : MailboxRelease.None)
			};
		}
	}
}
