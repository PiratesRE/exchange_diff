using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public abstract class ReceiveMessageEventSource : ReceiveEventSource
	{
		internal ReceiveMessageEventSource(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public abstract void RejectMessage(SmtpResponse response);

		public abstract void RejectMessage(SmtpResponse response, string trackingContext);

		public abstract void DiscardMessage(SmtpResponse response, string trackingContext);

		public abstract void Quarantine(IEnumerable<EnvelopeRecipient> recipients, string quarantineReason);
	}
}
