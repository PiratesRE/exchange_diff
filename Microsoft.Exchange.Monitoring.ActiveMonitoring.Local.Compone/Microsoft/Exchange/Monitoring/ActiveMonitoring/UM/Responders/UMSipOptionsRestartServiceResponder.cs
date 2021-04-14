using System;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM.Responders
{
	internal class UMSipOptionsRestartServiceResponder : RestartServiceResponder
	{
		public static ResponderDefinition CreateUMSipOptionRestartServiceResponder(string responderName, string alertMask, string targetResource, string healthSet, string windowsServiceName, int recurrenceIntervalSeconds, int timeoutSeconds, int waitIntervalSeconds, int retryAttempts, int serviceStopTimeoutSeconds, int serviceStartTimeoutSeconds, ServiceHealthStatus monitorHealthStatus, TracingContext traceContext, string throttleGroupName)
		{
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.UnifiedMessagingTracer, traceContext, "UMDiscovery:: DoWork(): Creating {0} for {1}", responderName, targetResource, null, "CreateUMSipOptionRestartServiceResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Responders\\UMSipOptionsRestartServiceResponder.cs", 56);
			ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition(responderName, alertMask, windowsServiceName, monitorHealthStatus, 15, 120, 0, false, DumpMode.None, null, 15.0, 0, "Exchange", null, true, true, throttleGroupName, false);
			responderDefinition.AssemblyPath = UMMonitoringConstants.AssemblyPath;
			responderDefinition.TypeName = typeof(UMSipOptionsRestartServiceResponder).FullName;
			responderDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			responderDefinition.TimeoutSeconds = timeoutSeconds;
			responderDefinition.MaxRetryAttempts = retryAttempts;
			responderDefinition.AlertTypeId = string.Format("Exchange/UM/{0}", responderName.Replace(' ', '\0'));
			responderDefinition.AlertMask = alertMask;
			responderDefinition.WaitIntervalSeconds = waitIntervalSeconds;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.ServiceName = healthSet;
			responderDefinition.Attributes["ServiceStopTimeout"] = TimeSpan.FromSeconds((double)serviceStopTimeoutSeconds).ToString();
			responderDefinition.Attributes["ServiceStartTimeout"] = TimeSpan.FromSeconds((double)serviceStartTimeoutSeconds).ToString();
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.UnifiedMessagingTracer, traceContext, "UMDiscovery:: DoWork(): Created {0} for {1}", responderName, targetResource, null, "CreateUMSipOptionRestartServiceResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Responders\\UMSipOptionsRestartServiceResponder.cs", 84);
			return responderDefinition;
		}
	}
}
