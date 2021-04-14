using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.SharedMailboxSentItems
{
	internal class PerformanceCountersFactory : IPerformanceCountersFactory
	{
		public IPerformanceCounters GetCounterInstance(string mdbGuid)
		{
			return new PerformanceCounters(SharedMailboxSentItemsDeliveryAgent.GetInstance(mdbGuid));
		}
	}
}
