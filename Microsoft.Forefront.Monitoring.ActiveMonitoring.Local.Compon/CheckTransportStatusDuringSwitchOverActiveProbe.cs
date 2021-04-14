using System;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class CheckTransportStatusDuringSwitchOverActiveProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionContext = "CheckTransportStatusDuringSwitchOverActiveProbe started.";
			ServiceState effectiveState = ServerComponentStateManager.GetEffectiveState(ServerComponentEnum.HubTransport);
			bool databaseCopyActivationDisabledAndMoveNow = CachedAdReader.Instance.LocalServer.DatabaseCopyActivationDisabledAndMoveNow;
			base.Result.StateAttribute1 = string.Format("ServerComponent state for HubTransport is :{0}", effectiveState);
			base.Result.StateAttribute2 = string.Format("SwitchOverActive state for the server is set to :{0}", databaseCopyActivationDisabledAndMoveNow);
			base.Result.StateAttribute6 = (double)effectiveState;
			base.Result.StateAttribute7 = (double)(databaseCopyActivationDisabledAndMoveNow ? 1 : 0);
			if (effectiveState == ServiceState.Active && databaseCopyActivationDisabledAndMoveNow)
			{
				string message = "Server Component State for hubtransport is active while SwitchOverActive for the server is set to true";
				WTFDiagnostics.TraceDebug(ExTraceGlobals.MonitoringTracer, base.TraceContext, message, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\CheckTransportStatusDuringSwitchOverActiveProbe.cs", 53);
				ProbeResult result = base.Result;
				result.ExecutionContext += "HubTransport has a Invalid servercomponent state with respect to SwitchOverActive state.";
				throw new Exception(message);
			}
			ProbeResult result2 = base.Result;
			result2.ExecutionContext += "HubTransport had a valid servercomponent state with respect to SwitchOverActive state.";
		}
	}
}
