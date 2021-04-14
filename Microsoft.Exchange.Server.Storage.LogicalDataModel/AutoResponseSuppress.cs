using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	[Flags]
	internal enum AutoResponseSuppress
	{
		None = 0,
		DeliveryReceipt = 1,
		NonDeliveryReceipt = 2,
		ReadNotification = 4,
		NotReadNotification = 8,
		OOF = 16,
		AutoReply = 32
	}
}
