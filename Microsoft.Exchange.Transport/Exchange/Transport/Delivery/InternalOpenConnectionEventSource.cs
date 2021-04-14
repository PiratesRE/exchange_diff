using System;
using Microsoft.Exchange.Data.Transport.Delivery;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Delivery
{
	internal class InternalOpenConnectionEventSource : OpenConnectionEventSource
	{
		public InternalOpenConnectionEventSource(DeliveryAgentMExEvents.DeliveryAgentMExSession mexSession, RoutedMailItemWrapper deliverableMailItem, ulong sessionId, NextHopConnection nextHopConnection, DeliveryAgentConnection.Stats stats)
		{
			this.source = new InternalDeliveryAgentEventSource(mexSession, deliverableMailItem, sessionId, nextHopConnection, null, stats);
		}

		public InternalDeliveryAgentEventSource InternalEventSource
		{
			get
			{
				return this.source;
			}
		}

		public override void FailQueue(SmtpResponse smtpResponse)
		{
			this.source.FailQueue(smtpResponse);
		}

		public override void DeferQueue(SmtpResponse smtpResponse)
		{
			this.source.DeferQueue(smtpResponse);
		}

		public override void DeferQueue(SmtpResponse smtpResponse, TimeSpan interval)
		{
			this.source.DeferQueue(smtpResponse, interval);
		}

		public override void RegisterConnection(string remoteHost, SmtpResponse smtpResponse)
		{
			this.source.RegisterConnection(remoteHost, smtpResponse);
		}

		private InternalDeliveryAgentEventSource source;
	}
}
