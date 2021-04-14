using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Exchange.Search.Query
{
	public class QueryExecutionStep
	{
		internal QueryExecutionStep(QueryExecutionStepType stepType, Stopwatch stopwatch)
		{
			this.StepType = stepType;
			this.StartTime = DateTime.UtcNow;
			this.startReading = stopwatch.ElapsedMilliseconds;
		}

		public QueryExecutionStepType StepType { get; private set; }

		public DateTime StartTime { get; private set; }

		public DateTime EndTime
		{
			get
			{
				return this.StartTime.AddMilliseconds((double)this.ElapsedMilliseconds);
			}
		}

		public long ElapsedMilliseconds { get; private set; }

		public IReadOnlyCollection<KeyValuePair<string, object>> AdditionalStatistics { get; private set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("QueryExecutionStep -> StepType: {0}, StartTime: {1}, EndTime {2}, ElapsedMilliseconds: {3}", new object[]
			{
				this.StepType,
				this.StartTime,
				this.EndTime,
				this.ElapsedMilliseconds
			});
			if (this.AdditionalStatistics != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in this.AdditionalStatistics)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("    AdditionalStatistic: {0} => {1}", keyValuePair.Key, keyValuePair.Value);
				}
			}
			return stringBuilder.ToString();
		}

		internal QueryExecutionStep Complete(Stopwatch stopwatch)
		{
			return this.Complete(stopwatch, null);
		}

		internal QueryExecutionStep Complete(Stopwatch stopwatch, IReadOnlyCollection<KeyValuePair<string, object>> additionalStatistics)
		{
			this.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds - this.startReading;
			this.AdditionalStatistics = (additionalStatistics ?? QueryExecutionStep.EmptyAdditionalStatistics);
			return this;
		}

		private static readonly IReadOnlyCollection<KeyValuePair<string, object>> EmptyAdditionalStatistics = new List<KeyValuePair<string, object>>();

		private readonly long startReading;
	}
}
