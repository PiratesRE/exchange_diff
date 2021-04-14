using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class WstConfigMetadataExpiredProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			ProbeResult result = base.Result;
			result.ExecutionContext += string.Format(string.Format("WstConfigMetadataExpiredProbe started {0}. ", DateTime.UtcNow), new object[0]);
			int num = int.Parse(base.Definition.Attributes["windowSeconds"]);
			ProbeResult result2 = base.Result;
			result2.ExecutionContext += string.Format(string.Format("windowSeconds:{0}. ", num), new object[0]);
			using (EventLog eventLog = new EventLog())
			{
				eventLog.Log = "Application";
				DateTime t = DateTime.UtcNow.AddSeconds((double)(-(double)num));
				int num2 = 0;
				try
				{
					int num3 = eventLog.Entries.Count - 1;
					while (num3 >= 0 && eventLog.Entries[num3].TimeGenerated.ToUniversalTime() >= t)
					{
						num2++;
						EventLogEntry eventLogEntry = eventLog.Entries[num3];
						if (string.Equals(eventLogEntry.Source, "ICLMetadata") && eventLogEntry.InstanceId == 8000L && !eventLogEntry.Message.Contains("Expired for 0 secs"))
						{
							throw new Exception("Wst metadata cache has expired.");
						}
						num3--;
					}
				}
				catch (ArgumentException)
				{
				}
				ProbeResult result3 = base.Result;
				result3.ExecutionContext += string.Format(string.Format("Checked {0} entries. ", num2), new object[0]);
				ProbeResult result4 = base.Result;
				result4.ExecutionContext += string.Format(string.Format("WstConfigMetadataExpiredProbe completed {0}. ", DateTime.UtcNow), new object[0]);
			}
		}
	}
}
