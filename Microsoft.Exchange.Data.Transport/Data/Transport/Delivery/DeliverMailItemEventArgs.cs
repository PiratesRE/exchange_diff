using System;

namespace Microsoft.Exchange.Data.Transport.Delivery
{
	public abstract class DeliverMailItemEventArgs
	{
		public abstract DeliverableMailItem DeliverableMailItem { get; }
	}
}
