using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public class NewMailNotification : ApplicationNotification
	{
		public NewMailNotification() : base(NotificationType.NewMail)
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public string Sender { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Subject { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string PreviewText { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ItemId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ConversationId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool IsClutter { get; set; }
	}
}
