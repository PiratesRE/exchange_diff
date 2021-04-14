using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync
{
	internal sealed class TransportSyncExceptionAnalyzerAlertDefinition
	{
		public bool IsEnabled { get; internal set; }

		public TimeSpan RecurrenceInterval { get; internal set; }

		public TimeSpan MonitoringInterval { get; internal set; }

		public string RedEvent { get; internal set; }

		public string MessageSubject { get; internal set; }

		public string MessageBody { get; internal set; }

		public string MonitorName
		{
			get
			{
				return this.RedEvent.ToString();
			}
		}

		public Component Component { get; internal set; }
	}
}
