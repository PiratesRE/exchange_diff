using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Agent.SharedMailboxSentItemsRoutingAgent
{
	internal interface IEventSource
	{
		void Defer(TimeSpan delayTime, SmtpResponse response);
	}
}
