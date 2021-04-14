using System;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors
{
	internal interface IEventNotificationSender
	{
		void CreateAndPublishMailboxEventNotification(string notificationReason, DirectoryMailbox mailbox, DirectoryDatabase database);
	}
}
