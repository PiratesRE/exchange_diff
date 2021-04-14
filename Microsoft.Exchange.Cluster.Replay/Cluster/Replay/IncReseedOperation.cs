using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum IncReseedOperation
	{
		IsIncrementalReseedRequiredOverall = 1,
		CheckForDivergenceAfterSeeding,
		CheckSourceDatabaseMountedFirst,
		QueryLogRangeFirst,
		PerformIncrementalReseedOverall,
		FindDivergencePoint,
		PrepareIncReseedV2Overall,
		RedirtyDatabase,
		PauseTruncation,
		GeneratePageListSinceDivergence,
		ReadDatabasePagesFromActive,
		CopyAndInspectRequiredLogFiles,
		PatchDatabaseOverall,
		ReplaceLogFiles,
		ReplaceE00LogTransacted,
		EnsureTargetDismounted,
		IsLogfileEqual,
		IsLogFileSubset
	}
}
