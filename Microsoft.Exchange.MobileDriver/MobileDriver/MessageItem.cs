using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class MessageItem
	{
		public MessageItem(Message message, MobileRecipient sender, ICollection<MobileRecipient> recipients, int maxSegmentsPerRecipient)
		{
			this.Message = message;
			this.Sender = sender;
			this.Recipients = recipients;
			this.MaxSegmentsPerRecipient = maxSegmentsPerRecipient;
		}

		public Message Message { get; private set; }

		public MobileRecipient Sender { get; private set; }

		public ICollection<MobileRecipient> Recipients { get; private set; }

		public int MaxSegmentsPerRecipient { get; private set; }
	}
}
