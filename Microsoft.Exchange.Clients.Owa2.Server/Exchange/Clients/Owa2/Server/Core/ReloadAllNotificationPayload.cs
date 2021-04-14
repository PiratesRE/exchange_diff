using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	internal class ReloadAllNotificationPayload : NotificationPayloadBase
	{
		public ReloadAllNotificationPayload()
		{
			base.SubscriptionId = "ReloadAllNotification";
		}

		public const string ReloadAllNotificationId = "ReloadAllNotification";
	}
}
