using System;
using Microsoft.Exchange.LogAnalyzer.Extensions.OABDownloadLog;

namespace Microsoft.Exchange.LogAnalyzer.Analyzers.OABDownloadLog
{
	public sealed class OABTenantInfo
	{
		public OABTenantInfo(string organization, OABDownloadLogLine logline, TimeSpan monitoringInterval)
		{
			this.organization = organization;
			this.logLine = logline;
			this.monitoringInterval = monitoringInterval;
		}

		public string Organization
		{
			get
			{
				return this.organization;
			}
		}

		public OABDownloadLogLine LogLine
		{
			get
			{
				return this.logLine;
			}
		}

		public DateTime LastAlertTime { get; set; }

		public int NoOfRequests { get; set; }

		public bool IsAlert(DateTime currentTime, int threshold, TimeSpan recurrenceInterval)
		{
			return currentTime - this.LastAlertTime > this.monitoringInterval && this.NoOfRequests > threshold && currentTime - this.logLine.Timestamp > recurrenceInterval;
		}

		private readonly string organization;

		private readonly TimeSpan monitoringInterval;

		private readonly OABDownloadLogLine logLine;
	}
}
