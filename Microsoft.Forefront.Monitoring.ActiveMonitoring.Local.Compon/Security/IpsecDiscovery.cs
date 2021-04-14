using System;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Security
{
	public sealed class IpsecDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			bool flag = FfoLocalEndpointManager.IsForefrontForOfficeDatacenter && (FfoLocalEndpointManager.IsBackgroundRoleInstalled || FfoLocalEndpointManager.IsDomainNameServerRoleInstalled || FfoLocalEndpointManager.IsFrontendTransportRoleInstalled || FfoLocalEndpointManager.IsHubTransportRoleInstalled || FfoLocalEndpointManager.IsWebServiceInstalled);
			if (flag)
			{
				GenericWorkItemHelper.CreateAllDefinitions(new string[]
				{
					"IpsecProbes.xml"
				}, base.Broker, base.TraceContext, base.Result);
			}
		}
	}
}
