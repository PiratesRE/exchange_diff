using System;

namespace Microsoft.Exchange.Data
{
	public interface ISyncNowNotificationClient
	{
		void NotifyOWALogonTriggeredSyncNowNeeded(Guid mailboxGuid, Guid mdbGuid, string mailboxServer);

		void NotifyOWARefreshButtonTriggeredSyncNowNeeded(Guid mailboxGuid, Guid mdbGuid, string mailboxServer);

		void NotifyOWAActivityTriggeredSyncNowNeeded(Guid mailboxGuid, Guid mdbGuid, string mailboxServer);
	}
}
