using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Service.Common;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.CalculatedCounters
{
	public class DatabaseMountedBitmapCalculatedCounter : MultiInstanceSingleObjectCalculatedCounter
	{
		public DatabaseMountedBitmapCalculatedCounter() : base("MSExchange Active Manager", "Database Mounted Bitmap", new string[]
		{
			"Database Mounted"
		})
		{
			this.stepDuration = Configuration.GetConfigTimeSpan("PerfLogAggregatorAnalyzerStepDuration", TimeSpan.FromMinutes(1.0), TimeSpan.MaxValue, TimeSpan.FromMinutes(5.0));
			this.previousTimestamp = DateTime.MinValue;
			this.isEnabled = (this.stepDuration.Ticks != 0L && ServerRole.IsRole("Mailbox"));
		}

		public override void OnLogLine(Dictionary<DiagnosticMeasurement, float?> countersAndValues, DateTime? timestamp)
		{
			if (!this.isEnabled)
			{
				return;
			}
			if (timestamp == null)
			{
				return;
			}
			foreach (KeyValuePair<string, DiagnosticMeasurement[]> keyValuePair in base.Instances)
			{
				float? num = countersAndValues[keyValuePair.Value[0]];
				DiagnosticMeasurement key = keyValuePair.Value[1];
				if (num != null && !"_total".Equals(keyValuePair.Key, StringComparison.OrdinalIgnoreCase) && Math.Abs((timestamp.Value - this.previousTimestamp).TotalMilliseconds) > 900.0)
				{
					float? value = new float?((float)((int)num.Value << (int)(timestamp.Value.Ticks % this.stepDuration.Ticks / this.perfSampleIntervalTickes)));
					countersAndValues.Add(key, value);
				}
			}
			this.previousTimestamp = timestamp.Value;
		}

		public const string PerfObjectName = "MSExchange Active Manager";

		public const string DatabaseMountedBitmapCounterName = "Database Mounted Bitmap";

		public const string DatabaseMountedCounterName = "Database Mounted";

		public const string StepDurationConfigName = "PerfLogAggregatorAnalyzerStepDuration";

		public const string TotalInstanceName = "_total";

		private const int MinimumIntervalBetweenSamples = 900;

		private readonly long perfSampleIntervalTickes = new TimeSpan(0, 0, 15).Ticks;

		private readonly TimeSpan stepDuration;

		private readonly bool isEnabled;

		private DateTime previousTimestamp;
	}
}
