using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpOutConnectionHandler
	{
		void HandleConnection(NextHopConnection connection);

		void HandleProxyConnection(NextHopConnection connection, IEnumerable<INextHopServer> proxyDestinations, bool internalDestination, string connectionContextString);

		SmtpOutConnection NewBlindProxyConnection(NextHopConnection connection, IEnumerable<INextHopServer> proxyDestinations, bool clientProxy, SmtpSendConnectorConfig connector, TlsSendConfiguration tlsSendConfiguration, RiskLevel riskLevel, int outboundIPPool, int connectionAttempts, ISmtpInSession inSession, string connectionContextString);

		void HandleShadowConnection(NextHopConnection connection, IEnumerable<INextHopServer> shadowHubs);
	}
}
