using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class SmtpSessionImpl : SmtpSession
	{
		public SmtpSessionImpl(ISmtpInSession internalSmtpSession, INetworkConnection networkConnection, bool isExternalConnection)
		{
			ArgumentValidator.ThrowIfNull("internalSmtpSession", internalSmtpSession);
			ArgumentValidator.ThrowIfNull("networkConnection", networkConnection);
			this.networkConnection = networkConnection;
			this.SmtpResponse = SmtpResponse.Empty;
			this.internalSmtpSession = internalSmtpSession;
			this.RemoteEndPoint = networkConnection.RemoteEndPoint;
			this.IsExternalConnection = isExternalConnection;
			this.LastExternalIPAddress = (isExternalConnection ? networkConnection.RemoteEndPoint.Address : null);
			this.sessionId = Microsoft.Exchange.Transport.SessionId.GetNextSessionId();
		}

		public override string HelloDomain { get; internal set; }

		public override IPEndPoint LocalEndPoint
		{
			get
			{
				return this.networkConnection.LocalEndPoint;
			}
		}

		public override IPEndPoint RemoteEndPoint { get; internal set; }

		public override IDictionary<string, object> Properties
		{
			get
			{
				return this.properties;
			}
		}

		public override long SessionId
		{
			get
			{
				return (long)this.sessionId;
			}
		}

		public override bool IsConnected
		{
			get
			{
				return !this.ShouldDisconnect;
			}
		}

		public override bool IsExternalConnection { get; internal set; }

		public override IPAddress LastExternalIPAddress { get; internal set; }

		public override AuthenticationSource AuthenticationSource
		{
			get
			{
				return this.internalSmtpSession.AuthenticationSourceForAgents;
			}
		}

		public override bool AntispamBypass
		{
			get
			{
				return SmtpInSessionUtils.HasSMTPAntiSpamBypassPermission(this.internalSmtpSession.Permissions);
			}
		}

		public override bool IsTls
		{
			get
			{
				return this.internalSmtpSession.IsTls;
			}
		}

		internal override bool DiscardingMessage
		{
			get
			{
				return this.internalSmtpSession.DiscardingMessage;
			}
		}

		internal override bool ShouldDisconnect { get; set; }

		internal override bool IsInboundProxiedSession { get; set; }

		internal override bool IsClientProxiedSession { get; set; }

		internal override bool XAttrAdvertised
		{
			get
			{
				return this.internalSmtpSession.AdvertisedEhloOptions.XAttr;
			}
		}

		internal override string ReceiveConnectorName
		{
			get
			{
				return this.internalSmtpSession.Connector.Name;
			}
		}

		internal override X509Certificate2 TlsRemoteCertificate
		{
			get
			{
				return this.internalSmtpSession.TlsRemoteCertificate;
			}
		}

		internal override SmtpResponse Banner { get; set; }

		internal override SmtpResponse SmtpResponse { get; set; }

		internal override DisconnectReason DisconnectReason { get; set; }

		internal override IExecutionControl ExecutionControl { get; set; }

		internal override string CurrentMessageTemporaryId
		{
			get
			{
				return this.internalSmtpSession.CurrentMessageTemporaryId;
			}
		}

		internal override bool DisableStartTls
		{
			get
			{
				return this.internalSmtpSession.DisableStartTls;
			}
			set
			{
				this.internalSmtpSession.DisableStartTls = value;
			}
		}

		internal override bool RequestClientTlsCertificate
		{
			get
			{
				return this.internalSmtpSession.ForceRequestClientTlsCertificate;
			}
			set
			{
				this.internalSmtpSession.ForceRequestClientTlsCertificate = value;
			}
		}

		internal override void GrantMailItemPermissions(Permission permissionsToGrant)
		{
			this.internalSmtpSession.GrantMailItemPermissions(permissionsToGrant);
		}

		internal override void RejectMessage(SmtpResponse response)
		{
			this.RejectMessage(response, null);
		}

		internal override void RejectMessage(SmtpResponse response, string sourceContext)
		{
			this.SmtpResponse = response;
			SmtpSessionHelper.RejectMessage(response, sourceContext, this.ExecutionControl, this.internalSmtpSession.TransportMailItem, this.internalSmtpSession.LocalEndPoint, this.internalSmtpSession.RemoteEndPoint, this.internalSmtpSession.SessionId, this.internalSmtpSession.Connector, this.internalSmtpSession.LogSession, this.messageTrackingLogWrapper);
		}

		internal override void DiscardMessage(SmtpResponse response, string sourceContext)
		{
			if (response.SmtpResponseType != SmtpResponseType.Success)
			{
				throw new InvalidOperationException("Response provided must be a success (2xx) one. If you want to reject, call RejectMessage instead");
			}
			this.SmtpResponse = response;
			SmtpSessionHelper.DiscardMessage(sourceContext, this.ExecutionControl, this.internalSmtpSession.TransportMailItem, this.internalSmtpSession.LogSession, this.messageTrackingLogWrapper);
		}

		internal override void Disconnect()
		{
			this.ShouldDisconnect = true;
			this.ExecutionControl.HaltExecution();
		}

		internal override CertificateValidationStatus ValidateCertificate()
		{
			this.ThrowIfNotTls();
			return SmtpSessionHelper.ConvertChainValidityStatusToCertValidationStatus(this.internalSmtpSession.TlsRemoteCertificateChainValidationStatus);
		}

		internal override CertificateValidationStatus ValidateCertificate(string domain, out string matchedCertDomain)
		{
			this.ThrowIfNotTls();
			return SmtpSessionHelper.ValidateCertificate(domain, this.internalSmtpSession.TlsRemoteCertificate, this.internalSmtpSession.SecureState, this.internalSmtpSession.TlsRemoteCertificateChainValidationStatus, this.internalSmtpSession.SmtpInServer.CertificateValidator, this.internalSmtpSession.LogSession, out matchedCertDomain);
		}

		private void ThrowIfNotTls()
		{
			if (!this.internalSmtpSession.IsTls)
			{
				throw new InvalidOperationException("GetCertificateValidationStatus can only be invoked for TLS session.");
			}
		}

		internal const string DateTimeFormat = "yyyy-MM-ddTHH\\:mm\\:ss.fffZ";

		private readonly ISmtpInSession internalSmtpSession;

		private readonly INetworkConnection networkConnection;

		private readonly IDictionary<string, object> properties = new Dictionary<string, object>();

		private readonly ulong sessionId;

		private readonly MessageTrackingLogWrapper messageTrackingLogWrapper = new MessageTrackingLogWrapper();
	}
}
