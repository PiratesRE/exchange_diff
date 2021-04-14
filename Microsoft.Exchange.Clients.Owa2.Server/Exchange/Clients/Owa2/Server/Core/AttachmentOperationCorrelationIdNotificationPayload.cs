using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class AttachmentOperationCorrelationIdNotificationPayload : NotificationPayloadBase
	{
		public AttachmentOperationCorrelationIdNotificationPayload()
		{
			base.SubscriptionId = NotificationType.AttachmentOperationCorrelationIdNotification.ToString();
		}

		[DataMember]
		public string CorrelationId { get; set; }

		[DataMember]
		public string SharePointCallName { get; set; }
	}
}
