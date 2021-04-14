using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class IncReseedPerformanceTracker : FailoverPerformanceTrackerBase<IncReseedOperation>
	{
		public IncReseedPerformanceTracker(IReplayConfiguration config) : base("IncReseedPerf")
		{
			this.m_config = config;
		}

		public bool IsRunningACLL { private get; set; }

		public bool IsRestartedIncReseed { private get; set; }

		public bool IsFailedPassivePagePatch { private get; set; }

		public bool IsE00LogExists { private get; set; }

		public bool IsDivergentAfterSeed { private get; set; }

		public bool IsPreviousLogNotBinaryEqual { private get; set; }

		public bool IsIncReseedNeeded { private get; set; }

		public bool IsIncReseedV1Performed { get; set; }

		public bool IsDatabaseConsistent { private get; set; }

		public bool IsPagesReferencedInDivergentLogs { private get; set; }

		public long PassiveEOLGen { private get; set; }

		public long NumPagesToBePatched { private get; set; }

		public long ActiveHighestLogGen { private get; set; }

		public long HighestLogGenRequired { private get; set; }

		public long LowestLogGenRequired { private get; set; }

		public long FirstDivergedLogGen { private get; set; }

		public string SourceServer { private get; set; }

		public override void LogEvent()
		{
			ReplayCrimsonEvents.IncrementalReseedPerformance.Log<string, Guid, bool, bool, bool, bool, bool, bool, bool, bool, bool, long, long, long, long, long, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, string, long, TimeSpan, TimeSpan, TimeSpan, TimeSpan, bool>(this.m_config.DatabaseName, this.m_config.IdentityGuid, this.IsRunningACLL, this.IsRestartedIncReseed, this.IsFailedPassivePagePatch, this.IsE00LogExists, this.IsDivergentAfterSeed, this.IsIncReseedNeeded, this.IsIncReseedV1Performed, this.IsDatabaseConsistent, this.IsPagesReferencedInDivergentLogs, this.PassiveEOLGen, this.NumPagesToBePatched, this.ActiveHighestLogGen, this.HighestLogGenRequired, this.LowestLogGenRequired, base.GetDuration(IncReseedOperation.IsIncrementalReseedRequiredOverall), base.GetDuration(IncReseedOperation.CheckForDivergenceAfterSeeding), base.GetDuration(IncReseedOperation.CheckSourceDatabaseMountedFirst), base.GetDuration(IncReseedOperation.QueryLogRangeFirst), base.GetDuration(IncReseedOperation.PerformIncrementalReseedOverall), base.GetDuration(IncReseedOperation.FindDivergencePoint), base.GetDuration(IncReseedOperation.PrepareIncReseedV2Overall), base.GetDuration(IncReseedOperation.RedirtyDatabase), base.GetDuration(IncReseedOperation.PauseTruncation), base.GetDuration(IncReseedOperation.GeneratePageListSinceDivergence), base.GetDuration(IncReseedOperation.ReadDatabasePagesFromActive), base.GetDuration(IncReseedOperation.CopyAndInspectRequiredLogFiles), base.GetDuration(IncReseedOperation.PatchDatabaseOverall), base.GetDuration(IncReseedOperation.ReplaceLogFiles), this.SourceServer, this.FirstDivergedLogGen, base.GetDuration(IncReseedOperation.ReplaceE00LogTransacted), base.GetDuration(IncReseedOperation.EnsureTargetDismounted), base.GetDuration(IncReseedOperation.IsLogfileEqual), base.GetDuration(IncReseedOperation.IsLogFileSubset), this.IsPreviousLogNotBinaryEqual);
		}

		private IReplayConfiguration m_config;
	}
}
