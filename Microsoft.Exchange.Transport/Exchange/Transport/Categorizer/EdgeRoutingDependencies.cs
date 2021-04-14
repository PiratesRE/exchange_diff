using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class EdgeRoutingDependencies
	{
		public EdgeRoutingDependencies(ITransportConfiguration transportConfig)
		{
			this.transportConfig = transportConfig;
		}

		public virtual ICollection<string> EdgeToHubAcceptedDomains
		{
			get
			{
				RoutingUtils.ThrowIfMissingDependency(this.transportConfig, "Configuration");
				return this.transportConfig.FirstOrgAcceptedDomainTable.EdgeToBHDomains;
			}
		}

		public virtual bool IsLocalServerId(ADObjectId serverId)
		{
			RoutingUtils.ThrowIfMissingDependency(this.transportConfig, "Configuration");
			return this.transportConfig.LocalServer.TransportServer.Id.Equals(serverId);
		}

		public virtual void RegisterForAcceptedDomainChanges(ConfigurationUpdateHandler<AcceptedDomainTable> handler)
		{
			RoutingUtils.ThrowIfMissingDependency(this.transportConfig, "Configuration");
			this.transportConfig.FirstOrgAcceptedDomainTableChanged += handler;
		}

		public virtual void UnregisterFromAcceptedDomainChanges(ConfigurationUpdateHandler<AcceptedDomainTable> handler)
		{
			RoutingUtils.ThrowIfMissingDependency(this.transportConfig, "Configuration");
			this.transportConfig.FirstOrgAcceptedDomainTableChanged -= handler;
		}

		private ITransportConfiguration transportConfig;
	}
}
