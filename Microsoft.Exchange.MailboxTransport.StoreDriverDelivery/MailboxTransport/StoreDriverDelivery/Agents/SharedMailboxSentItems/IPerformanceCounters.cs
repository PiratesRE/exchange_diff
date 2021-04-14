using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.SharedMailboxSentItems
{
	internal interface IPerformanceCounters
	{
		void IncrementSentItemsMessages();

		void IncrementErrors();

		void UpdateAverageMessageCopyTime(TimeSpan elapsedTime);
	}
}
