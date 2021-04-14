using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class SLALatencyOverallConsecutiveBelowThresholdMonitor : OverallConsecutiveSampleValueBelowThresholdMonitor
	{
		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			base.Result.StateAttribute6 = 0.0;
			base.Result.StateAttribute7 = 0.0;
			base.Result.StateAttribute8 = 0.0;
			string serverComponent = base.Definition.Attributes["ServerComponentName"];
			string expectedState = base.Definition.Attributes["ExpectedComponentState"];
			string text;
			this.shouldGatherSlaDataAndAlert = ComponentState.VerifyExpectedState(serverComponent, expectedState, out text);
			base.Result.StateAttribute10 = (double)(this.shouldGatherSlaDataAndAlert ? 1 : 0);
			base.DoMonitorWork(cancellationToken);
			if (!this.shouldGatherSlaDataAndAlert)
			{
				return;
			}
			base.Result.StateAttribute6 = this.GatherSlaDatainStateAttribute("stateAttribute6Category", "stateAttribute6Counter", "stateAttribute6Instance");
			base.Result.StateAttribute7 = this.GatherSlaDatainStateAttribute("stateAttribute7Category", "stateAttribute7Counter", "stateAttribute7Instance");
			base.Result.StateAttribute8 = this.GatherSlaDatainStateAttribute("stateAttribute8Category", "stateAttribute8Counter", "stateAttribute8Instance");
		}

		protected override bool ShouldAlert()
		{
			double num = 0.0;
			if (!this.shouldGatherSlaDataAndAlert)
			{
				return false;
			}
			string s;
			if (!base.Definition.Attributes.TryGetValue("GatingThreshold", out s) || !double.TryParse(s, out num))
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.TransportTracer, base.TraceContext, "SLALatency: no GatingThreshold", null, "ShouldAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\SLAOverallConsecutiveBelowThresholdMonitor.cs", 121);
				base.Result.StateAttribute1 = "No GatingThreshold or invalid value syntax";
				return true;
			}
			string text = base.Definition.Attributes["GatingCategory"];
			string text2 = base.Definition.Attributes["GatingCounter"];
			string text3 = base.Definition.Attributes["GatingInstance"];
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text3))
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.TransportTracer, base.TraceContext, "SLALatency: Gating parameters are incorrect", null, "ShouldAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\SLAOverallConsecutiveBelowThresholdMonitor.cs", 132);
				throw new ArgumentNullException("Gating parameters are incorrect");
			}
			bool flag = base.ShouldAlert();
			if (flag)
			{
				double num2;
				if (SLALatencyOverallConsecutiveBelowThresholdMonitor.GetPerfmonCounter(text, text2, text3, out num2) && num > num2)
				{
					base.Result.StateAttribute1 = string.Format("Monitor alert suppressed by gated performance counter: threshold='{0}', current='{1}'", num, num2);
					return false;
				}
				base.Result.StateAttribute1 = string.Format("Both SLA and alert gating performance counter are exceeding threshold='{0}', current='{1}'", num, num2);
			}
			return flag;
		}

		protected override void SetStateAttribute6ForScopeMonitoring(double counter)
		{
		}

		private static bool GetPerfmonCounter(string category, string counterName, string instance, out double counterValue)
		{
			counterValue = 0.0;
			try
			{
				using (PerformanceCounter performanceCounter = new PerformanceCounter(category, counterName, instance))
				{
					performanceCounter.NextValue();
					counterValue = (double)performanceCounter.NextValue();
					return true;
				}
			}
			catch (InvalidOperationException)
			{
				counterValue = 0.0;
			}
			return false;
		}

		private double GatherSlaDatainStateAttribute(string categoryName, string counterName, string instanceName)
		{
			string text = base.Definition.Attributes[categoryName];
			string text2 = base.Definition.Attributes[counterName];
			string text3 = base.Definition.Attributes[instanceName];
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text3))
			{
				string text4 = string.Format("SLALatency: One of CateoryName: {0}, Counter: {1}, Instance: {2} parameters is incorrect", categoryName, counterName, instanceName);
				WTFDiagnostics.TraceError(ExTraceGlobals.TransportTracer, base.TraceContext, text4, null, "GatherSlaDatainStateAttribute", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\SLAOverallConsecutiveBelowThresholdMonitor.cs", 223);
				throw new ArgumentNullException(text4);
			}
			double result;
			if (!SLALatencyOverallConsecutiveBelowThresholdMonitor.GetPerfmonCounter(text, text2, text3, out result))
			{
				return 0.0;
			}
			return result;
		}

		internal const string ServerComponentName = "ServerComponentName";

		internal const string ExpectedComponentState = "ExpectedComponentState";

		internal const string GatingThresholdLabel = "GatingThreshold";

		internal const string GatingCategoryLabel = "GatingCategory";

		internal const string GatingCounterLabel = "GatingCounter";

		internal const string GatingInstanceLabel = "GatingInstance";

		private bool shouldGatherSlaDataAndAlert;
	}
}
