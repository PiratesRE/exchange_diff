using System;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Oab
{
	public class OabWebAppRestartEventMonitor : MonitorWorkItem
	{
		public static MonitorDefinition CreateDefinition(string name, string sampleMask, Component component, int failureCount, int monitoringInterval, int recurrenceInterval)
		{
			return new MonitorDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				TypeName = typeof(OabWebAppRestartEventMonitor).FullName,
				Name = name,
				SampleMask = sampleMask,
				Component = component,
				MonitoringThreshold = (double)failureCount,
				MonitoringIntervalSeconds = monitoringInterval,
				RecurrenceIntervalSeconds = recurrenceInterval,
				TimeoutSeconds = recurrenceInterval * 2,
				ServiceName = component.Name,
				MaxRetryAttempts = 3
			};
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			string queryString = string.Format("<QueryList><Query Id=\"0\" Path=\"System\"><Select Path=\"System\">*[System[Provider[@Name='Microsoft-Windows-WAS'] and (EventID=5117) and (TimeCreated[@SystemTime&gt;='{0}' and @SystemTime&lt;='{1}'])] and (EventData/Data[@Name='AppPoolID']='MSExchangeOabAppPool')]</Select></Query></QueryList>", base.MonitoringWindowStartTime.ToString("o"), base.Result.ExecutionStartTime.ToString("o"));
			if (this.GetSystemEvent(queryString))
			{
				base.Result.IsAlert = true;
				NotificationItem notificationItem = new EventNotificationItem(ExchangeComponent.Oab.Name, ExchangeComponent.Oab.Name, null, "OABAppPoolTooManyRecycles", ResultSeverityLevel.Error);
				notificationItem.Publish(false);
			}
		}

		private bool GetSystemEvent(string queryString)
		{
			TimeSpan.FromSeconds(30.0);
			using (EventLogReader eventLogReader = new EventLogReader(new EventLogQuery("System", PathType.LogName, queryString)
			{
				ReverseDirection = true
			}))
			{
				base.Result.StateAttribute1 = eventLogReader.BatchSize.ToString();
				if (eventLogReader.BatchSize > OabWebAppRestartEventMonitor.noOfAppPoolRecyclesThreshold)
				{
					return true;
				}
			}
			return false;
		}

		private static readonly int noOfAppPoolRecyclesThreshold = 3;
	}
}
