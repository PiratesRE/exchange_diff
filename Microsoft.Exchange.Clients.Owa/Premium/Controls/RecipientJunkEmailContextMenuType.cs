using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	[Flags]
	public enum RecipientJunkEmailContextMenuType
	{
		None = 0,
		Sender = 2,
		Recipient = 4,
		SenderAndRecipient = 6
	}
}
