using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public sealed class MessageItemNotification : RowNotification
	{
		public MessageItemNotification() : base(NotificationType.MessageItem)
		{
		}
	}
}
