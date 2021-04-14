using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Delivery;

namespace Microsoft.Exchange.Transport.Delivery
{
	internal class InternalOpenConnectionEventArgs : OpenConnectionEventArgs
	{
		public InternalOpenConnectionEventArgs(DeliverableMailItem deliverableMailItem, string nextHopDomain)
		{
			this.deliverableMailItem = deliverableMailItem;
			this.nextHopDomain = new RoutingDomain(nextHopDomain);
		}

		public override DeliverableMailItem DeliverableMailItem
		{
			get
			{
				return this.deliverableMailItem;
			}
		}

		public override RoutingDomain NextHopDomain
		{
			get
			{
				return this.nextHopDomain;
			}
		}

		private DeliverableMailItem deliverableMailItem;

		private RoutingDomain nextHopDomain;
	}
}
