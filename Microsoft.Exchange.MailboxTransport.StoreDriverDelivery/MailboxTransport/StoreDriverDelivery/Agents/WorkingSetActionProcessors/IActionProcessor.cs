using System;
using Microsoft.Exchange.WorkingSet.SignalApi;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.WorkingSetActionProcessors
{
	internal interface IActionProcessor
	{
		void Process(StoreDriverDeliveryEventArgsImpl argsImpl, Action action, int traceId);
	}
}
