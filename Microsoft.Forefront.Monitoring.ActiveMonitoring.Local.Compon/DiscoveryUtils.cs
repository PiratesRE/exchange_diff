using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	internal static class DiscoveryUtils
	{
		public static bool IsFrontendTransportRoleInstalled()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			return (instance.ExchangeServerRoleEndpoint == null && FfoLocalEndpointManager.IsFrontendTransportRoleInstalled) || (instance.ExchangeServerRoleEndpoint != null && instance.ExchangeServerRoleEndpoint.IsFrontendTransportRoleInstalled);
		}

		public static bool IsHubTransportRoleInstalled()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			return (instance.ExchangeServerRoleEndpoint == null && FfoLocalEndpointManager.IsHubTransportRoleInstalled) || (instance.ExchangeServerRoleEndpoint != null && instance.ExchangeServerRoleEndpoint.IsBridgeheadRoleInstalled);
		}

		public static bool IsGatewayRoleInstalled()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			return instance.ExchangeServerRoleEndpoint != null && instance.ExchangeServerRoleEndpoint.IsGatewayRoleInstalled;
		}

		public static bool IsMailboxRoleInstalled()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			return instance.ExchangeServerRoleEndpoint != null && instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled;
		}

		public static bool IsForefrontForOfficeDatacenter()
		{
			return DatacenterRegistry.IsForefrontForOffice();
		}

		public static bool IsMultiTenancyEnabled()
		{
			return VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled || DiscoveryUtils.IsForefrontForOfficeDatacenter() || DatacenterRegistry.IsPartnerHostedOnly();
		}
	}
}
