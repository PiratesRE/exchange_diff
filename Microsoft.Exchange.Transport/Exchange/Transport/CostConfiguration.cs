using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport
{
	internal class CostConfiguration
	{
		public CostConfiguration(IWaitConditionManagerConfig config, IProcessingQuotaComponent quotaOverride, Func<DateTime> timeProvider)
		{
			this.config = config;
			this.quotaOverride = quotaOverride;
			this.timeProvider = timeProvider;
		}

		public CostConfiguration(IWaitConditionManagerConfig config, bool reversedCollection, int maxThreads, long processingCapacity, IProcessingQuotaComponent quotaOverride, Func<DateTime> timeProvider) : this(config, quotaOverride, timeProvider)
		{
			this.reversedCollection = reversedCollection;
			this.maxThreads = maxThreads;
			this.processingCapacity = processingCapacity;
		}

		public IWaitConditionManagerConfig Config
		{
			get
			{
				return this.config;
			}
		}

		public bool ProcessingHistoryEnabled
		{
			get
			{
				return this.config.ProcessingTimeThrottlingEnabled;
			}
		}

		public bool MemoryCollectionEnabled
		{
			get
			{
				return this.config.ThrottlingMemoryMaxThreshold.ToBytes() > 0UL;
			}
		}

		public TimeSpan HistoryInterval
		{
			get
			{
				return this.config.ThrottlingHistoryInterval;
			}
		}

		public TimeSpan BucketSize
		{
			get
			{
				return this.config.ThrottlingHistoryBucketSize;
			}
		}

		public TimeSpan MinInterestingProcessingInterval
		{
			get
			{
				return this.config.ThrottlingProcessingMinThreshold;
			}
		}

		public ByteQuantifiedSize MinInterestingMemorySize
		{
			get
			{
				return this.config.ThrottlingMemoryMinThreshold;
			}
		}

		public ByteQuantifiedSize MemoryThreshold
		{
			get
			{
				return this.config.ThrottlingMemoryMaxThreshold;
			}
		}

		public IProcessingQuotaComponent QuotaOverride
		{
			get
			{
				return this.quotaOverride;
			}
		}

		public Func<DateTime> TimeProvider
		{
			get
			{
				return this.timeProvider;
			}
		}

		public bool ReversedCost
		{
			get
			{
				return this.reversedCollection;
			}
		}

		public int MaxThreads
		{
			get
			{
				return this.maxThreads;
			}
		}

		public long ProcessingCapacity
		{
			get
			{
				return this.processingCapacity;
			}
		}

		public bool AboveThresholdBehaviorEnabled
		{
			get
			{
				return this.config.AboveThresholdThrottlingBehaviorEnabled;
			}
		}

		public int MaxAllowedCapacity
		{
			get
			{
				return this.config.MaxAllowedCapacityPercentage;
			}
		}

		public TimeSpan EmptyCostRemovalInterval
		{
			get
			{
				return this.config.EmptyThrottlingCostRemovalInterval;
			}
		}

		public bool OverrideEnabled
		{
			get
			{
				return this.config.QuotaOverrideEnabled;
			}
		}

		public bool TestOverrideEnabled
		{
			get
			{
				return this.config.TestQuotaOverrideEnabled;
			}
		}

		private readonly IWaitConditionManagerConfig config;

		private readonly bool reversedCollection;

		private readonly int maxThreads;

		private readonly long processingCapacity;

		private readonly IProcessingQuotaComponent quotaOverride;

		private readonly Func<DateTime> timeProvider;
	}
}
