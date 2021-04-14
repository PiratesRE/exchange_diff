using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ManagementEndpoint
{
	public sealed class ManagementEndpoint
	{
		public SmtpDomain DomainName { get; internal set; }

		public Guid ExternalDirectoryOrganizationId { get; internal set; }

		public string RemotePowershellUrl { get; internal set; }

		public string ResourceForest { get; internal set; }

		public string AccountPartition { get; internal set; }

		public string SmtpNextHopDomain { get; internal set; }

		public ManagementEndpointVersion Version { get; private set; }

		public ManagementEndpoint(string remotePowershellUrl, ManagementEndpointVersion version)
		{
			this.Version = version;
			this.RemotePowershellUrl = remotePowershellUrl;
		}
	}
}
