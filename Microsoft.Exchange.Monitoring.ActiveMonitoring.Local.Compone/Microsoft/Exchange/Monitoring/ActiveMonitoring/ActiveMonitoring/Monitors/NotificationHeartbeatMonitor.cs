using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors
{
	public class NotificationHeartbeatMonitor : MonitorWorkItem
	{
		public static MonitorDefinition CreateDefinition(string monitorName, string serviceName, Component component, string notificationMask, int recurrenceInterval, int heartbeatSLA, bool enabled)
		{
			return new MonitorDefinition
			{
				AssemblyPath = NotificationHeartbeatMonitor.AssemblyPath,
				TypeName = NotificationHeartbeatMonitor.TypeName,
				Name = monitorName,
				ServiceName = serviceName,
				Component = component,
				SampleMask = notificationMask,
				MonitoringIntervalSeconds = heartbeatSLA,
				RecurrenceIntervalSeconds = recurrenceInterval,
				TimeoutSeconds = 120,
				MaxRetryAttempts = 0,
				Enabled = enabled
			};
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			this.Trace("NotificationHeartbeatMonitor.DoWork: Getting health state for all instances of probe '{0}'...", new object[]
			{
				base.Definition.SampleMask
			});
			DateTime currentTime = base.Result.ExecutionStartTime;
			DateTime monitoringWindowStartTime = base.MonitoringWindowStartTime;
			DateTime currentTime2 = currentTime;
			TimeSpan monitoringWindow = currentTime2 - monitoringWindowStartTime;
			DateTime heartbeartSLATime = base.MonitoringWindowStartTime;
			StringBuilder diagnosticsString = new StringBuilder(128);
			diagnosticsString.AppendFormat("CurrentTime={0};", currentTime.ToString());
			diagnosticsString.AppendFormat("NextSchedule={0};", base.Definition.StartTime.ToString());
			diagnosticsString.AppendFormat("MonitoringWindowStart={0};", monitoringWindowStartTime.ToString());
			diagnosticsString.AppendFormat("MonitoringWindow={0}s;", monitoringWindow.TotalSeconds);
			diagnosticsString.AppendFormat("HeartbeatSLA={0};", heartbeartSLATime.ToString());
			DateTime lastNotificationTime = DateTime.MinValue;
			IDataAccessQuery<ProbeResult> probeResults = base.Broker.GetProbeResults(base.Definition.SampleMask, monitoringWindowStartTime, currentTime2);
			Task<int> task2 = base.Broker.AsDataAccessQuery<ProbeResult>(probeResults).ExecuteAsync(delegate(ProbeResult result)
			{
				if (result != null && result.ExecutionStartTime > lastNotificationTime)
				{
					lastNotificationTime = result.ExecutionStartTime;
				}
			}, cancellationToken, base.TraceContext);
			task2.ContinueWith(delegate(Task<int> task)
			{
				this.Trace("NotificationHeartbeatMonitor.DoWork: Last notification time: {0}", new object[]
				{
					lastNotificationTime.ToString()
				});
				if (lastNotificationTime == DateTime.MinValue)
				{
					this.Trace("NotificationHeartbeatMonitor.DoWork: Querying for last monitor result", new object[0]);
					MonitorResult lastMonitorResult = null;
					IDataAccessQuery<MonitorResult> lastMonitorResult2 = this.Broker.GetLastMonitorResult(this.Definition, monitoringWindow);
					Task<int> task3 = this.Broker.AsDataAccessQuery<MonitorResult>(lastMonitorResult2).ExecuteAsync(delegate(MonitorResult result)
					{
						if (result != null)
						{
							lastMonitorResult = result;
						}
					}, cancellationToken, this.TraceContext);
					task3.Wait();
					diagnosticsString.AppendFormat("LastResultTime={0};", (lastMonitorResult == null) ? "null" : lastMonitorResult.ExecutionStartTime.ToString());
					diagnosticsString.AppendFormat("LastResultNotificationTime={0};", (lastMonitorResult == null) ? "null" : lastMonitorResult.StateAttribute1);
					if (lastMonitorResult == null)
					{
						lastNotificationTime = currentTime;
					}
					else
					{
						DateTime.TryParse(lastMonitorResult.StateAttribute1, out lastNotificationTime);
					}
				}
				this.Trace("NotificationHeartbeatMonitor.DoWork: Last notification time based on last monitor result: {0}", new object[]
				{
					lastNotificationTime.ToString()
				});
				this.Result.StateAttribute1 = lastNotificationTime.ToString();
				this.Result.StateAttribute2 = diagnosticsString.ToString();
				if (lastNotificationTime < heartbeartSLATime)
				{
					this.Result.IsAlert = true;
				}
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current);
		}

		private void Trace(string message, params object[] args)
		{
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format(message, args), null, "Trace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\NotificationHeartbeatMonitor.cs", 178);
		}

		private const int DefaultTimeoutSeconds = 120;

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(NotificationHeartbeatMonitor).FullName;
	}
}
