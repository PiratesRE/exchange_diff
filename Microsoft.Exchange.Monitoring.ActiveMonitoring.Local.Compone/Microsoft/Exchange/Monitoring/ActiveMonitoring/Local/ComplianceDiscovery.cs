using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	public sealed class ComplianceDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				if (ExEnvironment.IsTest)
				{
					GenericWorkItemHelper.CreateAllDefinitions(new List<string>
					{
						"ComplianceDefinitions.xml",
						"eDiscoveryDefinitions.xml",
						"HoldDefinitionsForTest.xml"
					}, base.Broker, base.TraceContext, base.Result);
					return;
				}
				GenericWorkItemHelper.CreateAllDefinitions(new List<string>
				{
					"ComplianceDefinitions.xml",
					"eDiscoveryDefinitions.xml",
					"HoldDefinitions.xml"
				}, base.Broker, base.TraceContext, base.Result);
			}
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
