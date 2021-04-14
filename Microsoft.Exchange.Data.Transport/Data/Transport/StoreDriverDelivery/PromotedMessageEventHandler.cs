using System;
using Microsoft.Exchange.Data.Transport.StoreDriver;

namespace Microsoft.Exchange.Data.Transport.StoreDriverDelivery
{
	internal delegate void PromotedMessageEventHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs e);
}
