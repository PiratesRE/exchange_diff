using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Inference
{
	public class ClassificationSLATrigger : PerfLogCounterTrigger
	{
		public ClassificationSLATrigger(IJob job) : base(job, Regex.Escape(string.Format("{0}({1})\\{2}", "MSExchangeInference Pipeline", "classificationpipeline", "Classification SLA")), new PerfLogCounterTrigger.TriggerConfiguration("ClassificationSLATrigger", 98.5, double.MinValue, TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(30.0), 1))
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("The SLA is calculated from the 'MSExchange Delivery Store Driver\\SucessfulDeliveries', 'MSExchangeInference Pipeline\\Number Of Failed Documents\\classificationpipeline', 'MSExchangeInference Classification Processing\\Items Skipped' and 'MSExchange Delivery Store Driver Agents\\StoreDriverDelivery Agent Failure\\inference classification agent' counters over the past hour.");
			stringBuilder.AppendLine("Please visit https://eds.outlook.com/ for more historical data.");
			this.additionalInfo = stringBuilder.ToString();
		}

		internal static string LastFailureCounterData { get; set; }

		protected override void OnThresholdEvent(PerfLogLine line, PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
		}

		protected override string CollectAdditionalInformation(PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
			return (ClassificationSLATrigger.LastFailureCounterData ?? string.Empty) + this.additionalInfo;
		}

		internal const double TriggerThreshold = 98.5;

		internal const double IgnorableExceptionTriggerThreshold = 97.0;

		private readonly string additionalInfo;
	}
}
