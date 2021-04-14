using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.SharedMailboxSentItems
{
	internal interface IAgentInfoWriter
	{
		void AddAgentInfo(string eventName, string message);
	}
}
