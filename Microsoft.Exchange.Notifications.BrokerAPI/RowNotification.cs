using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Notifications.Broker
{
	[KnownType(typeof(ConversationNotification))]
	[KnownType(typeof(PeopleIKnowNotification))]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[KnownType(typeof(MessageItemNotification))]
	[KnownType(typeof(CalendarItemNotification))]
	[KnownType(typeof(ItemType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public abstract class RowNotification : ApplicationNotification
	{
		protected RowNotification(NotificationType notificationType) : base(notificationType)
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public ItemType Item { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Prior { get; set; }

		[DataMember]
		public string FolderId { get; set; }
	}
}
