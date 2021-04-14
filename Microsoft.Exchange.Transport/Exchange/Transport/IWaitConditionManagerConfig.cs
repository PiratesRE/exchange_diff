using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport
{
	internal interface IWaitConditionManagerConfig
	{
		bool TenantThrottlingEnabled { get; }

		bool SenderThrottlingEnabled { get; }

		bool QuotaOverrideEnabled { get; }

		bool TestQuotaOverrideEnabled { get; }

		bool ProcessingTimeThrottlingEnabled { get; }

		TimeSpan ThrottlingHistoryInterval { get; }

		TimeSpan ThrottlingHistoryBucketSize { get; }

		TimeSpan ThrottlingProcessingMinThreshold { get; }

		ByteQuantifiedSize ThrottlingMemoryMinThreshold { get; }

		ByteQuantifiedSize ThrottlingMemoryMaxThreshold { get; }

		bool AboveThresholdThrottlingBehaviorEnabled { get; }

		int MaxAllowedCapacityPercentage { get; }

		TimeSpan EmptyThrottlingCostRemovalInterval { get; }

		int LockedMessageDehydrationThreshold { get; }
	}
}
