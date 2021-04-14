using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport
{
	internal interface IAgentRuntime
	{
		ISmtpAgentSession NewSmtpAgentSession(ISmtpInSession smtpInSession, INetworkConnection networkConnection, bool isExternalConnection);

		ISmtpAgentSession NewSmtpAgentSession(SmtpInSessionState sessionState, IIsMemberOfResolver<RoutingAddress> isMemberOfResolver, AcceptedDomainCollection firstOrgAcceptedDomains, RemoteDomainCollection remoteDomains, ServerVersion adminDisplayVersion, out IMExSession mexSession);

		AcceptedDomainCollection FirstOrgAcceptedDomains { get; }

		RemoteDomainCollection RemoteDomains { get; }
	}
}
