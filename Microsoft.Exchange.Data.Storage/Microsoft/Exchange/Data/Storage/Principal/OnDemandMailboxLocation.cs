using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OnDemandMailboxLocation : IMailboxLocation
	{
		public OnDemandMailboxLocation(Func<IMailboxLocation> mailboxLocationFactory)
		{
			ArgumentValidator.ThrowIfNull("mailboxLocationFactory", mailboxLocationFactory);
			this.mailboxLocationFactory = mailboxLocationFactory;
		}

		public string ServerFqdn
		{
			get
			{
				return this.GetDatabaseLocation().ServerFqdn;
			}
		}

		public Guid ServerGuid
		{
			get
			{
				return this.GetDatabaseLocation().ServerGuid;
			}
		}

		public string ServerLegacyDn
		{
			get
			{
				return this.GetDatabaseLocation().ServerLegacyDn;
			}
		}

		public int ServerVersion
		{
			get
			{
				return this.GetDatabaseLocation().ServerVersion;
			}
		}

		public ADObjectId ServerSite
		{
			get
			{
				return this.GetDatabaseLocation().ServerSite;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.GetDatabaseLocation().DatabaseName;
			}
		}

		public string RpcClientAccessServerLegacyDn
		{
			get
			{
				return this.GetDatabaseLocation().RpcClientAccessServerLegacyDn;
			}
		}

		public string DatabaseLegacyDn
		{
			get
			{
				return this.GetDatabaseLocation().DatabaseLegacyDn;
			}
		}

		public Guid HomePublicFolderDatabaseGuid
		{
			get
			{
				return this.GetDatabaseLocation().HomePublicFolderDatabaseGuid;
			}
		}

		private IMailboxLocation GetDatabaseLocation()
		{
			if (this.databaseLocation == null && this.mailboxLocationFactory != null)
			{
				this.databaseLocation = this.mailboxLocationFactory();
				this.mailboxLocationFactory = null;
			}
			return this.databaseLocation;
		}

		public override string ToString()
		{
			return this.GetDatabaseLocation().ToString();
		}

		private Func<IMailboxLocation> mailboxLocationFactory;

		private IMailboxLocation databaseLocation;
	}
}
