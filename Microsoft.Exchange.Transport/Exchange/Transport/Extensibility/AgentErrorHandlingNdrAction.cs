using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Extensibility
{
	internal class AgentErrorHandlingNdrAction : IErrorHandlingAction
	{
		public AgentErrorHandlingNdrAction(SmtpResponse response)
		{
			this.Response = response;
		}

		public ErrorHandlingActionType ActionType
		{
			get
			{
				return ErrorHandlingActionType.NDR;
			}
		}

		public SmtpResponse Response { get; private set; }

		public void TakeAction(QueuedMessageEventSource source, MailItem mailItem)
		{
			EnvelopeRecipientCollection recipients = mailItem.Recipients;
			if (recipients == null || recipients.Count == 0)
			{
				ExTraceGlobals.ExtensibilityTracer.TraceInformation<string>(0, (long)this.GetHashCode(), "Error Handler: Mssage id '{0} has no recipients to NDR", mailItem.Message.MessageId);
				return;
			}
			TransportMailItemWrapper transportMailItemWrapper = mailItem as TransportMailItemWrapper;
			if (transportMailItemWrapper == null)
			{
				throw new ArgumentException("Can't map mailitem to a TransportMailItemWrapper");
			}
			TransportMailItem transportMailItem = transportMailItemWrapper.TransportMailItem;
			transportMailItem.SuppressBodyInDsn = true;
			for (int i = recipients.Count - 1; i >= 0; i--)
			{
				ExTraceGlobals.ExtensibilityTracer.TraceInformation<string, string>(0, (long)this.GetHashCode(), "Error Handler: dropping message id '{0} for recipient '{1}'", mailItem.Message.MessageId, recipients[i].Address.ToString());
				mailItem.Recipients.Remove(recipients[i], DsnType.Failure, this.Response, "Error triggered action.");
			}
		}
	}
}
