using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.SharedMailboxSentItems
{
	internal sealed class PerformanceCounters : IPerformanceCounters
	{
		public PerformanceCounters(SharedMailboxSentItemsDeliveryAgentInstance perfCounterInstance)
		{
			ArgumentValidator.ThrowIfNull("perfCounterInstance", perfCounterInstance);
			this.perfCounterInstance = perfCounterInstance;
		}

		public void IncrementSentItemsMessages()
		{
			this.perfCounterInstance.SentItemsMessages.Increment();
		}

		public void IncrementErrors()
		{
			this.perfCounterInstance.Errors.Increment();
		}

		public void UpdateAverageMessageCopyTime(TimeSpan elapsedTime)
		{
			this.perfCounterInstance.AverageMessageCopyTime.IncrementBy(elapsedTime.Ticks);
			this.perfCounterInstance.AverageMessageCopyTimeBase.Increment();
		}

		private readonly SharedMailboxSentItemsDeliveryAgentInstance perfCounterInstance;
	}
}
