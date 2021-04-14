using System;
using System.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Monitors
{
	public class DelayedStartOverallConsecutiveProbeFailuresMonitor : OverallConsecutiveProbeFailuresMonitor
	{
		protected override bool ShouldAlert()
		{
			bool flag = base.ShouldAlert();
			string text;
			if (flag && base.Definition.Attributes.TryGetValue("ProcessElapsedGatingService", out text))
			{
				int num = 0;
				string s;
				if (!base.Definition.Attributes.TryGetValue("ProcessElapsedGatingThreshold", out s) || !int.TryParse(s, out num))
				{
					num = base.Definition.MonitoringIntervalSeconds;
				}
				int num2;
				if (DelayedStartOverallConsecutiveProbeFailuresMonitor.GetProcessElapsedTimeCounterValue(text, out num2) && num > num2)
				{
					base.Result.StateAttribute1 = string.Format("Monitor alert suppressed by gated performance counter: \\Process({0})\\Elapsed Time. Process Elapsed Time: {1} seconds. Time until alert: {2} seconds", text, num2, num - num2);
					return false;
				}
			}
			return flag;
		}

		private static bool GetProcessElapsedTimeCounterValue(string serviceName, out int value)
		{
			value = 0;
			if (!string.IsNullOrEmpty(serviceName))
			{
				try
				{
					using (PerformanceCounter performanceCounter = new PerformanceCounter("Process", "Elapsed Time", serviceName))
					{
						performanceCounter.NextValue();
						value = (int)performanceCounter.NextValue();
						return true;
					}
				}
				catch (InvalidOperationException)
				{
				}
				return false;
			}
			return false;
		}
	}
}
