using System;

namespace Microsoft.Exchange.Diagnostics
{
	[Serializable]
	public sealed class MultiGranularityPercentileCounter : IPercentileCounter
	{
		public MultiGranularityPercentileCounter(TimeSpan expiryInterval, TimeSpan granularityInterval, MultiGranularityPercentileCounter.Param[] parameters, CurrentTimeProvider currentTimeProvider)
		{
			this.count = parameters.Length;
			ExAssert.RetailAssert(this.count > 0, "Number of parameters: {0} must be greater than zero.", new object[]
			{
				this.count
			});
			for (int i = 0; i < this.count - 1; i++)
			{
				ExAssert.RetailAssert(parameters[i].Granularity < parameters[i + 1].Granularity, "The granularities must be sorted.");
				ExAssert.RetailAssert(parameters[i].Range < parameters[i + 1].Range, "The ranges  must be sorted.");
				ExAssert.RetailAssert(parameters[i].Range % parameters[i + 1].Granularity == 0L, "The range[i] MOD granularity[i + 1] must be zero.");
			}
			this.percentileCounters = new PercentileCounter[this.count];
			this.borderBuckets = new long[this.count];
			for (int j = 0; j < this.count; j++)
			{
				this.percentileCounters[j] = new PercentileCounter(expiryInterval, granularityInterval, parameters[j].Granularity, parameters[j].Range, currentTimeProvider);
				this.borderBuckets[j] = parameters[j].Range - parameters[j].Granularity;
			}
		}

		public MultiGranularityPercentileCounter(TimeSpan expiryInterval, TimeSpan granularityInterval, MultiGranularityPercentileCounter.Param[] parameters) : this(expiryInterval, granularityInterval, parameters, null)
		{
		}

		public void AddValue(long value)
		{
			foreach (PercentileCounter percentileCounter in this.percentileCounters)
			{
				percentileCounter.AddValue(value);
			}
		}

		public long PercentileQuery(double percentage)
		{
			long num;
			return this.PercentileQuery(percentage, out num);
		}

		public long PercentileQuery(double percentage, out long samples)
		{
			long num = 0L;
			samples = 0L;
			int i = 0;
			while (i < this.count)
			{
				if (percentage <= 100.0 - this.percentileCounters[i].InfiniteBucketPercentage || i == this.count - 1)
				{
					num = this.percentileCounters[i].PercentileQuery(percentage, out samples);
					if (i > 0 && num < this.borderBuckets[i - 1])
					{
						num = this.borderBuckets[i - 1];
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			return num;
		}

		private PercentileCounter[] percentileCounters;

		private long[] borderBuckets;

		private int count;

		public struct Param
		{
			public Param(long granularity, long range)
			{
				ExAssert.RetailAssert(range % granularity == 0L, "The range MOD granularity must be zero.");
				this.granularity = granularity;
				this.range = range;
			}

			public long Granularity
			{
				get
				{
					return this.granularity;
				}
			}

			public long Range
			{
				get
				{
					return this.range;
				}
			}

			private long granularity;

			private long range;
		}
	}
}
