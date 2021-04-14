using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Monitors
{
	public class MaintenanceAssistantMonitor : NotificationHeartbeatMonitor
	{
		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			string targetExtension = base.Definition.TargetExtension;
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Starting maintenance assistant check against database {0}", targetExtension, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\MaintenanceAssistantMonitor.cs", 29);
			Guid databaseGuid = new Guid(targetExtension);
			if (!DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(databaseGuid))
			{
				base.Result.StateAttribute1 = base.Result.ExecutionStartTime.ToString();
				base.Result.IsAlert = false;
				return;
			}
			base.DoMonitorWork(cancellationToken);
		}
	}
}
