using System;
using Microsoft.Exchange.MailboxLoadBalance.Band;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	internal interface IRebalancingRequestProcessor
	{
		void ProcessRebalanceRequest(BandMailboxRebalanceData rebalanceRequest);
	}
}
