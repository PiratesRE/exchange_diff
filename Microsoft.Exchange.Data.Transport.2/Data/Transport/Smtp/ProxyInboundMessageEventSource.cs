using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	internal abstract class ProxyInboundMessageEventSource : ReceiveEventSource
	{
		protected ProxyInboundMessageEventSource(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public abstract void SetProxyRoutingOverride(IEnumerable<INextHopServer> destinationServers, bool internalDestination);
	}
}
