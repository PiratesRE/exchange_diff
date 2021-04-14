using System;
using Microsoft.Exchange.LogAnalyzer.Analyzers.EventLog;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ProcessIsolation.Monitors
{
	internal class SubComponentConfiguration
	{
		internal SubComponentConfiguration(StackTraceAnalysisProcessNames process, StackTraceAnalysisComponentNames subComponent, ProcessTrigger triggerType) : this(process, subComponent, null, triggerType, false)
		{
		}

		internal SubComponentConfiguration(StackTraceAnalysisProcessNames process, StackTraceAnalysisComponentNames subComponent, Component escalationComponent, ProcessTrigger triggerType, bool addCorrelation)
		{
			this.Process = process;
			this.SubComponent = subComponent;
			this.EscalationComponent = escalationComponent;
			this.TriggerType = triggerType;
			this.AddCorrelation = addCorrelation;
		}

		internal bool AddCorrelation { get; set; }

		internal const string SubComponentParameter = "SubComponent";

		internal readonly StackTraceAnalysisProcessNames Process;

		internal readonly StackTraceAnalysisComponentNames SubComponent;

		internal readonly Component EscalationComponent;

		internal readonly ProcessTrigger TriggerType;
	}
}
