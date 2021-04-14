using System;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Agent.SharedMailboxSentItemsRoutingAgent
{
	internal sealed class EventSource : IEventSource
	{
		public EventSource(SubmittedMessageEventSource source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.source = source;
		}

		public void Defer(TimeSpan delayTime, SmtpResponse response)
		{
			this.source.Defer(delayTime, response);
		}

		private SubmittedMessageEventSource source;
	}
}
