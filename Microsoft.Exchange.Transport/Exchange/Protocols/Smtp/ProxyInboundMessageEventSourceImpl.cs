using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class ProxyInboundMessageEventSourceImpl : ProxyInboundMessageEventSource
	{
		private ProxyInboundMessageEventSourceImpl(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public IEnumerable<INextHopServer> DestinationServers { get; private set; }

		public bool InternalDestination { get; private set; }

		public static ProxyInboundMessageEventSourceImpl Create(SmtpSession smtpSession)
		{
			return new ProxyInboundMessageEventSourceImpl(smtpSession);
		}

		public override void SetProxyRoutingOverride(IEnumerable<INextHopServer> destinationServers, bool internalDestination)
		{
			this.DestinationServers = destinationServers;
			this.InternalDestination = internalDestination;
		}

		public override CertificateValidationStatus ValidateCertificate()
		{
			return base.SmtpSession.ValidateCertificate();
		}

		public override CertificateValidationStatus ValidateCertificate(string domain, out string matchedCertDomain)
		{
			return base.SmtpSession.ValidateCertificate(domain, out matchedCertDomain);
		}

		public override void Disconnect()
		{
			base.SmtpSession.Disconnect();
		}
	}
}
