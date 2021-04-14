using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum TextMessageDeliveryStatus
	{
		None,
		Submitted,
		RoutedToDeliveryPoint = 25,
		RoutedToExternalMessagingSystem = 50,
		Delivered = 100
	}
}
