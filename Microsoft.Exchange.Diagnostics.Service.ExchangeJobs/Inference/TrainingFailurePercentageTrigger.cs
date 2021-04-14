using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Inference
{
	public class TrainingFailurePercentageTrigger : PerInstanceTrigger
	{
		public TrainingFailurePercentageTrigger(IJob job) : base(job, string.Format("{0}\\({1}\\)\\\\{2}", "MSExchangeInference Pipeline", "training-(.+?)", "Training Failure Percentage"), new PerfLogCounterTrigger.TriggerConfiguration("TrainingFailurePercentageTrigger", 5.0, double.MaxValue, TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(30.0), 0), new HashSet<DiagnosticMeasurement>(), new HashSet<string>())
		{
		}

		protected override string CollectAdditionalInformation(PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
			return "The failure percentage is calculated from the 'MSExchangeInference Pipeline(training-<mdbGuid>)\\Number Of Succeeded Documents' counter and 'Number Of Failed Documents' counter over the past hour.";
		}

		private const string AdditionalInfo = "The failure percentage is calculated from the 'MSExchangeInference Pipeline(training-<mdbGuid>)\\Number Of Succeeded Documents' counter and 'Number Of Failed Documents' counter over the past hour.";
	}
}
