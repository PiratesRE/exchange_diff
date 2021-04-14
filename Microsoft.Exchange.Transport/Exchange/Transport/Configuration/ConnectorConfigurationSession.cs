using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal class ConnectorConfigurationSession
	{
		public ConnectorConfigurationSession(OrganizationId orgId)
		{
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				throw new InvalidOperationException("Multi-tenant deployments supported only.");
			}
			if (orgId == null)
			{
				throw new ArgumentException("A tenant id must be specified.");
			}
			this.orgId = orgId;
		}

		public static TenantOutboundConnector GetEnabledOutboundConnector(OrganizationId orgId, string connectorName)
		{
			ConnectorConfigurationSession connectorConfigurationSession = new ConnectorConfigurationSession(orgId);
			return connectorConfigurationSession.GetEnabledOutboundConnector(connectorName);
		}

		public static ADOperationResult TryGetEnabledOutboundConnectorByGuid(OrganizationId orgId, Guid connectorGuid, out TenantOutboundConnector matchingConnector)
		{
			IEnumerable<TenantOutboundConnector> enumerable;
			ADOperationResult outboundConnectors = ConnectorConfiguration.GetOutboundConnectors(orgId, (TenantOutboundConnector toc) => toc.IsTransportRuleScoped && connectorGuid.Equals(toc.Guid) && toc.Enabled, out enumerable);
			matchingConnector = null;
			if (outboundConnectors.Succeeded && enumerable != null)
			{
				matchingConnector = enumerable.FirstOrDefault<TenantOutboundConnector>();
			}
			return outboundConnectors;
		}

		private TenantOutboundConnector GetEnabledOutboundConnector(string connectorName)
		{
			IEnumerable<TenantOutboundConnector> enumerable;
			ADOperationResult outboundConnectors = ConnectorConfiguration.GetOutboundConnectors(this.orgId, (TenantOutboundConnector toc) => toc.IsTransportRuleScoped && string.Equals(connectorName, toc.Name, StringComparison.OrdinalIgnoreCase) && toc.Enabled, out enumerable);
			TenantOutboundConnector result;
			if (!outboundConnectors.Succeeded || enumerable == null || (result = enumerable.FirstOrDefault<TenantOutboundConnector>()) == null)
			{
				throw new OutboundConnectorNotFoundException(connectorName, this.orgId, outboundConnectors.Exception);
			}
			return result;
		}

		private readonly OrganizationId orgId;
	}
}
