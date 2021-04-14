using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class ConversationNotification : BaseNotification
	{
		public ConversationNotification() : base(NotificationKindType.Conversation)
		{
		}

		public ConversationType Conversation { get; set; }
	}
}
