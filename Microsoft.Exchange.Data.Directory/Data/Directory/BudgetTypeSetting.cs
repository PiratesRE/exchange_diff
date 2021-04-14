using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal class BudgetTypeSetting
	{
		public TimeSpan MaxDelay { get; private set; }

		public int MaxMicroDelayMultiplier { get; private set; }

		public int MaxDelayedThreads { get; private set; }

		public int MaxDelayedThreadPerProcessor { get; private set; }

		public BudgetTypeSetting(TimeSpan maxDelay, int maxMicroDelayMultiplier, int maxDelayedThreadsPerProcessor)
		{
			this.MaxDelay = maxDelay;
			this.MaxMicroDelayMultiplier = maxMicroDelayMultiplier;
			this.MaxDelayedThreadPerProcessor = maxDelayedThreadsPerProcessor;
			this.MaxDelayedThreads = maxDelayedThreadsPerProcessor * BudgetTypeSetting.ProcessorCount;
		}

		private static readonly int ProcessorCount = Environment.ProcessorCount;

		public static readonly BudgetTypeSetting OneMinuteSetting = new BudgetTypeSetting(TimeSpan.FromMinutes(1.0), 10, 50);
	}
}
