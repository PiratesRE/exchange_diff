using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Worker.Health
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IRemoteServerHealthConfiguration
	{
		bool RemoteServerHealthManagementEnabled { get; }

		bool RemoteServerPoisonMarkingEnabled { get; }

		TimeSpan RemoteServerLatencySlidingCounterWindowSize { get; }

		TimeSpan RemoteServerLatencySlidingCounterBucketLength { get; }

		TimeSpan RemoteServerLatencyThreshold { get; }

		int RemoteServerBackoffCountLimit { get; }

		TimeSpan RemoteServerBackoffTimeSpan { get; }

		TimeSpan RemoteServerHealthDataExpiryPeriod { get; }

		TimeSpan RemoteServerHealthDataExpiryAndPersistanceFrequency { get; }

		double RemoteServerAllowedCapacityUsagePercentage { get; }

		TimeSpan RemoteServerCapacityUsageThreshold { get; }
	}
}
