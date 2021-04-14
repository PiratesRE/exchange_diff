using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;

namespace Microsoft.Exchange.MailboxLoadBalance.Config
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ILoadBalanceSettings
	{
		bool AutomaticDatabaseDrainEnabled { get; }

		int AutomaticDrainStartFileSizePercent { get; }

		bool AutomaticLoadBalancingEnabled { get; }

		BatchSizeReducerType BatchBatchSizeReducer { get; }

		bool BuildLocalCacheOnStartup { get; }

		int CapacityGrowthPeriods { get; }

		TimeSpan ClientCacheTimeToLive { get; }

		int ConsumerGrowthRate { get; }

		long DefaultDatabaseMaxSizeGb { get; }

		int DefaultDatabaseRelativeLoadCapacity { get; }

		bool DisableWlm { get; }

		bool DontCreateMoveRequests { get; }

		bool DontRemoveSoftDeletedMailboxes { get; }

		string ExcludedMailboxProcessors { get; }

		TimeSpan IdleRunDelay { get; }

		int InjectionBatchSize { get; }

		bool IsEnabled { get; }

		bool LoadBalanceBlocked { get; }

		TimeSpan LocalCacheRefreshPeriod { get; }

		string LogFilePath { get; }

		long LogMaxDirectorySize { get; }

		long LogMaxFileSize { get; }

		bool MailboxProcessorsEnabled { get; }

		int MaxDatabaseDiskUtilizationPercent { get; }

		long MaximumAmountOfDataPerRoundGb { get; }

		long MaximumBatchSizeGb { get; }

		int MaximumConsumerMailboxSizePercent { get; }

		int MaximumNumberOfRunspaces { get; }

		long MaximumPendingMoveCount { get; }

		TimeSpan MinimumSoftDeletedMailboxCleanupAge { get; }

		string NonMovableOrganizationIds { get; }

		int OrganizationGrowthRate { get; }

		int QueryBufferPeriodDays { get; }

		int ReservedCapacityInGb { get; }

		bool SoftDeletedCleanupEnabled { get; }

		int SoftDeletedCleanupThreshold { get; }

		int TransientFailureMaxRetryCount { get; }

		TimeSpan TransientFailureRetryDelay { get; }

		bool UseCachingActiveManager { get; }

		bool UseDatabaseSelectorForMoveInjection { get; }

		bool UseHeatMapProvisioning { get; }

		bool UseParallelDiscovery { get; }

		int WeightDeviationPercent { get; }

		TimeSpan MinimumTimeInDatabaseForItemUpgrade { get; }

		string DisabledMailboxPolicies { get; set; }
	}
}
