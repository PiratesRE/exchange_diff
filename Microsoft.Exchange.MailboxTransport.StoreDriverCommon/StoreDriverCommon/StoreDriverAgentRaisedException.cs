using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	internal class StoreDriverAgentRaisedException : LocalizedException
	{
		public StoreDriverAgentRaisedException(Exception actualAgentException) : base(new LocalizedString("Wrapper class for exceptions that agents throw"), actualAgentException)
		{
		}

		public StoreDriverAgentRaisedException(string agentName, Exception actualAgentException) : this(actualAgentException)
		{
			this.AgentName = agentName;
		}

		public string AgentName { get; private set; }
	}
}
