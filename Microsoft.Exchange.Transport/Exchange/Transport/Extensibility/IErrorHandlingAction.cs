using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.Transport.Extensibility
{
	internal interface IErrorHandlingAction
	{
		ErrorHandlingActionType ActionType { get; }

		void TakeAction(QueuedMessageEventSource source, MailItem mailItem);
	}
}
