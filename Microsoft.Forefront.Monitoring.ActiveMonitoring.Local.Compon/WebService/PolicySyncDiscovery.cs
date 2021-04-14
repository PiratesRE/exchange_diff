using System;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebService
{
	public sealed class PolicySyncDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (FfoLocalEndpointManager.IsWebServiceInstalled)
			{
				GenericWorkItemHelper.CreateAllDefinitions(new string[]
				{
					"PolicySyncProbes.xml"
				}, base.Broker, base.TraceContext, base.Result);
			}
		}
	}
}
