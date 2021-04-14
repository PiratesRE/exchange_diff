using System;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Inference
{
	public class TrainingFailurePercentageCounter : MultiInstanceFailurePercentageCalculatedCounter
	{
		public TrainingFailurePercentageCounter() : base("MSExchangeInference Pipeline", "Training Failure Percentage", "Number Of Succeeded Documents", "Number Of Failed Documents", TrainingFailurePercentageCounter.TimeRange, TrainingFailurePercentageCounter.MinimumProcessedCountNeeded)
		{
		}

		protected override bool ShouldCalculateForInstance(string instanceName)
		{
			Guid guid;
			return !string.IsNullOrEmpty(instanceName) && instanceName.StartsWith("training-", StringComparison.OrdinalIgnoreCase) && instanceName.Length > 9 && Guid.TryParse(instanceName.Substring(9), out guid);
		}

		public const string TrainingFailurePercentageCounterName = "Training Failure Percentage";

		private static readonly TimeSpan TimeRange = TimeSpan.FromMinutes(60.0);

		private static readonly int MinimumProcessedCountNeeded = 1000;
	}
}
