using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal struct EnhancedDnsHostsResult
	{
		public EnhancedDnsHostsResult(EnhancedDnsTargetHost[] hosts, EnhancedDnsRequestContext requestContext)
		{
			this.Hosts = hosts;
			this.RequestContext = requestContext;
		}

		public readonly EnhancedDnsTargetHost[] Hosts;

		public readonly EnhancedDnsRequestContext RequestContext;
	}
}
