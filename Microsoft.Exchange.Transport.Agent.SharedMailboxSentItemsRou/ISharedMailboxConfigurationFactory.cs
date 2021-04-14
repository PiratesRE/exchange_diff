using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Agent.SharedMailboxSentItemsRoutingAgent
{
	internal interface ISharedMailboxConfigurationFactory
	{
		SharedMailboxConfiguration GetSharedMailboxConfiguration(MailItem transportMailItem, string sender);
	}
}
