using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class DalDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsBackgroundRoleInstalled && !FfoLocalEndpointManager.IsDomainNameServerRoleInstalled && !FfoLocalEndpointManager.IsFrontendTransportRoleInstalled && !FfoLocalEndpointManager.IsHubTransportRoleInstalled && !FfoLocalEndpointManager.IsWebServiceInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DALTracer, DalDiscovery.traceContext, "[DalDiscovery.DoWork]: DAL is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DAL\\DalDiscovery.cs", 47);
				base.Result.StateAttribute1 = "DalDiscovery: DAL is not installed on this server.";
				return;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DALTracer, DalDiscovery.traceContext, "[DalDiscovery.DoWork]: DAL is installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DAL\\DalDiscovery.cs", 56);
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"Dal.xml",
				"Common_HygieneDAL.xml"
			}, base.Broker, base.TraceContext, base.Result);
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
