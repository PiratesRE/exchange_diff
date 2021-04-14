using System;
using System.Net;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal struct EnhancedDnsStatusResult
	{
		public EnhancedDnsStatusResult(DnsStatus status, IPAddress server, EnhancedDnsRequestContext requestContext, string diagnosticInfo)
		{
			this.Status = status;
			this.Server = server;
			this.RequestContext = requestContext;
			this.DiagnosticInfo = diagnosticInfo;
		}

		public readonly DnsStatus Status;

		public readonly IPAddress Server;

		public readonly EnhancedDnsRequestContext RequestContext;

		public readonly string DiagnosticInfo;
	}
}
