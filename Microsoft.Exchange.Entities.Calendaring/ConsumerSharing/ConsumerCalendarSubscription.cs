using System;

namespace Microsoft.Exchange.Entities.Calendaring.ConsumerSharing
{
	internal class ConsumerCalendarSubscription
	{
		public ConsumerCalendarSubscription(long consumerCalendarOwnerId, Guid consumerCalendarGuid)
		{
			this.ConsumerCalendarOwnerId = consumerCalendarOwnerId;
			this.ConsumerCalendarGuid = consumerCalendarGuid;
		}

		public long ConsumerCalendarOwnerId { get; private set; }

		public Guid ConsumerCalendarGuid { get; private set; }
	}
}
