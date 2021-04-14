using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class MailboxNotification : BasicMulticastNotification<MailboxNotificationPayload, MailboxNotificationRecipient>
	{
		public MailboxNotification(MailboxNotificationPayload payload, List<MailboxNotificationRecipient> recipients) : base(payload, recipients)
		{
		}

		protected override Notification CreateFragment(MailboxNotificationPayload payload, MailboxNotificationRecipient recipient)
		{
			return new MailboxNotificationFragment(base.Identifier, payload, recipient);
		}
	}
}
