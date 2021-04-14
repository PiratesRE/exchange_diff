using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	[Flags]
	public enum MailFlags : ushort
	{
		None = 0,
		DeliveryCompleted = 1,
		NeedsReadNotification = 2,
		NeedsNotReadNotification = 4,
		OOFCanBeSent = 8,
		SentRepresentingAddedByTransport = 16,
		ReadReceiptSent = 32
	}
}
