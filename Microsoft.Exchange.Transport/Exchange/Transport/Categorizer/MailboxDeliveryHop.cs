using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class MailboxDeliveryHop : RoutingNextHop
	{
		public MailboxDeliveryHop(ADObjectId databaseId, DeliveryType deliveryType)
		{
			RoutingUtils.ThrowIfNullOrEmpty(databaseId, "databaseId");
			if (!NextHopType.IsMailboxDeliveryType(deliveryType))
			{
				throw new ArgumentOutOfRangeException("deliveryType", deliveryType, "Non-mailbox delivery type provided: " + deliveryType);
			}
			this.databaseId = databaseId;
			this.deliveryType = deliveryType;
		}

		public override DeliveryType DeliveryType
		{
			get
			{
				return this.deliveryType;
			}
		}

		public override Guid NextHopGuid
		{
			get
			{
				return this.databaseId.ObjectGuid;
			}
		}

		public override string GetNextHopDomain(RoutingContext context)
		{
			return this.databaseId.Name;
		}

		public override bool Match(RoutingNextHop other)
		{
			return other is MailboxDeliveryHop;
		}

		private ADObjectId databaseId;

		private DeliveryType deliveryType;
	}
}
