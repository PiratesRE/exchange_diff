using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class MessageNotification : BaseNotification
	{
		public MessageNotification() : base(NotificationKindType.Message)
		{
		}

		public MessageType Message { get; set; }
	}
}
