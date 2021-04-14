using System;
using Microsoft.Exchange.Data.Transport.StoreDriver;

namespace Microsoft.Exchange.Data.Transport.StoreDriverDelivery
{
	internal delegate void CompletedMessageEventHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs e);
}
