using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ServerNotInSiteException : ServiceDiscoveryPermanentException
	{
		public ServerNotInSiteException(string message, string serverName) : base(message)
		{
			this.ServerName = serverName;
		}

		public ServerNotInSiteException(string message, string serverName, string mailboxDisplayName) : this(message, serverName)
		{
			this.MailboxDisplayName = mailboxDisplayName;
		}

		public readonly string ServerName;

		public readonly string MailboxDisplayName;
	}
}
