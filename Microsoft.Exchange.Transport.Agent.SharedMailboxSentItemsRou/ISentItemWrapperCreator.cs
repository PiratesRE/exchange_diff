using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Agent.SharedMailboxSentItemsRoutingAgent
{
	internal interface ISentItemWrapperCreator
	{
		Exception CreateAndSubmit(MailItem item, int traceId);
	}
}
