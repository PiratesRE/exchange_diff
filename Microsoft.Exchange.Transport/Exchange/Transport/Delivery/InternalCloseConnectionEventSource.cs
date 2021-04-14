using System;
using Microsoft.Exchange.Data.Transport.Delivery;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Delivery
{
	internal class InternalCloseConnectionEventSource : CloseConnectionEventSource
	{
		public InternalCloseConnectionEventSource(DeliveryAgentMExEvents.DeliveryAgentMExSession mexSession, ulong sessionId, string remoteHost, NextHopConnection nextHopConnection, DeliveryAgentConnection.Stats stats)
		{
			this.source = new InternalDeliveryAgentEventSource(mexSession, null, sessionId, nextHopConnection, remoteHost, stats);
		}

		public bool ConnectionUnregistered
		{
			get
			{
				return this.source.ConnectionUnregistered;
			}
		}

		public override void UnregisterConnection(SmtpResponse smtpResponse)
		{
			this.source.UnregisterConnection(smtpResponse);
		}

		private InternalDeliveryAgentEventSource source;
	}
}
