using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders
{
	public class SetMonitorStateRepairingResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, string monitorName, ServiceHealthStatus targetHealthState, bool enabled = true, int recurrenceIntervalSeconds = 300)
		{
			return new ResponderDefinition
			{
				AssemblyPath = SetMonitorStateRepairingResponder.AssemblyPath,
				TypeName = SetMonitorStateRepairingResponder.TypeName,
				Name = name,
				ServiceName = serviceName,
				AlertTypeId = alertTypeId,
				AlertMask = alertMask,
				TargetResource = targetResource,
				TargetPartition = monitorName,
				TargetHealthState = targetHealthState,
				RecurrenceIntervalSeconds = recurrenceIntervalSeconds,
				TimeoutSeconds = (int)SetMonitorStateRepairingResponder.DefaultTimeout.TotalSeconds,
				MaxRetryAttempts = 3,
				Enabled = enabled
			};
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			string targetPartition = base.Definition.TargetPartition;
			string targetResource = base.Definition.TargetResource;
			if (string.IsNullOrWhiteSpace(targetPartition))
			{
				throw new ArgumentNullException("MonitorName");
			}
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Setting monitor {0} for target resource {1} into repairing", targetPartition, targetResource, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\SetMonitorStateRepairingResponder.cs", 97);
			RpcSetServerMonitor.Invoke(Environment.MachineName, targetPartition, targetResource, new bool?(true), 30000);
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Successfully set monitor {0} for target resource {1} into repairing", targetPartition, targetResource, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\SetMonitorStateRepairingResponder.cs", 109);
		}

		private static TimeSpan DefaultTimeout = TimeSpan.FromMinutes(2.0);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(SetMonitorStateRepairingResponder).FullName;
	}
}
