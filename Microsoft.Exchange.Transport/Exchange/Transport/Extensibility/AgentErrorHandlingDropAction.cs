using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.Transport.Extensibility
{
	internal class AgentErrorHandlingDropAction : IErrorHandlingAction
	{
		public ErrorHandlingActionType ActionType
		{
			get
			{
				return ErrorHandlingActionType.Drop;
			}
		}

		public void TakeAction(QueuedMessageEventSource source, MailItem mailItem)
		{
			source.Delete();
		}
	}
}
