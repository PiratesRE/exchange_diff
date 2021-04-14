﻿using System;

namespace Microsoft.Exchange.Common.HA
{
	internal enum FailureTag : uint
	{
		NoOp,
		Configuration,
		Repairable,
		Space,
		IoHard,
		SourceCorruption,
		Corruption,
		Hard,
		Unrecoverable,
		Remount,
		Reseed,
		Performance,
		MoveLoad,
		Memory,
		CatalogReseed,
		AlertOnly,
		ExpectedDismount,
		UnexpectedDismount,
		ExceededMaxDatabases,
		GenericMountFailure,
		PagePatchRequested,
		PagePatchCompleted,
		LostFlushDetected,
		SourceLogCorruption,
		FailedToRepair,
		LostFlushDbTimeTooOld,
		LostFlushDbTimeTooNew,
		ExceededMaxActiveDatabases,
		SourceLogCorruptionOutsideRequiredRange,
		HungIoExceededThreshold,
		HungIoCancelSucceeded,
		HungIoCancelFailed,
		RecoveryRedoLogCorruption,
		ReplayFailedToPagePatch,
		FileSystemCorruption,
		HungIoLowThreshold,
		HungIoMediumThreshold,
		HungIoExceededThresholdDoubleDisk,
		HungStoreWorker,
		UnaccessibleStoreWorker,
		MonitoredDatabaseFailed,
		LogGapFatal,
		ExceededDatabaseMaxSize,
		LowDiskSpaceStraggler,
		LockedVolume
	}
}
