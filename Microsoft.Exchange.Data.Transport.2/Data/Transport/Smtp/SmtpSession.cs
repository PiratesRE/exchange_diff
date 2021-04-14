using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public abstract class SmtpSession
	{
		internal SmtpSession()
		{
		}

		public abstract string HelloDomain { get; internal set; }

		public abstract IPEndPoint LocalEndPoint { get; }

		public abstract IPEndPoint RemoteEndPoint { get; internal set; }

		public abstract IDictionary<string, object> Properties { get; }

		public abstract long SessionId { get; }

		public abstract bool IsConnected { get; }

		public abstract IPAddress LastExternalIPAddress { get; internal set; }

		public abstract AuthenticationSource AuthenticationSource { get; }

		public abstract bool AntispamBypass { get; }

		public abstract bool IsExternalConnection { get; internal set; }

		public abstract bool IsTls { get; }

		internal abstract bool DiscardingMessage { get; }

		internal abstract bool ShouldDisconnect { get; set; }

		internal abstract bool IsInboundProxiedSession { get; set; }

		internal abstract bool IsClientProxiedSession { get; set; }

		internal abstract bool XAttrAdvertised { get; }

		internal abstract string ReceiveConnectorName { get; }

		internal abstract X509Certificate2 TlsRemoteCertificate { get; }

		internal abstract SmtpResponse Banner { get; set; }

		internal abstract SmtpResponse SmtpResponse { get; set; }

		internal abstract DisconnectReason DisconnectReason { get; set; }

		internal abstract IExecutionControl ExecutionControl { get; set; }

		internal abstract string CurrentMessageTemporaryId { get; }

		internal abstract bool DisableStartTls { get; set; }

		internal abstract bool RequestClientTlsCertificate { get; set; }

		internal abstract void RejectMessage(SmtpResponse response);

		internal abstract void RejectMessage(SmtpResponse response, string trackingContext);

		internal abstract void DiscardMessage(SmtpResponse response, string trackingContext);

		internal abstract void Disconnect();

		internal abstract CertificateValidationStatus ValidateCertificate();

		internal abstract CertificateValidationStatus ValidateCertificate(string domain, out string matchedCertDomain);

		internal abstract void GrantMailItemPermissions(Permission permissionsToGrant);
	}
}
