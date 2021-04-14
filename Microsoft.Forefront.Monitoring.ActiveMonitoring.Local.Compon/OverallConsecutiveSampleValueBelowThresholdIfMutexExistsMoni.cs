using System;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class OverallConsecutiveSampleValueBelowThresholdIfMutexExistsMonitor : OverallConsecutiveSampleValueBelowThresholdMonitor
	{
		public new Task GetConsecutiveSampleValueBelowThresholdCounts(string sampleMask, double thresholdValue, int consecutiveThreshold, Action<int> setNewResultCount, Action<int> setTotalResultCount, CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation<string, double>(ExTraceGlobals.MonitoringTracer, base.TraceContext, "OverallConsecutiveSampleValueBelowThresholdIfMutexExistsMonitor.GetConsecutiveBelowThresholdCount: Querying for values of {0} having values below {1}.", sampleMask, thresholdValue, null, "GetConsecutiveSampleValueBelowThresholdCounts", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Common\\Monitors\\OverallConsecutiveSampleValueBelowThresholdIfMutexExistsMonitor.cs", 45);
			Func<double, bool> thresholdComparer = delegate(double value)
			{
				bool flag;
				try
				{
					string name = this.Definition.Attributes["MutexName"];
					using (Mutex.OpenExisting(name, MutexRights.ReadPermissions))
					{
						flag = true;
					}
				}
				catch (WaitHandleCannotBeOpenedException)
				{
					flag = false;
				}
				catch (UnauthorizedAccessException)
				{
					flag = true;
				}
				return flag && value < thresholdValue;
			};
			return base.GetConsecutiveSampleValueThresholdCounts(sampleMask, consecutiveThreshold, thresholdComparer, setNewResultCount, setTotalResultCount, cancellationToken);
		}

		protected override Task SetConsecutiveFailureNumbers(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.MonitoringTracer, base.TraceContext, "OverallConsecutiveSampleValueBelowThresholdIfMutexExistsMonitor.SetConsecutiveFailureNumbers: Getting overall consecutive samples of: {0}.", base.Definition.SampleMask, null, "SetConsecutiveFailureNumbers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Common\\Monitors\\OverallConsecutiveSampleValueBelowThresholdIfMutexExistsMonitor.cs", 94);
			return this.GetConsecutiveSampleValueBelowThresholdCounts(base.Definition.SampleMask, (double)((int)base.Definition.MonitoringThreshold), (int)base.Definition.SecondaryMonitoringThreshold, delegate(int newValue)
			{
				base.Result.NewValue = (double)newValue;
			}, delegate(int totalValue)
			{
				base.Result.TotalValue = (double)totalValue;
			}, cancellationToken);
		}
	}
}
