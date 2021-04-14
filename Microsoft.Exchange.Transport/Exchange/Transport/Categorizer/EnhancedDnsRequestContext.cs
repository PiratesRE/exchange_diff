using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal struct EnhancedDnsRequestContext
	{
		public EnhancedDnsRequestContext(SmtpSendConnectorConfig destinationConnector)
		{
			this = new EnhancedDnsRequestContext(null, destinationConnector, null);
		}

		public EnhancedDnsRequestContext(IEnumerable<INextHopServer> destinationServers, SmtpSendConnectorConfig destinationConnector, SmtpSendConnectorConfig proxyConnector)
		{
			this.DestinationServers = destinationServers;
			this.DestinationConnector = destinationConnector;
			this.ProxyConnector = proxyConnector;
		}

		public readonly IEnumerable<INextHopServer> DestinationServers;

		public readonly SmtpSendConnectorConfig DestinationConnector;

		public readonly SmtpSendConnectorConfig ProxyConnector;
	}
}
