using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class LocalUserNotification : UserNotification<LocalUserNotificationPayload>
	{
		public LocalUserNotification(string workloadId, LocalUserNotificationPayload payload, List<UserNotificationRecipient> recipients) : base(workloadId, payload, recipients)
		{
		}

		protected override Notification CreateFragment(LocalUserNotificationPayload payload, UserNotificationRecipient recipient)
		{
			return new LocalUserNotificationFragment(base.Identifier, payload, recipient);
		}
	}
}
