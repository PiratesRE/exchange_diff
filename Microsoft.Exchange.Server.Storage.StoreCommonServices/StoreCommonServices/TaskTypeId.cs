using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public enum TaskTypeId : byte
	{
		None,
		ForTestPurposes,
		BadPlanDetector = 11,
		CheckpointSmoother,
		ApplyLogicalIndexMaintenance,
		LongOperationReporter,
		EnumeratePreQuarantinedMailboxes,
		CleanupNonActiveMailboxStates,
		MaintenanceIdleCheck,
		MarkForMaintenance,
		ResourceMonitorDigest,
		DrainSearchQueue,
		DismountDatabase,
		TimedEventsProcessing,
		PropertyPromotion,
		SearchFolderPopulation,
		RopSummaryCollection,
		PerfCounterFlush,
		FlushDirtyPerUserCaches,
		PropertyPromotionBootstrap,
		CategorizedViewSearchFolderRestriction,
		OnlineIntegrityCheck,
		DatabaseSizeCheck,
		RopLockContentionCollection,
		RopResourceCollection,
		CalculateTombstoneTableSize,
		UrgentTombstoneTableCleanup,
		FlushEventCounterUpperBound
	}
}
