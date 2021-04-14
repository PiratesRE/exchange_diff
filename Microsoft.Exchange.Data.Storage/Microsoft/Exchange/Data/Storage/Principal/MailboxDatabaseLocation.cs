using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxDatabaseLocation : IMailboxLocation
	{
		public MailboxDatabaseLocation(DatabaseLocationInfo locationInfo)
		{
			ArgumentValidator.ThrowIfNull("locationInfo", locationInfo);
			this.ServerFqdn = locationInfo.ServerFqdn;
			this.ServerGuid = locationInfo.ServerGuid;
			this.ServerLegacyDn = locationInfo.ServerLegacyDN;
			this.ServerVersion = locationInfo.ServerVersion;
			this.ServerSite = locationInfo.ServerSite;
			this.DatabaseName = locationInfo.DatabaseName;
			this.DatabaseLegacyDn = locationInfo.DatabaseLegacyDN;
			this.RpcClientAccessServerLegacyDn = locationInfo.RpcClientAccessServerLegacyDN;
			this.HomePublicFolderDatabaseGuid = locationInfo.HomePublicFolderDatabaseGuid;
		}

		private MailboxDatabaseLocation()
		{
		}

		public string ServerFqdn
		{
			get
			{
				if (string.IsNullOrEmpty(this.serverFqdn))
				{
					throw new DatabaseLocationUnavailableException(ServerStrings.ExCurrentServerNotInSite(string.Empty));
				}
				return this.serverFqdn;
			}
			private set
			{
				this.serverFqdn = value;
			}
		}

		public Guid ServerGuid { get; private set; }

		public string ServerLegacyDn { get; private set; }

		public int ServerVersion { get; private set; }

		public ADObjectId ServerSite { get; private set; }

		public string DatabaseName { get; private set; }

		public string RpcClientAccessServerLegacyDn { get; private set; }

		public string DatabaseLegacyDn { get; private set; }

		public Guid HomePublicFolderDatabaseGuid { get; private set; }

		public override string ToString()
		{
			return string.Format("ServerFqdn: {0}, ServerVersion: {1}, DatabaseName: {2}, HomePublicFolderDatabaseGuid: {3}", new object[]
			{
				this.ServerFqdn,
				this.ServerVersion,
				this.DatabaseName,
				this.HomePublicFolderDatabaseGuid
			});
		}

		public static IMailboxLocation Unknown = new MailboxDatabaseLocation();

		private string serverFqdn;
	}
}
