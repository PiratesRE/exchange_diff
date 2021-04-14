using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	internal class NewMailNotificationPayload : NotificationPayloadBase
	{
		[DataMember]
		public string Sender { get; set; }

		[DataMember]
		public string Subject { get; set; }

		[DataMember]
		public string PreviewText { get; set; }

		[DataMember]
		public string ItemId { get; set; }

		[DataMember]
		public string ConversationId { get; set; }

		[DataMember]
		public bool IsClutter { get; set; }
	}
}
