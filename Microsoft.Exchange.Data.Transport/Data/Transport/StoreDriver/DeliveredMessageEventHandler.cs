using System;

namespace Microsoft.Exchange.Data.Transport.StoreDriver
{
	internal delegate void DeliveredMessageEventHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs e);
}
