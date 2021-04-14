using System;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors
{
	internal class EventNotificationSender : IEventNotificationSender
	{
		public void CreateAndPublishMailboxEventNotification(string notificationReason, DirectoryMailbox mailbox, DirectoryDatabase database)
		{
			if (mailbox == null)
			{
				return;
			}
			Guid guid = (database == null) ? Guid.Empty : database.Guid;
			EventNotificationItem eventNotificationItem = new EventNotificationItem(ExchangeComponent.MailboxMigration.Name, ExchangeComponent.MailboxMigration.Name, notificationReason, ResultSeverityLevel.Error)
			{
				StateAttribute1 = mailbox.Guid.ToString(),
				StateAttribute2 = guid.ToString(),
				StateAttribute3 = mailbox.OrganizationId.ToString(),
				StateAttribute4 = "Client=MSExchangeMailboxLoadBalance"
			};
			eventNotificationItem.Publish(false);
		}
	}
}
