using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders
{
	public class EscalationNotificationResponder : ResponderWorkItem
	{
		internal ProbeResult LastFailedProbeResult { get; private set; }

		internal MonitorResult LastFailedMonitorResult { get; private set; }

		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, bool enabled = true, int recurrenceIntervalSeconds = 300)
		{
			return new ResponderDefinition
			{
				AssemblyPath = EscalationNotificationResponder.AssemblyPath,
				TypeName = EscalationNotificationResponder.TypeName,
				Name = name,
				ServiceName = serviceName,
				AlertTypeId = alertTypeId,
				AlertMask = alertMask,
				TargetResource = targetResource,
				TargetHealthState = targetHealthState,
				RecurrenceIntervalSeconds = recurrenceIntervalSeconds,
				TimeoutSeconds = Math.Min(recurrenceIntervalSeconds / 2, (int)EscalationNotificationResponder.DefaultTimeoutSeconds.TotalSeconds),
				MaxRetryAttempts = 3,
				Enabled = enabled
			};
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			base.Result.RecoveryResult = ServiceRecoveryResult.Failed;
			this.LastFailedMonitorResult = WorkItemResultHelper.GetLastFailedMonitorResult(this, base.Broker, cancellationToken);
			if (this.LastFailedMonitorResult == null)
			{
				base.Result.StateAttribute1 = "No monitor result - skipping notification";
				return;
			}
			this.LastFailedProbeResult = WorkItemResultHelper.GetLastFailedProbeResult(this.LastFailedMonitorResult, base.Broker, cancellationToken);
			if (this.LastFailedProbeResult != null)
			{
				string message = this.LastFailedProbeResult.IsNotified ? this.LastFailedProbeResult.Error : this.LastFailedProbeResult.Exception;
				this.PublishNotification(message, base.Definition.ServiceName, base.Definition.Name, base.Definition.TargetPartition);
				return;
			}
			base.Result.StateAttribute1 = "No probe result - skipping notification";
		}

		private void PublishNotification(string message, string serviceName, string notificationName, string notificationReason)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Logging a notification for monitor watching for alert to turn into alert state", null, "PublishNotification", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\EscalationNotificationResponder.cs", 154);
			EventNotificationItem eventNotificationItem = new EventNotificationItem(serviceName, notificationName, notificationReason, message, ResultSeverityLevel.Error);
			eventNotificationItem.StateAttribute1 = base.Definition.TargetResource;
			this.PopulateCustomAttributes(eventNotificationItem);
			eventNotificationItem.Publish(false);
		}

		public virtual void PopulateCustomAttributes(EventNotificationItem notificationItem)
		{
			if (this.LastFailedProbeResult != null)
			{
				notificationItem.StateAttribute2 = this.LastFailedProbeResult.WorkItemId.ToString();
				notificationItem.StateAttribute3 = this.LastFailedProbeResult.ResultId.ToString();
			}
		}

		private static TimeSpan DefaultTimeoutSeconds = TimeSpan.FromSeconds(120.0);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(EscalationNotificationResponder).FullName;
	}
}
