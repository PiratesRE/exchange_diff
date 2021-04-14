using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class XProxyToParseOutput
	{
		public string DecodedCertificateSubject { get; set; }

		public List<INextHopServer> Destinations { get; set; }

		public ErrorPolicies? ErrorPolicies { get; set; }

		public bool? ForceHelo { get; set; }

		public Fqdn Fqdn { get; set; }

		public bool IsDnsRoutingEnabled { get; set; }

		public bool? IsLast { get; set; }

		public bool IsProbeConnection { get; set; }

		public string NextHopDomain { get; set; }

		public int? OutboundIPPool { get; set; }

		public int? Port { get; set; }

		public bool? RequireOorg { get; set; }

		public bool? RequireTls { get; set; }

		public RiskLevel? Risk { get; set; }

		public string SessionId { get; set; }

		public bool? ShouldSkipTls { get; set; }

		public RequiredTlsAuthLevel? TlsAuthLevel { get; set; }

		public List<SmtpDomainWithSubdomains> TlsDomains { get; set; }
	}
}
