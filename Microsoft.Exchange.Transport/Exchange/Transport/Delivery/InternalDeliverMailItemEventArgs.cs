using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Delivery;

namespace Microsoft.Exchange.Transport.Delivery
{
	internal class InternalDeliverMailItemEventArgs : DeliverMailItemEventArgs
	{
		public InternalDeliverMailItemEventArgs(DeliverableMailItem deliverableMailItem)
		{
			this.deliverableMailItem = deliverableMailItem;
		}

		public override DeliverableMailItem DeliverableMailItem
		{
			get
			{
				return this.deliverableMailItem;
			}
		}

		private DeliverableMailItem deliverableMailItem;
	}
}
