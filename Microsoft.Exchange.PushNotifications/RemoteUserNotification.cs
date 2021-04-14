using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class RemoteUserNotification : UserNotification<RemoteUserNotificationPayload>
	{
		public RemoteUserNotification(string workloadId, RemoteUserNotificationPayload payload, List<UserNotificationRecipient> recipients) : base(workloadId, payload, recipients)
		{
		}

		protected override Notification CreateFragment(RemoteUserNotificationPayload payload, UserNotificationRecipient recipient)
		{
			return new RemoteUserNotificationFragment(base.Identifier, payload, recipient);
		}
	}
}
