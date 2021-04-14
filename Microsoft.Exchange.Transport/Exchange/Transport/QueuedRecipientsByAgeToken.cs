using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport
{
	internal class QueuedRecipientsByAgeToken
	{
		public static QueuedRecipientsByAgeToken Generate(TransportMailItem item)
		{
			return new QueuedRecipientsByAgeToken(item.LatencyStartTime, DeliveryType.Undefined, item.Priority, item.Recipients.Count);
		}

		public static QueuedRecipientsByAgeToken Generate(RoutedMailItem item)
		{
			return new QueuedRecipientsByAgeToken(item.LatencyStartTime, item.DeliveryType, item.Priority, item.Recipients.Count);
		}

		public DateTime OrgArrivalTimeUsed
		{
			get
			{
				return this.orgArrivalTimeUsed;
			}
			set
			{
				this.orgArrivalTimeUsed = value;
			}
		}

		public DateTime OrgArrivalTimeUtc
		{
			get
			{
				return this.orgArrivalTimeUtc;
			}
		}

		public DeliveryType DeliveryType
		{
			get
			{
				return this.deliveryType;
			}
		}

		public DeliveryPriority DeliveryPriority
		{
			get
			{
				return this.deliveryPriority;
			}
		}

		public int RecipientCount
		{
			get
			{
				return this.recipientCount;
			}
		}

		private QueuedRecipientsByAgeToken(DateTime orgArrivalTimeUtc, DeliveryType deliveryType, DeliveryPriority deliveryPriority, int recipientCount)
		{
			this.orgArrivalTimeUtc = orgArrivalTimeUtc;
			this.deliveryType = deliveryType;
			this.deliveryPriority = deliveryPriority;
			this.recipientCount = recipientCount;
		}

		private DateTime orgArrivalTimeUsed;

		private readonly DateTime orgArrivalTimeUtc;

		private readonly DeliveryType deliveryType;

		private readonly DeliveryPriority deliveryPriority;

		private readonly int recipientCount;
	}
}
