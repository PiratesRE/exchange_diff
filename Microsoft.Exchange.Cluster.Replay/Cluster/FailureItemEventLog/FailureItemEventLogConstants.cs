using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.FailureItemEventLog
{
	public static class FailureItemEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcConfiguration9a = new ExEventLog.EventTuple(3221487717U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtConfiguration9a = new ExEventLog.EventTuple(3221487718U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcRepairable9a = new ExEventLog.EventTuple(3221487719U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtRepairable9a = new ExEventLog.EventTuple(3221487720U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_Space9a = new ExEventLog.EventTuple(3221487721U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcIoHard9a = new ExEventLog.EventTuple(3221487722U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtIoHard9a = new ExEventLog.EventTuple(3221487723U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SourceCorruption9a = new ExEventLog.EventTuple(3221487724U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcCorruption9a = new ExEventLog.EventTuple(3221487725U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtCorruption9a = new ExEventLog.EventTuple(3221487726U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHard9a = new ExEventLog.EventTuple(3221487727U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtHard9a = new ExEventLog.EventTuple(3221487728U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcUnrecoverable9a = new ExEventLog.EventTuple(3221487729U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtUnrecoverable9a = new ExEventLog.EventTuple(3221487730U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcRemount9a = new ExEventLog.EventTuple(3221487731U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtRemount9a = new ExEventLog.EventTuple(3221487732U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtReseed9a = new ExEventLog.EventTuple(3221487733U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcPerformance9a = new ExEventLog.EventTuple(3221487734U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MoveLoad9a = new ExEventLog.EventTuple(3221487735U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcMemory9a = new ExEventLog.EventTuple(3221487736U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtMemory9a = new ExEventLog.EventTuple(3221487737U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcCatalogReseed9a = new ExEventLog.EventTuple(3221487738U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TgtCatalogReseed9a = new ExEventLog.EventTuple(3221487739U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AlertOnly9a = new ExEventLog.EventTuple(3221487740U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnexpectedDismount9a = new ExEventLog.EventTuple(3221487742U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcExceededMaxDatabases9a = new ExEventLog.EventTuple(3221487743U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcGenericMountFailure9a = new ExEventLog.EventTuple(3221487744U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PagePatchRequested9a = new ExEventLog.EventTuple(3221487745U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PagePatchCompleted9a = new ExEventLog.EventTuple(1074004098U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LostFlushDetected9a = new ExEventLog.EventTuple(3221487747U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcSourceLogCorrupt9a = new ExEventLog.EventTuple(3221487748U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcFailedToRepair9a = new ExEventLog.EventTuple(3221487749U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LostFlushDbTimeTooOld9a = new ExEventLog.EventTuple(3221487750U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtExceededMaxDatabases9a = new ExEventLog.EventTuple(3221487751U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcLostFlushDbTimeTooNew9a = new ExEventLog.EventTuple(3221487752U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtLostFlushDbTimeTooNew9a = new ExEventLog.EventTuple(3221487753U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcReseed9a = new ExEventLog.EventTuple(3221487754U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtPerformance9a = new ExEventLog.EventTuple(3221487755U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtFailedToRepair9a = new ExEventLog.EventTuple(3221487756U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnexpectedDismountMoveWasSkipped9a = new ExEventLog.EventTuple(1074004109U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtGenericMountFailure9a = new ExEventLog.EventTuple(3221487758U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcExceededMaxActiveDatabases9a = new ExEventLog.EventTuple(3221487759U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtExceededMaxActiveDatabases9a = new ExEventLog.EventTuple(3221487760U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcSourceLogCorruptOutsideRequiredRange9a = new ExEventLog.EventTuple(3221487761U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcGeneric9a = new ExEventLog.EventTuple(3221487762U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtGeneric9a = new ExEventLog.EventTuple(3221487763U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcLogCorruptionDetectedByESE9a = new ExEventLog.EventTuple(3221487764U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtLogCorruptionDetectedByESE9a = new ExEventLog.EventTuple(3221487765U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcFileSystemCorruption9a = new ExEventLog.EventTuple(3221487766U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtFileSystemCorruption9a = new ExEventLog.EventTuple(3221487767U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHungIoLowThreshold9a = new ExEventLog.EventTuple(3221487768U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtHungIoLowThreshold9a = new ExEventLog.EventTuple(3221487769U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHungIoMediumThreshold9a = new ExEventLog.EventTuple(3221487770U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtHungIoMediumThreshold9a = new ExEventLog.EventTuple(3221487771U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHungIoExceededThresholdDoubleDisk9a = new ExEventLog.EventTuple(3221487772U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtHungIoExceededThresholdDoubleDisk9a = new ExEventLog.EventTuple(3221487773U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHungIoExceededThreshold9a = new ExEventLog.EventTuple(3221487774U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtHungIoExceededThreshold9a = new ExEventLog.EventTuple(3221487775U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHungIoCancelFailed9a = new ExEventLog.EventTuple(3221487776U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtHungIoCancelFailed9a = new ExEventLog.EventTuple(3221487777U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHungIoCancelSucceeded9a = new ExEventLog.EventTuple(3221487778U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtHungIoCancelSucceeded9a = new ExEventLog.EventTuple(3221487779U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHungStoreWorker9a = new ExEventLog.EventTuple(3221487780U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtHungStoreWorker9a = new ExEventLog.EventTuple(3221487781U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcUnaccessibleStoreWorker9a = new ExEventLog.EventTuple(3221487782U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtUnaccessibleStoreWorker9a = new ExEventLog.EventTuple(3221487783U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcMonitoredDatabaseFailed9a = new ExEventLog.EventTuple(3221487784U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogGapFatal9a = new ExEventLog.EventTuple(3221487785U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtMonitoredDatabaseFailed9a = new ExEventLog.EventTuple(3221487786U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcExceededDatabaseMaxSize9a = new ExEventLog.EventTuple(3221487787U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtExceededDatabaseMaxSize9a = new ExEventLog.EventTuple(3221487788U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtLowDiskSpaceStraggler9a = new ExEventLog.EventTuple(3221487789U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcLockedVolume9a = new ExEventLog.EventTuple(3221487790U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtLockedVolume9a = new ExEventLog.EventTuple(3221487791U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcConfiguration9b = new ExEventLog.EventTuple(3221487817U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtConfiguration9b = new ExEventLog.EventTuple(3221487818U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcRepairable9b = new ExEventLog.EventTuple(3221487819U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_Space9b = new ExEventLog.EventTuple(3221487821U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcIoHard9b = new ExEventLog.EventTuple(3221487822U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtIoHard9b = new ExEventLog.EventTuple(3221487823U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SourceCorruption9b = new ExEventLog.EventTuple(3221487824U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcCorruption9b = new ExEventLog.EventTuple(3221487825U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtCorruption9b = new ExEventLog.EventTuple(3221487826U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHard9b = new ExEventLog.EventTuple(3221487827U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtHard9b = new ExEventLog.EventTuple(3221487828U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcUnrecoverable9b = new ExEventLog.EventTuple(3221487829U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtUnrecoverable9b = new ExEventLog.EventTuple(3221487830U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcRemount9b = new ExEventLog.EventTuple(3221487831U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtReseed9b = new ExEventLog.EventTuple(3221487833U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcPerformance9b = new ExEventLog.EventTuple(3221487834U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MoveLoad9b = new ExEventLog.EventTuple(3221487835U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcMemory9b = new ExEventLog.EventTuple(3221487836U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SrcCatalogReseed9b = new ExEventLog.EventTuple(3221487838U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcSourceLogCorrupt9b = new ExEventLog.EventTuple(3221487840U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnexpectedDismount9b = new ExEventLog.EventTuple(3221487841U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcExceededMaxDatabases9b = new ExEventLog.EventTuple(3221487842U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtExceededMaxDatabases9b = new ExEventLog.EventTuple(3221487843U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcFailedToRepair9b = new ExEventLog.EventTuple(3221487844U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtFailedToRepair9b = new ExEventLog.EventTuple(3221487845U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtLostFlushDbTimeTooNew9b = new ExEventLog.EventTuple(3221487846U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcGenericMountFailure9b = new ExEventLog.EventTuple(3221487847U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AlertOnly9b = new ExEventLog.EventTuple(3221487848U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PagePatchRequested9b = new ExEventLog.EventTuple(3221487849U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LostFlushDetected9b = new ExEventLog.EventTuple(3221487850U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LostFlushDbTimeTooOld9b = new ExEventLog.EventTuple(3221487851U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcExceededMaxActiveDatabases9b = new ExEventLog.EventTuple(3221487852U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcSourceLogCorruptOutsideRequiredRange9b = new ExEventLog.EventTuple(3221487853U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcGeneric9b = new ExEventLog.EventTuple(3221487854U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtGeneric9b = new ExEventLog.EventTuple(3221487855U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcLogCorruptionDetectedByESE9b = new ExEventLog.EventTuple(3221487856U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtLogCorruptionDetectedByESE9b = new ExEventLog.EventTuple(3221487857U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcFileSystemCorruption9b = new ExEventLog.EventTuple(3221487858U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtFileSystemCorruption9b = new ExEventLog.EventTuple(3221487859U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHungIoLowThreshold9b = new ExEventLog.EventTuple(3221487860U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtHungIoLowThreshold9b = new ExEventLog.EventTuple(3221487861U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHungIoMediumThreshold9b = new ExEventLog.EventTuple(3221487862U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtHungIoMediumThreshold9b = new ExEventLog.EventTuple(3221487863U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHungIoExceededThresholdDoubleDisk9b = new ExEventLog.EventTuple(3221487864U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHungIoExceededThreshold9b = new ExEventLog.EventTuple(3221487865U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcHungStoreWorker9b = new ExEventLog.EventTuple(3221487866U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcUnaccessibleStoreWorker9b = new ExEventLog.EventTuple(3221487868U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcMonitoredDatabaseFailed9b = new ExEventLog.EventTuple(3221487869U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogGapFatal9b = new ExEventLog.EventTuple(3221487870U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtMonitoredDatabaseFailed9b = new ExEventLog.EventTuple(3221487871U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcExceededDatabaseMaxSize9b = new ExEventLog.EventTuple(3221487872U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TgtLowDiskSpaceStraggler9b = new ExEventLog.EventTuple(3221487873U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SrcLockedVolume9b = new ExEventLog.EventTuple(3221487874U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			Database_recovery = 1,
			Ese,
			Store,
			Content_Indexing,
			Replay
		}

		internal enum Message : uint
		{
			SrcConfiguration9a = 3221487717U,
			TgtConfiguration9a,
			SrcRepairable9a,
			TgtRepairable9a,
			Space9a,
			SrcIoHard9a,
			TgtIoHard9a,
			SourceCorruption9a,
			SrcCorruption9a,
			TgtCorruption9a,
			SrcHard9a,
			TgtHard9a,
			SrcUnrecoverable9a,
			TgtUnrecoverable9a,
			SrcRemount9a,
			TgtRemount9a,
			TgtReseed9a,
			SrcPerformance9a,
			MoveLoad9a,
			SrcMemory9a,
			TgtMemory9a,
			SrcCatalogReseed9a,
			TgtCatalogReseed9a,
			AlertOnly9a,
			UnexpectedDismount9a = 3221487742U,
			SrcExceededMaxDatabases9a,
			SrcGenericMountFailure9a,
			PagePatchRequested9a,
			PagePatchCompleted9a = 1074004098U,
			LostFlushDetected9a = 3221487747U,
			SrcSourceLogCorrupt9a,
			SrcFailedToRepair9a,
			LostFlushDbTimeTooOld9a,
			TgtExceededMaxDatabases9a,
			SrcLostFlushDbTimeTooNew9a,
			TgtLostFlushDbTimeTooNew9a,
			SrcReseed9a,
			TgtPerformance9a,
			TgtFailedToRepair9a,
			UnexpectedDismountMoveWasSkipped9a = 1074004109U,
			TgtGenericMountFailure9a = 3221487758U,
			SrcExceededMaxActiveDatabases9a,
			TgtExceededMaxActiveDatabases9a,
			SrcSourceLogCorruptOutsideRequiredRange9a,
			SrcGeneric9a,
			TgtGeneric9a,
			SrcLogCorruptionDetectedByESE9a,
			TgtLogCorruptionDetectedByESE9a,
			SrcFileSystemCorruption9a,
			TgtFileSystemCorruption9a,
			SrcHungIoLowThreshold9a,
			TgtHungIoLowThreshold9a,
			SrcHungIoMediumThreshold9a,
			TgtHungIoMediumThreshold9a,
			SrcHungIoExceededThresholdDoubleDisk9a,
			TgtHungIoExceededThresholdDoubleDisk9a,
			SrcHungIoExceededThreshold9a,
			TgtHungIoExceededThreshold9a,
			SrcHungIoCancelFailed9a,
			TgtHungIoCancelFailed9a,
			SrcHungIoCancelSucceeded9a,
			TgtHungIoCancelSucceeded9a,
			SrcHungStoreWorker9a,
			TgtHungStoreWorker9a,
			SrcUnaccessibleStoreWorker9a,
			TgtUnaccessibleStoreWorker9a,
			SrcMonitoredDatabaseFailed9a,
			LogGapFatal9a,
			TgtMonitoredDatabaseFailed9a,
			SrcExceededDatabaseMaxSize9a,
			TgtExceededDatabaseMaxSize9a,
			TgtLowDiskSpaceStraggler9a,
			SrcLockedVolume9a,
			TgtLockedVolume9a,
			SrcConfiguration9b = 3221487817U,
			TgtConfiguration9b,
			SrcRepairable9b,
			Space9b = 3221487821U,
			SrcIoHard9b,
			TgtIoHard9b,
			SourceCorruption9b,
			SrcCorruption9b,
			TgtCorruption9b,
			SrcHard9b,
			TgtHard9b,
			SrcUnrecoverable9b,
			TgtUnrecoverable9b,
			SrcRemount9b,
			TgtReseed9b = 3221487833U,
			SrcPerformance9b,
			MoveLoad9b,
			SrcMemory9b,
			SrcCatalogReseed9b = 3221487838U,
			SrcSourceLogCorrupt9b = 3221487840U,
			UnexpectedDismount9b,
			SrcExceededMaxDatabases9b,
			TgtExceededMaxDatabases9b,
			SrcFailedToRepair9b,
			TgtFailedToRepair9b,
			TgtLostFlushDbTimeTooNew9b,
			SrcGenericMountFailure9b,
			AlertOnly9b,
			PagePatchRequested9b,
			LostFlushDetected9b,
			LostFlushDbTimeTooOld9b,
			SrcExceededMaxActiveDatabases9b,
			SrcSourceLogCorruptOutsideRequiredRange9b,
			SrcGeneric9b,
			TgtGeneric9b,
			SrcLogCorruptionDetectedByESE9b,
			TgtLogCorruptionDetectedByESE9b,
			SrcFileSystemCorruption9b,
			TgtFileSystemCorruption9b,
			SrcHungIoLowThreshold9b,
			TgtHungIoLowThreshold9b,
			SrcHungIoMediumThreshold9b,
			TgtHungIoMediumThreshold9b,
			SrcHungIoExceededThresholdDoubleDisk9b,
			SrcHungIoExceededThreshold9b,
			SrcHungStoreWorker9b,
			SrcUnaccessibleStoreWorker9b = 3221487868U,
			SrcMonitoredDatabaseFailed9b,
			LogGapFatal9b,
			TgtMonitoredDatabaseFailed9b,
			SrcExceededDatabaseMaxSize9b,
			TgtLowDiskSpaceStraggler9b,
			SrcLockedVolume9b
		}
	}
}
