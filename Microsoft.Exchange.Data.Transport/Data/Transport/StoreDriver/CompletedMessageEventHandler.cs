using System;

namespace Microsoft.Exchange.Data.Transport.StoreDriver
{
	internal delegate void CompletedMessageEventHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs e);
}
