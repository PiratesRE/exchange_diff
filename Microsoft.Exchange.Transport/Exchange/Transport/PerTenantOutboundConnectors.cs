using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport
{
	internal class PerTenantOutboundConnectors : TenantConfigurationCacheableItem<TenantOutboundConnector>
	{
		public TenantOutboundConnector[] TenantOutboundConnectors { get; private set; }

		public override long ItemSize
		{
			get
			{
				return (long)((this.TenantOutboundConnectors != null) ? this.TenantOutboundConnectors.Length : 0);
			}
		}

		public override void ReadData(IConfigurationSession tenantConfigurationSession)
		{
			IEnumerable<TenantOutboundConnector> enumerable;
			ADOperationResult outboundConnectors = ConnectorConfiguration.GetOutboundConnectors(tenantConfigurationSession, (TenantOutboundConnector toc) => !toc.IsTransportRuleScoped, out enumerable);
			if (!outboundConnectors.Succeeded)
			{
				throw new TenantOutboundConnectorsRetrievalException(outboundConnectors);
			}
			this.TenantOutboundConnectors = ((enumerable != null) ? enumerable.ToArray<TenantOutboundConnector>() : null);
		}
	}
}
