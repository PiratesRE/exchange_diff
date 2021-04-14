using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class CreateGroupNotificationPayload : NotificationPayloadBase
	{
		public CreateGroupNotificationPayload()
		{
			base.SubscriptionId = NotificationType.GroupCreateNotification.ToString();
		}

		[DataMember]
		public string ExternalDirectoryObjectId { get; set; }

		[DataMember]
		public string PushToken { get; set; }
	}
}
