using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.Transport.Extensibility
{
	internal class AgentErrorHandlingAllowAction : IErrorHandlingAction
	{
		public ErrorHandlingActionType ActionType
		{
			get
			{
				return ErrorHandlingActionType.Allow;
			}
		}

		public void TakeAction(QueuedMessageEventSource source, MailItem mailItem)
		{
		}
	}
}
