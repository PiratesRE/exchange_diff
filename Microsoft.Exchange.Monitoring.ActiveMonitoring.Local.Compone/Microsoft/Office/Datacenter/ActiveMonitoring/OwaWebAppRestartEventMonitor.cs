using System;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Threading;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class OwaWebAppRestartEventMonitor : OverallXFailuresMonitor
	{
		public new static MonitorDefinition CreateDefinition(string name, string sampleMask, string serviceName, Component component, int monitoringInterval, int recurrenceInterval, int numberOfFailures, bool enabled = true)
		{
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(name, sampleMask, serviceName, component, monitoringInterval, recurrenceInterval, numberOfFailures, enabled);
			monitorDefinition.AssemblyPath = OwaWebAppRestartEventMonitor.AssemblyPath;
			monitorDefinition.TypeName = OwaWebAppRestartEventMonitor.TypeName;
			return monitorDefinition;
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			base.DoMonitorWork(cancellationToken);
			string queryString = string.Format("<QueryList><Query Id=\"0\" Path=\"System\"><Select Path=\"System\">*[System[Provider[@Name='Microsoft-Windows-WAS'] and (EventID=5079) and (TimeCreated[@SystemTime&gt;='{0}' and @SystemTime&lt;='{1}'])] and (EventData/Data[@Name='AppPoolID']='MSExchangeOWAAppPool')]</Select></Query></QueryList>", base.MonitoringWindowStartTime.ToString("o"), base.Result.ExecutionStartTime.ToString("o"));
			if (this.GetSystemEvent(queryString))
			{
				base.Result.IsAlert = false;
			}
		}

		private bool GetSystemEvent(string queryString)
		{
			TimeSpan timeout = TimeSpan.FromSeconds(30.0);
			using (EventLogReader eventLogReader = new EventLogReader(new EventLogQuery("System", PathType.LogName, queryString)
			{
				ReverseDirection = true
			}))
			{
				EventRecord eventRecord = eventLogReader.ReadEvent(timeout);
				if (eventRecord != null)
				{
					return true;
				}
			}
			return false;
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(OwaWebAppRestartEventMonitor).FullName;
	}
}
