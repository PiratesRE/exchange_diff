using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Logging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class TlsSendConfiguration
	{
		public bool RequireTls { get; set; }

		public SmtpX509Identifier TlsCertificateName { get; set; }

		public string TlsCertificateFqdn { get; set; }

		public RequiredTlsAuthLevel? TlsAuthLevel { get; set; }

		public IList<SmtpDomainWithSubdomains> TlsDomains { get; set; }

		public bool ShouldSkipTls { get; set; }

		public TlsSendConfiguration(SmtpSendConnectorConfig connector, RequiredTlsAuthLevel? tlsOverride, string nextHopDomain, string nextHopTlsDomain)
		{
			if (tlsOverride != null)
			{
				this.TlsAuthLevel = tlsOverride;
				this.ShouldSkipTls = false;
				this.RequireTls = true;
				this.authLevelOverrideDescription = string.Format("Overriding connector TLS configuration: TlsAuthLevel -> {0} : {1}, IgnoreSTARTTLS -> {2} : {3}, RequireTLS -> {4} : {5}", new object[]
				{
					connector.TlsAuthLevel,
					this.TlsAuthLevel,
					connector.IgnoreSTARTTLS,
					this.ShouldSkipTls,
					connector.RequireTLS,
					this.RequireTls
				});
			}
			else
			{
				this.TlsAuthLevel = EnumConverter.InternalToPublic(connector.TlsAuthLevel);
				this.ShouldSkipTls = connector.IgnoreSTARTTLS;
				this.RequireTls = connector.RequireTLS;
			}
			this.TlsCertificateName = connector.TlsCertificateName;
			this.TlsCertificateFqdn = (string.IsNullOrEmpty(connector.CertificateSubject) ? connector.Fqdn : connector.CertificateSubject);
			this.ResolveTlsDomains(connector, nextHopDomain, nextHopTlsDomain);
		}

		public TlsSendConfiguration()
		{
			this.TlsAuthLevel = null;
			this.ShouldSkipTls = false;
			this.RequireTls = false;
			this.TlsDomains = null;
			this.TlsCertificateName = null;
			this.TlsCertificateFqdn = null;
		}

		public void LogTlsOverride(IProtocolLogSession logSession)
		{
			if (!string.IsNullOrEmpty(this.authLevelOverrideDescription))
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), this.authLevelOverrideDescription);
				logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, this.authLevelOverrideDescription);
			}
			if (!string.IsNullOrEmpty(this.domainOverrideDescription))
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), this.domainOverrideDescription);
				logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, this.domainOverrideDescription);
			}
		}

		private void ResolveTlsDomains(SmtpSendConnectorConfig connector, string nextHopDomain, string nextHopTlsDomain)
		{
			if (connector == null)
			{
				throw new ArgumentNullException("ResolveTlsDomains can only be invoked if connector has been initialized");
			}
			List<SmtpDomainWithSubdomains> list = new List<SmtpDomainWithSubdomains>();
			SmtpDomainWithSubdomains item;
			if (SmtpDomainWithSubdomains.TryParse(nextHopTlsDomain, out item))
			{
				list.Add(item);
				this.domainOverrideDescription = string.Format("Overriding connector TLS domain: {0}", nextHopTlsDomain);
			}
			else
			{
				SmtpDomainWithSubdomains tlsDomain = connector.TlsDomain;
				SmtpDomain domain;
				if (tlsDomain != null)
				{
					list.Add(tlsDomain);
				}
				else if (this.TlsAuthLevel != null && this.TlsAuthLevel.Value.Equals(RequiredTlsAuthLevel.DomainValidation) && SmtpDomain.TryParse(nextHopDomain, out domain))
				{
					list.Add(new SmtpDomainWithSubdomains(domain, true));
					list.Add(new SmtpDomainWithSubdomains(domain, false));
				}
			}
			this.TlsDomains = list;
		}

		private readonly string authLevelOverrideDescription;

		private string domainOverrideDescription;
	}
}
