using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class BugcheckSimulator
	{
		public static BugcheckSimulator Instance { get; private set; } = new BugcheckSimulator();

		public bool IsHangRpc { get; private set; }

		public bool IsEnabled
		{
			get
			{
				return RegistryHelper.GetPropertyIntBool("IsEnabled", false, "BugcheckSimulation", null, false);
			}
		}

		public DateTime SimulatedSystemBootTime
		{
			get
			{
				DateTime result = DateTime.MinValue;
				string property = RegistryHelper.GetProperty<string>("SimulatedBootTime", string.Empty, "BugcheckSimulation", null, false);
				if (!string.IsNullOrEmpty(property))
				{
					result = DateTime.Parse(property);
				}
				return result;
			}
		}

		public TimeSpan Duration
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryHelper.GetProperty<int>("DurationInSeconds", 60, "BugcheckSimulation", null, false));
			}
		}

		public bool IsExitProcess
		{
			get
			{
				return RegistryHelper.GetPropertyIntBool("IsExitProcess", true, "BugcheckSimulation", null, false);
			}
		}

		public void TakeActionIfRequired()
		{
			if (!this.IsEnabled)
			{
				return;
			}
			WTFDiagnostics.TraceWarning(ExTraceGlobals.RecoveryActionTracer, this.traceContext, "Bugcheck simulator is attempting to do the operation.", null, "TakeActionIfRequired", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\BugcheckSimulator.cs", 153);
			bool isExitProcess = this.IsExitProcess;
			TimeSpan duration = this.Duration;
			try
			{
				this.IsHangRpc = true;
				Thread.Sleep(duration);
				DateTime localTime = ExDateTime.Now.LocalTime;
				this.SetSimulatedBootTime(localTime);
				ManagedAvailabilityCrimsonEvents.SimulatedBugcheckInAction.Log<TimeSpan, bool, DateTime, bool>(duration, false, localTime, isExitProcess);
				if (isExitProcess)
				{
					Environment.Exit(1);
				}
			}
			finally
			{
				this.IsHangRpc = false;
			}
		}

		private void SetDuration(TimeSpan duration)
		{
			RegistryHelper.SetProperty<int>("DurationInSeconds", (int)duration.TotalSeconds, "BugcheckSimulation", null, false);
		}

		private void SetSimulatedBootTime(DateTime simulatedDateTime)
		{
			string propertValue = simulatedDateTime.ToString("o");
			RegistryHelper.SetProperty<string>("SimulatedBootTime", propertValue, "BugcheckSimulation", null, false);
		}

		private void SetEnabled(bool isEnabled)
		{
			RegistryHelper.SetPropertyIntBool("IsEnabled", isEnabled, "BugcheckSimulation", null, false);
		}

		private void SetIsExitProcess(bool isExitProcess)
		{
			RegistryHelper.SetPropertyIntBool("IsExitProcess", isExitProcess, "BugcheckSimulation", null, false);
		}

		internal const string SubKeyName = "BugcheckSimulation";

		internal const string PropertyNameIsEnabled = "IsEnabled";

		internal const string PropertyNameDurationInSeconds = "DurationInSeconds";

		internal const string PropertyNameSimulatedBootTime = "SimulatedBootTime";

		internal const string PropertyNameIsExitProcess = "IsExitProcess";

		private TracingContext traceContext = TracingContext.Default;
	}
}
