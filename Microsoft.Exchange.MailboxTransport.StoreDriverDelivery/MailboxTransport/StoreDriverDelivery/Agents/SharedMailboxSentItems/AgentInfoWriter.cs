using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.SharedMailboxSentItems
{
	internal class AgentInfoWriter : IAgentInfoWriter
	{
		public AgentInfoWriter(StoreDriverDeliveryEventArgs eventArgs, string agentName)
		{
			ArgumentValidator.ThrowIfNull("eventArgs", eventArgs);
			ArgumentValidator.ThrowIfNull("agentName", agentName);
			this.eventArgs = eventArgs;
			this.agentName = agentName;
		}

		public void AddAgentInfo(string eventName, string message)
		{
			ArgumentValidator.ThrowIfNull("message", message);
			List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("message", message)
			};
			this.eventArgs.AddAgentInfo(this.agentName, eventName, data);
		}

		private readonly StoreDriverDeliveryEventArgs eventArgs;

		private readonly string agentName;
	}
}
