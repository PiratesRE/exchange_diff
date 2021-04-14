using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class OutlookNotification : BasicMulticastNotification<OutlookNotificationPayload, OutlookNotificationRecipient>
	{
		public OutlookNotification(OutlookNotificationPayload payload, List<OutlookNotificationRecipient> recipients) : base(payload, recipients)
		{
		}

		protected override Notification CreateFragment(OutlookNotificationPayload payload, OutlookNotificationRecipient recipient)
		{
			return new OutlookNotificationFragment(base.Identifier, payload, recipient);
		}
	}
}
