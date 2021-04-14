using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.FfoSelfRecoveryFx;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.SyslogListener
{
	public sealed class SyslogListenerDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsSyslogListenerRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RAAServiceTracer, SyslogListenerDiscovery.traceContext, "[SyslogListenerDiscovery.DoWork]: SyslogListener role is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\SyslogListener\\SyslogListenerDiscovery.cs", 38);
				base.Result.StateAttribute1 = "SyslogListenerDiscovery: SyslogListener role is not installed on this server.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"SyslogListener.xml"
			}, base.Broker, base.TraceContext, base.Result);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RAAServiceTracer, SyslogListenerDiscovery.traceContext, "[SyslogListenerDiscovery.DoWork]: SyslogListenerDiscovery work item definitions created.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\SyslogListener\\SyslogListenerDiscovery.cs", 57);
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
