using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Notifications.Broker
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public sealed class ConversationNotification : RowNotification
	{
		public ConversationNotification() : base(NotificationType.Conversation)
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public ConversationType Conversation { get; set; }
	}
}
