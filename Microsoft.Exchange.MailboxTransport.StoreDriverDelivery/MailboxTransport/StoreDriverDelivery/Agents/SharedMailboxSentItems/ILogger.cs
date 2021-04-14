using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.SharedMailboxSentItems
{
	internal interface ILogger
	{
		void LogEvent(ExEventLog.EventTuple eventTuple, Exception exception);

		void TraceDebug(params string[] messagesStrings);
	}
}
