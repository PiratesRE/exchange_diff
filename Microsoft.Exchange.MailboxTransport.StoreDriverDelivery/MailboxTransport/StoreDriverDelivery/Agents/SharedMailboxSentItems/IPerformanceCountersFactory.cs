using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.SharedMailboxSentItems
{
	internal interface IPerformanceCountersFactory
	{
		IPerformanceCounters GetCounterInstance(string mdbGuid);
	}
}
