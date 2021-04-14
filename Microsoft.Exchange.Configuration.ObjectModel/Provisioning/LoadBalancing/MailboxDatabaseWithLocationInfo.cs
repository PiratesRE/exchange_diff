using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Provisioning.LoadBalancing
{
	internal class MailboxDatabaseWithLocationInfo
	{
		public MailboxDatabase MailboxDatabase { get; private set; }

		public DatabaseLocationInfo DatabaseLocationInfo { get; private set; }

		public MailboxDatabaseWithLocationInfo(MailboxDatabase mailboxDatabase, DatabaseLocationInfo databaseLocationInfo)
		{
			if (mailboxDatabase == null)
			{
				throw new ArgumentNullException("mailboxDatabase");
			}
			if (databaseLocationInfo == null)
			{
				throw new ArgumentNullException("databaseLocationInfo");
			}
			this.MailboxDatabase = mailboxDatabase;
			this.DatabaseLocationInfo = databaseLocationInfo;
		}
	}
}
