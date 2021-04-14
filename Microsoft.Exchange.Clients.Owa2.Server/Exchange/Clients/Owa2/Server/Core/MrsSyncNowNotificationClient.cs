using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class MrsSyncNowNotificationClient : ISyncNowNotificationClient
	{
		void ISyncNowNotificationClient.NotifyOWALogonTriggeredSyncNowNeeded(Guid mailboxGuid, Guid mdbGuid, string mailboxServer)
		{
			this.SyncNow(mailboxGuid, mdbGuid);
		}

		void ISyncNowNotificationClient.NotifyOWARefreshButtonTriggeredSyncNowNeeded(Guid mailboxGuid, Guid mdbGuid, string mailboxServer)
		{
			this.SyncNow(mailboxGuid, mdbGuid);
		}

		void ISyncNowNotificationClient.NotifyOWAActivityTriggeredSyncNowNeeded(Guid mailboxGuid, Guid mdbGuid, string mailboxServer)
		{
			this.SyncNow(mailboxGuid, mdbGuid);
		}

		private void SyncNow(Guid mailboxGuid, Guid mdbGuid)
		{
			MailboxReplicationServiceClientSlim.NotifyToSync(SyncNowNotificationFlags.ActivateJob, mailboxGuid, mdbGuid);
		}
	}
}
