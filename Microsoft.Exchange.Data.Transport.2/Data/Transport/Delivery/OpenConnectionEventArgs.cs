using System;

namespace Microsoft.Exchange.Data.Transport.Delivery
{
	public abstract class OpenConnectionEventArgs
	{
		public abstract DeliverableMailItem DeliverableMailItem { get; }

		public abstract RoutingDomain NextHopDomain { get; }
	}
}
