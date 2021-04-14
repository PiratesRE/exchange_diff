using System;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public sealed class FederationTrustCertificateStatus
	{
		public ADSite Site
		{
			get
			{
				return this.site;
			}
		}

		internal TopologyServer Server
		{
			get
			{
				return this.server;
			}
		}

		public FederationTrustCertificateState State
		{
			get
			{
				return this.state;
			}
		}

		public string Thumbprint
		{
			get
			{
				return this.thumbprint;
			}
		}

		internal FederationTrustCertificateStatus(ADSite site, TopologyServer server, FederationTrustCertificateState state, string thumbprint)
		{
			this.site = site;
			this.server = server;
			this.state = state;
			this.thumbprint = thumbprint;
		}

		private ADSite site;

		private TopologyServer server;

		private FederationTrustCertificateState state;

		private readonly string thumbprint;
	}
}
